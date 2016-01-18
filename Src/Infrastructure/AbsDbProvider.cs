using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using FS.Cache;
using FS.Configs;
using FS.Infrastructure;
using FS.Sql.Client.MySql;
using FS.Sql.Client.OleDb;
using FS.Sql.Client.Oracle;
using FS.Sql.Client.PostgreSql;
using FS.Sql.Client.SqlServer;
using FS.Sql.Client.SqLite;
using FS.Sql.ExpressionVisitor;
using FS.Sql.Internal;
using FS.Sql.Map;
using FS.Utils.Common;
using FS.Extends;

namespace FS.Sql.Infrastructure
{
    /// <summary>
    ///     数据库提供者（不同数据库的特性）
    /// </summary>
    public abstract class AbsDbProvider
    {
        /// <summary>
        ///     参数前缀
        /// </summary>
        public virtual string ParamsPrefix => "@";

        /// <summary>
        ///     创建提供程序对数据源类的实现的实例
        /// </summary>
        public abstract DbProviderFactory DbProviderFactory { get; }

        /// <summary>
        ///     创建提供程序对数据源类的实现的实例
        /// </summary>
        public abstract AbsFunctionProvider FunctionProvider { get; }

        /// <summary>
        ///     是否支持事务操作
        /// </summary>
        public abstract bool IsSupportTransaction { get; }

        /// <summary>
        ///     创建字段保护符
        /// </summary>
        /// <param name="fieldName">字符名称</param>
        public virtual string KeywordAegis(string fieldName)
        {
            return $"[{fieldName}]";
        }

        #region 创建参数

        /// <summary>
        ///     将C#值转成数据库能存储的值
        /// </summary>
        /// <param name="valu"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object ParamConvertValue(object valu, DbType type)
        {
            if (valu == null) { return null; }

            // 时间类型转换
            if (type == DbType.DateTime)
            {
                DateTime dtValue;
                DateTime.TryParse(valu.ToString(), out dtValue);
                if (dtValue == DateTime.MinValue) { valu = new DateTime(1900, 1, 1); }
            }
            // 枚举类型转换
            if (valu is Enum) { valu = Convert.ToInt32(valu); }

            // List类型转换成字符串并以,分隔
            if (valu.GetType().IsGenericType)
            {
                var sb = new StringBuilder();
                // list类型
                if (valu.GetType().GetGenericTypeDefinition() != typeof(Nullable<>))
                {
                    var enumerator = ((IEnumerable)valu).GetEnumerator();
                    while (enumerator.MoveNext()) { sb.Append(enumerator.Current + ","); }
                }
                else if (valu.GetType().GetGenericArguments()[0] == typeof(int))
                {
                    var enumerator = ((IEnumerable<int?>)valu).GetEnumerator();
                    while (enumerator.MoveNext()) { sb.Append(enumerator.Current.GetValueOrDefault() + ","); }
                }
                valu = sb.Length > 0 ? sb.Remove(sb.Length - 1, 1).ToString() : "";
            }
            return valu;
        }

        /// <summary>
        ///     根据值，返回类型
        /// </summary>
        /// <param name="type">参数类型</param>
        /// <param name="len">参数长度</param>
        public DbType GetDbType(Type type, out int len)
        {
            type = type.GetNullableArguments();
            if (type.BaseType != null && type.BaseType == typeof(Enum))
            {
                len = 1;
                return DbType.Byte;
            }
            switch (type.Name)
            {
                case "DateTime":
                    len = 8;
                    return DbType.DateTime;
                case "Boolean":
                    len = 1;
                    return DbType.Boolean;
                case "Int32":
                    len = 4;
                    return DbType.Int32;
                case "Int16":
                    len = 2;
                    return DbType.Int16;
                case "Decimal":
                    len = 8;
                    return DbType.Decimal;
                case "Byte":
                    len = 1;
                    return DbType.Byte;
                case "Long":
                case "Float":
                case "Double":
                    len = 8;
                    return DbType.Decimal;
                case "Guid":
                    len = 16;
                    return DbType.Guid;
                default:
                    len = 0;
                    return DbType.String;
            }
        }

