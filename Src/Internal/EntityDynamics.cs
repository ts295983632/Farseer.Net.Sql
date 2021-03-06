﻿// ********************************************
// 作者：steden QQ：11042427
// 时间：2016-08-22 10:20
// ********************************************

using System;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using FS.Extends;
using FS.Log;
using FS.Sql.Map;

namespace FS.Sql.Internal
{
    /// <summary>
    /// 实体类动态生成DataRow、IDataReader类型转换构造函数
    /// </summary>
    public class EntityDynamics
    {
        /// <summary>
        /// 生成的派生类dll缓存起来
        /// </summary>
        private static readonly ConcurrentDictionary<Type, Type> DicEntityType = new ConcurrentDictionary<Type, Type>();

        public Type GetEntityType<TEntity>()
        {
            var type = typeof(TEntity);
            if (DicEntityType.ContainsKey(type)) { return DicEntityType[type]; }
            var newType = CreateEntity<TEntity>();
            DicEntityType.TryAdd(type, newType);
            return newType;
        }

        /// <summary>
        /// 根据TEntity实体，动态生成派生类
        /// </summary>
        private Type CreateEntity<TEntity>()
        {
            var entityType = typeof(TEntity);
            var setPhysicsMap = new SetPhysicsMap(entityType);
            var clsName = entityType.Name + "ByDataRow";       // 类名

            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using FS.Utils.Common;");
            sb.AppendLine("using FS.Infrastructure;");

            sb.AppendLine($"namespace {entityType.Namespace}\r\n{{");
            sb.AppendLine($"public class {clsName} : {entityType.FullName}\r\n{{");
            // DataRow构造
            sb.AppendLine(CreateToList(entityType));
            // DataTable构造
            sb.AppendLine(CreateToEntity(entityType, setPhysicsMap));

            sb.AppendLine("}}");
            var compilerParams = new CompilerParameters
            {
                CompilerOptions = "/target:library /optimize",   //编译器选项设置
                GenerateInMemory = true,                        //编译时在内存输出
                IncludeDebugInformation = false                 //生成调试信息
            };

            //添加相关的引用
            compilerParams.ReferencedAssemblies.Add("System.dll");
            compilerParams.ReferencedAssemblies.Add("System.Data.dll");
            compilerParams.ReferencedAssemblies.Add("System.Xml.dll");
            var ass = AppDomain.CurrentDomain.GetAssemblies();  //  得到当前所有程序集
            compilerParams.ReferencedAssemblies.Add(ass.FirstOrDefault(o => o.ManifestModule.Name == "Farseer.Net.dll")?.Location);
            compilerParams.ReferencedAssemblies.Add(ass.FirstOrDefault(o => o.ManifestModule.Name == "Farseer.Net.Sql.dll")?.Location);

            CompilerResults results = null;
            try
            {
                // 加载成员依赖的类型所在的程序集
                var properties = entityType.GetProperties();
                foreach (var propertyInfo in properties)
                {
                    // 找到真实程序集
                    var declaringType = propertyInfo.PropertyType.GetNullableArguments();
                    while (declaringType.IsGenericType) { declaringType = declaringType.GetGenericType(); }

                    compilerParams.ReferencedAssemblies.Add(declaringType.Assembly.Location);
                }

                // 需要把基类型的dll，也载进来
                var baseType = entityType;
                while (baseType != null)
                {
                    compilerParams.ReferencedAssemblies.Add(baseType.Assembly.Location);
                    baseType = baseType.BaseType;
                }

                var compiler = CodeDomProvider.CreateProvider("CSharp");

                //编译
                results = compiler.CompileAssemblyFromSource(compilerParams, sb.ToString());
                return results.CompiledAssembly.GetExportedTypes()[0];
            }
            catch (Exception exp)
            {
                if (results != null)
                {
                    var error = new string[results.Errors.Count];
                    for (int i = 0; i < error.Length; i++)
                    {
                        error[i] = results.Errors[i].ErrorText;
                        LogManger.Log.Error(error[i]);
                    }
                    throw new Exception(error.ToString(","));
                }
                LogManger.Log.Error(exp.ToString());
                throw exp;
            }
        }

