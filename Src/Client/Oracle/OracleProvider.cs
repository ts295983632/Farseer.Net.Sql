using System;
using System.Data;
using System.Data.Common;
using System.Reflection;
using FS.Cache;
using FS.Extends;
using FS.Sql.Infrastructure;
using FS.Sql.Internal;

namespace FS.Sql.Client.Oracle
{
    /// <summary>
    ///     Oracle 数据库提供者（不同数据库的特性）
    /// </summary>
    public class OracleProvider : AbsDbProvider
    {
        public override DbProviderFactory DbProviderFactory => (DbProviderFactory)InstanceCacheManger.Cache(Assembly.Load("Oracle.ManagedDataAccess").GetType("Oracle.ManagedDataAccess.Client.OracleClientFactory"));
        public override AbsFunctionProvider FunctionProvider => new OracleFunctionProvider();
        public override string ParamsPrefix => ":";
        public override string KeywordAegis(string fieldName) => fieldName;
        public override bool IsSupportTransaction => true;
        internal override AbsSqlBuilder CreateSqlBuilder(ExpressionBuilder expBuilder, string name) => new OracleBuilder(this, expBuilder, name);
        public override string CreateDbConnstring(string userID, string passWord, string server, string catalog, string dataVer, string additional, int connectTimeout = 60, int poolMinSize = 16, int poolMaxSize = 100, string port = "")
        {
            if (string.IsNullOrWhiteSpace(port)) { port = "1521"; }
            return string.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={3})))(CONNECT_DATA=(SERVER=DEDICATED)(SID={4})));User Id={1};Password={2};{5}", server, userID, passWord, port, catalog, additional);
        }

        ///// <summary>
        /////     创建一个数据库参数对象
        ///// </summary>
        ///// <param name="name">参数名称</param>
        ///// <param name="valu">参数值</param>
        ///// <param name="valType">值类型</param>
        ///// <param name="output">是否是输出值</param>
        //public override DbParameter CreateDbParam(string name, object valu, Type valType, bool output = false)
        //{
        //    int len;
        //    var type = GetDbType(valType, out len);
        //    if (type == DbType.Boolean) { type = DbType.Int32;}
        //    return CreateDbParam(name, valu, type, len, output);
        //}

        /// <summary>
        ///     根据值，返回类型
        /// </summary>
        /// <param name="type">参数类型</param>
        /// <param name="len">参数长度</param>
        public override DbType GetDbType(Type type, out int len)
        {
            type = type.GetNullableArguments();
            if (type.BaseType != null && type.BaseType == typeof(Enum))
            {
                len = 1;
                return DbType.Byte;
            }
            switch (type.Name)
            {
                case "DateTime": len = 8; return DbType.Date;
                case "Boolean": len = 1; return DbType.Int32;
                case "Int32": len = 4; return DbType.Int32;
                case "Int16": len = 2; return DbType.Int16;
                case "Decimal": len = 8; return DbType.Decimal;
                case "Byte": len = 1; return DbType.Byte;
                case "Long":
                case "Float":
                case "Double": len = 8; return DbType.Decimal;
                case "Guid": len = 16; return DbType.Guid;
                default: len = 0; return DbType.String;
            }
        }
    }
}