        /// <summary>
        ///     创建一个数据库参数对象
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="valu">参数值</param>
        /// <param name="type">参数类型</param>
        /// <param name="len">参数长度</param>
        /// <param name="output">是否是输出值</param>
        public DbParameter CreateDbParam(string name, object valu, DbType type, int len, bool output = false)
        {
            var param = DbProviderFactory.CreateParameter();
            param.DbType = type;
            param.ParameterName = ParamsPrefix + name;
            param.Value = ParamConvertValue(valu, type);
            if (len > 0) param.Size = len;
            if (output) param.Direction = ParameterDirection.Output;
            return param;
        }

        /// <summary>
        ///     创建一个数据库参数对象
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="valu">参数值</param>
        /// <param name="valType">值类型</param>
        /// <param name="output">是否是输出值</param>
        public DbParameter CreateDbParam(string name, object valu, Type valType, bool output = false)
        {
            int len;
            var type = GetDbType(valType, out len);
            return CreateDbParam(name, valu, type, len, output);
        }

        /// <summary>
        ///     创建一个数据库参数对象
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="valu">参数值</param>
        /// <param name="output">是否是输出值</param>
        public DbParameter CreateDbParam(string name, object valu, bool output = false)
        {
            return CreateDbParam(name, valu, valu.GetType(), output);
        }

        #endregion

        #region 返回数据库连接字符串

        /// <summary>
        ///     创建数据库连接字符串
        /// </summary>
        /// <param name="dbIndex">数据库配置</param>
        public string CreateDbConnstring(int dbIndex = 0)
        {
            DbInfo dbInfo = dbIndex;
            return CreateDbConnstring(dbInfo.UserID, dbInfo.PassWord, dbInfo.Server, dbInfo.Catalog, dbInfo.DataVer, dbInfo.Additional, dbInfo.ConnectTimeout, dbInfo.PoolMinSize, dbInfo.PoolMaxSize, dbInfo.Port);
        }

        /// <summary>
        ///     创建数据库连接字符串
        /// </summary>
        /// <param name="userID">账号</param>
        /// <param name="passWord">密码</param>
        /// <param name="server">服务器地址</param>
        /// <param name="catalog">表名</param>
        /// <param name="dataVer">数据库版本</param>
        /// <param name="additional">自定义连接字符串</param>
        /// <param name="connectTimeout">链接超时时间</param>
        /// <param name="poolMinSize">连接池最小数量</param>
        /// <param name="poolMaxSize">连接池最大数量</param>
        /// <param name="port">端口</param>
        public abstract string CreateDbConnstring(string userID, string passWord, string server, string catalog, string dataVer, string additional, int connectTimeout = 60, int poolMinSize = 16, int poolMaxSize = 100, string port = "");

        /// <summary>
        ///     获取数据库文件的路径
        /// </summary>
        /// <param name="filePath">数据库路径</param>
        protected string GetFilePath(string filePath)
        {
            if (filePath.IndexOf(':') > -1) { return filePath; }

            var fileName = filePath.Replace("/", "\\");
            if (fileName.StartsWith("/")) { fileName = fileName.Substring(1); }
            fileName = SysMapPath.AppData + fileName;
            return fileName;
        }

        #endregion

        #region 返回DbProvider

        /// <summary>
        ///     返回数据库类型名称
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <param name="dataVer">数据库版本</param>
        public static AbsDbProvider CreateInstance(eumDbType dbType, string dataVer = null)
        {
            switch (dbType)
            {
                case eumDbType.OleDb: return new OleDbProvider();
                case eumDbType.MySql: return new MySqlProvider();
                case eumDbType.SQLite: return new SqLiteProvider();
                case eumDbType.Oracle: return new OracleProvider();
                case eumDbType.PostgreSql: return new PostgreSqlProvider();
            }
            switch (dataVer)
            {
                case "2000": return new SqlServer2000Provider();
            }
            return new SqlServerProvider();
        }

