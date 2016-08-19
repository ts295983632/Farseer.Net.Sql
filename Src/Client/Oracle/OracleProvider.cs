using System;
using System.Data;
using System.Data.Common;
using System.Data.OracleClient;
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
        protected override string ParamsPrefix => ":";
        public override string KeywordAegis(string fieldName) => fieldName;
        public override bool IsSupportTransaction => true;
        internal override AbsSqlBuilder CreateSqlBuilder(ExpressionBuilder expBuilder, string name) => new OracleBuilder(this, expBuilder, name);
        public override string CreateDbConnstring(string userID, string passWord, string server, string catalog, string dataVer, string additional, int connectTimeout = 60, int poolMinSize = 16, int poolMaxSize = 100, string port = "")
        {
            if (string.IsNullOrWhiteSpace(port)) { port = "1521"; }
            return string.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={3})))(CONNECT_DATA=(SERVER=DEDICATED)(SID={4})));User Id={1};Password={2};{5}", server, userID, passWord, port, catalog, additional);
        }

        /// <summary>
        ///     根据值，返回类型
        /// </summary>
        /// <param name="type">参数类型</param>
        /// <param name="len">参数长度</param>
        protected override DbType GetDbType(Type type, out int len)
        {
            type = type.GetNullableArguments();
            if (type.Name != "DateTime") { return base.GetDbType(type, out len); }
            len = 8;
            return DbType.Date;
        }
    }
}