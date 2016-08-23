// ********************************************
// 作者：steden QQ：11042427
// 时间：2016-08-22 10:20
// ********************************************

using System;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using FS.Extends;
using FS.Map;
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
            sb.AppendLine("using System.Data;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using FS.Utils.Common;");
            sb.AppendLine("using FS.Extends;");

            sb.AppendLine($"namespace {entityType.Namespace}\r\n{{");
            sb.AppendLine($"public class {clsName} : {entityType.FullName}\r\n{{");
            // DataRow构造
            sb.AppendLine(CreateDataRowMethod(entityType, setPhysicsMap));
            // DataTable构造
            sb.AppendLine(CreateDataTableMethod(entityType, setPhysicsMap));
            // IDataReader构造
            sb.AppendLine(CreateDataReaderMethod(entityType, setPhysicsMap));

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
            compilerParams.ReferencedAssemblies.Add("Farseer.Net.dll");
            compilerParams.ReferencedAssemblies.Add("Farseer.Net.Sql.dll");
            // 需要把基类型的dll，也载进来
            var baseType = entityType;
            while (baseType != null)
            {
                compilerParams.ReferencedAssemblies.Add(baseType.Assembly.ManifestModule.Name);
                baseType = baseType.BaseType;
            }

            var compiler = CodeDomProvider.CreateProvider("CSharp");
            //编译
            var results = compiler.CompileAssemblyFromSource(compilerParams, sb.ToString());
            return results.CompiledAssembly.GetExportedTypes()[0];
        }

        /// <summary> 生成DataTable转换方法 </summary>
        private string CreateDataTableMethod(Type entityType, SetPhysicsMap setPhysicsMap)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"\t public static List<{entityType.FullName}> ConvertDataTable (DataTable dt)\r\n{{");
            sb.AppendLine($"var lst = new List<{entityType.FullName}>(dt.Rows.Count);");
            sb.AppendLine($"foreach (DataRow dr in dt.Rows)\r\n{{");
            sb.AppendLine($"lst.Add(ConvertDataRow(dr));");
            sb.AppendLine("}");
            sb.AppendLine($"return lst;");
            sb.AppendLine("}");
            return sb.ToString();
        }
        /// <summary> 生成DataRow转换方法 </summary>
        private string CreateDataRowMethod(Type entityType, SetPhysicsMap setPhysicsMap)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"\t public static {entityType.FullName} ConvertDataRow (DataRow dr)\r\n{{");
            sb.AppendLine($"var entity = new {entityType.FullName}();");
            foreach (var map in setPhysicsMap.MapList)
            {
                var filedName = map.Value.Field.IsFun ? map.Key.Name : map.Value.Field.Name;
                // DataRow是否包含字段
                sb.Append($"\t\t if (dr.Table.Columns.Contains(\"{filedName}\") && dr[\"{filedName}\"]!=null) {{ ");
                // 创建赋值
                sb.AppendLine(CreateAssign(map, filedName, "dr"));
                sb.AppendLine("}");
            }
            sb.AppendLine($"return entity;");
            sb.AppendLine("}");
            return sb.ToString();
        }
        /// <summary> 生成DataReader转换方法 </summary>
        private string CreateDataReaderMethod(Type entityType, SetPhysicsMap setPhysicsMap)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"\t public static {entityType.FullName} ConvertDataReader (IDataReader reader)\r\n{{");
            sb.AppendLine($"var entity = new {entityType.FullName}();");
            sb.AppendLine($"var isHaveValue = false;");
            sb.AppendLine($"if (reader.Read())\r\n{{");
            sb.AppendLine($"isHaveValue = true;");
            foreach (var map in setPhysicsMap.MapList)
            {
                var filedName = map.Value.Field.IsFun ? map.Key.Name : map.Value.Field.Name;
                // DataReader是否包含字段
                sb.AppendLine($"if (reader.HaveName(\"{filedName}\"))\r\n{{");
                // 创建赋值
                sb.AppendLine(CreateAssign(map, filedName, "reader"));
                sb.AppendLine("}");
            }
            sb.AppendLine("}");
            sb.AppendLine($"return isHaveValue ? entity : null;");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string CreateAssign(KeyValuePair<PropertyInfo, FieldMapState> map, string filedName, string dataRowOrDataReader)
        {
            var sb = new StringBuilder();
            // 字段赋值
            var propertyAssign = $"entity.{map.Key.Name} = ";
            var rowsVal = $"{dataRowOrDataReader}[\"{filedName}\"]";
            // 类型转换
            var propertyType = map.Key.PropertyType.GetNullableArguments();

            // 如果是List类型，则使用ConvertType
            if (propertyType.IsGenericType)
            {
                // 使用FS的ConvertHelper 进行类型转换
                var asType = propertyType.IsArray ? $"{propertyType.GetGenericArguments()[0].FullName}[]" : $"List<{propertyType.GetGenericArguments()[0].FullName}>";
                var convertType = $"ConvertHelper.ConvertType({rowsVal},typeof({asType}))";
                sb.Append(propertyAssign + convertType + " as " + asType + "; ");
            }
            else
            {
                // 字符串不需要处理
                if (propertyType == typeof(string)) { sb.Append($"{propertyAssign}{rowsVal}.ToString();"); }
                else if (propertyType == typeof(bool)) { sb.Append($"{propertyAssign}{rowsVal}.ConvertType(false);"); }
                else if (propertyType.IsEnum) { sb.Append($"{propertyAssign}({propertyType}){rowsVal}.ConvertType(typeof({propertyType}));"); }
                else if (!propertyType.IsClass)
                {
                    sb.Append($"{propertyType.FullName} {filedName}_Out;"); // 定义用于out输出的类型
                    sb.Append($"if ({propertyType.FullName}.TryParse({rowsVal}.ToString(), out {filedName}_Out)) {{ {propertyAssign}{filedName}_Out; }}");
                }
            }
            return sb.ToString();
        }

    }
}