        #endregion

        /// <summary>
        ///     创建SQL查询
        /// </summary>
        /// <param name="expBuilder">表达式持久化</param>
        /// <param name="name">表名/视图名/存储过程名</param>
        /// 
        internal abstract AbsSqlBuilder CreateSqlBuilder(ExpressionBuilder expBuilder, string name);

        /// <summary>
        ///     存储过程创建SQL 输入、输出参数化
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="map">实体类结构</param>
        /// <param name="entity">实体类</param>
        public List<DbParameter> InitParam<TEntity>(SetPhysicsMap map, TEntity entity) where TEntity : class, new()
        {
            var lstParam = new List<DbParameter>();
            if (entity == null) { return lstParam; }
            foreach (var kic in map.MapList.Where(o => o.Value.Field.IsInParam || o.Value.Field.IsOutParam))
            {
                var obj = PropertyGetCacheManger.Cache(kic.Key, entity);
                lstParam.Add(CreateDbParam(kic.Value.Field.Name, obj, kic.Key.PropertyType, kic.Value.Field.IsOutParam));
            }
            return lstParam;
        }

        /// <summary>
        ///     将OutPut参数赋值到实体
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="map">实体类结构</param>
        /// <param name="lstParam">SQL参数列表</param>
        /// <param name="entity">实体类</param>
        public void SetParamToEntity<TEntity>(SetPhysicsMap map, List<DbParameter> lstParam, TEntity entity) where TEntity : class, new()
        {
            if (entity == null) { return; }
            foreach (var kic in map.MapList.Where(o => o.Value.Field.IsOutParam))
            {
                var oVal = ConvertHelper.ConvertType(lstParam.Find(o => o.ParameterName == ParamsPrefix + kic.Value.Field.Name).Value, kic.Key.PropertyType);
                PropertySetCacheManger.Cache(kic.Key, entity, oVal);
            }
        }

        #region 表达式树的解释器
        /// <summary>
        ///     提供字段赋值时表达式树的解析
        /// </summary>
        /// <param name="map">字段映射</param>
        /// <param name="paramList">SQL参数列表</param>
        public virtual AssignVisitor CreateAssignVisitor(SetDataMap map, List<DbParameter> paramList) => new AssignVisitor(this, map, paramList);
        /// <summary>
        ///     提供字段插入表达式树的解析
        /// </summary>
        /// <param name="map">字段映射</param>
        /// <param name="paramList">SQL参数列表</param>
        public virtual InsertVisitor CreateInsertVisitor(SetDataMap map, List<DbParameter> paramList) => new InsertVisitor(this, map, paramList);
        /// <summary>
        ///     提供字段排序时表达式树的解析
        /// </summary>
        /// <param name="map">字段映射</param>
        /// <param name="paramList">SQL参数列表</param>
        public virtual OrderByVisitor CreateOrderByVisitor(SetDataMap map, List<DbParameter> paramList) => new OrderByVisitor(this, map, paramList);
        /// <summary>
        ///     Select筛选字段时表达式树的解析
        /// </summary>
        /// <param name="map">字段映射</param>
        /// <param name="paramList">SQL参数列表</param>
        public virtual SelectVisitor CreateSelectVisitor(SetDataMap map, List<DbParameter> paramList) => new SelectVisitor(this, map, paramList);
        /// <summary>
        ///     Select筛选字段时表达式树的解析
        /// </summary>
        /// <param name="map">字段映射</param>
        /// <param name="paramList">SQL参数列表</param>
        public virtual WhereVisitor CreateWhereVisitor(SetDataMap map, List<DbParameter> paramList) => new WhereVisitor(this, map, paramList);
        #endregion
    }
}