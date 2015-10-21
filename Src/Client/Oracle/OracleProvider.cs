using System.Data.Common;
using FS.Sql.Infrastructure;
using FS.Sql.Internal;

namespace FS.Sql.Client.Oracle
{
    /// <summary>
    ///     Oracle 数据库提供者（不同数据库的特性）
    /// </summary>
    public class OracleProvider : AbsDbProvider
    {
        public override string ParamsPrefix => ":";

        public override string KeywordAegis(string fieldName)
        {
            return fieldName;
        }

        public override DbProviderFactory GetDbProviderFactory => DbProviderFactories.GetFactory("System.Data.OracleClient");

        internal override ISqlBuilder CreateSqlBuilder(ExpressionBuilder expBuilder, string name)
        {
            return new OracleSqlBuilder(this, expBuilder, name);
        }

        public override string CreateDbConnstring(string userID, string passWord, string server, string catalog, string dataVer, int connectTimeout = 60, int poolMinSize = 16, int poolMaxSize = 100, string port = "")
        {
            if (string.IsNullOrWhiteSpace(port)) { port = "1521"; }
            return string.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={3})))(CONNECT_DATA=(SERVER=DEDICATED)(SID={4})));User Id={1};Password={2};", server, userID, passWord, port, catalog);
        }
    }
}