        /// <summary> 生成ToList转换方法 </summary>
        private string CreateToList(Type entityType)
        {
            var sb = new StringBuilder();
            sb.AppendFormat(@"    
                            public static List<{0}> ToList(MapingData[] mapData)
                            {{
                                var lst = new List<{0}>(mapData[0].DataList.Count);
                                for (int i = 0; i < mapData[0].DataList.Count; i++) {{ lst.Add(ToEntity(mapData, i));}}
                                return lst;
                            }}", entityType.FullName);
            return sb.ToString();
        }

        /// <summary> 生成CreateToEntity转换方法 </summary>
        private string CreateToEntity(Type entityType, SetPhysicsMap setPhysicsMap)
        {
            var sb = new StringBuilder();
            sb.AppendFormat(@"    
            public static {0} ToEntity(MapingData[] mapData, int rowsIndex = 0)
            {{
                if ( mapData == null || mapData.Length == 0 || mapData[0].DataList.Count == 0) {{ return null; }}
                var entity = new {0}();
                foreach (var map in mapData)
                {{
                    var col = map.DataList[rowsIndex];
                    if (col == null) {{ continue; }}
                    switch (map.ColumnName.ToUpper())
                    {{
{1}
                    }}
                }}
                return entity;
            }}", entityType.FullName, CreateSwitchCase(setPhysicsMap));
            return sb.ToString();
        }

        /// <summary> 生成赋值操作 </summary>
        private static string CreateSwitchCase(SetPhysicsMap setPhysicsMap)
        {
            var sb = new StringBuilder();
            foreach (var map in setPhysicsMap.MapList)
            {
                // 字段名
                var filedName = map.Value.Field.IsFun ? map.Key.Name : map.Value.Field.Name;
                // 类型转换
                var propertyType = map.Key.PropertyType.GetNullableArguments();
                // 字段赋值
                var propertyAssign = $"entity.{map.Key.Name}";

                // case 字段名
                sb.Append($"\t\t\tcase \"{filedName.ToUpper()}\":\r\n\t\t\t\t");
                // 使用FS的ConvertHelper 进行类型转换泛型类型
                if (propertyType.IsGenericType)
                {
                    if (propertyType.IsArray)   // 数组类型
                    {
                        var asType = $"{propertyType.GetGenericArguments()[0].FullName}[]";
                        sb.Append($"{propertyAssign} = ConvertHelper.ConvertType(col,typeof({asType})) as {asType}; ");
                    }
                    else   // List集合
                    {
                        sb.Append($"{propertyAssign} = ConvertHelper.ToList<{propertyType.GetGenericArguments()[0].FullName}>(col.ToString()); ");
                    }
                }
                else
                {
                    // 字符串不需要处理
                    if (propertyType == typeof(string)) { sb.Append($"{propertyAssign} = col.ToString();"); }
                    else if (propertyType.IsEnum) { sb.Append($"if (typeof({propertyType.FullName}).GetEnumUnderlyingType() == col.GetType()) {{ {propertyAssign} = ({propertyType.FullName})col; }} else {{ {propertyType.FullName} {filedName}_Out; if (System.Enum.TryParse(col.ToString(), out {filedName}_Out)) {{ {propertyAssign} = {filedName}_Out; }} }}"); }
                    else if (propertyType == typeof(bool)) { sb.Append($"{propertyAssign} = ConvertHelper.ConvertType(col,false);"); }
                    else if (!propertyType.IsClass) { sb.Append($"if (col is {propertyType.FullName}) {{ {propertyAssign} = ({propertyType.FullName})col; }} else {{ {propertyType.FullName} {filedName}_Out; if ({propertyType.FullName}.TryParse(col.ToString(), out {filedName}_Out)) {{ {propertyAssign} = {filedName}_Out; }} }}"); }
                }

                // 退出case
                sb.AppendLine("break;");
            }
            return sb.ToString();
        }
    }
}