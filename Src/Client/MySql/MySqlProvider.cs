using System.Data.Common;
using FS.Sql.Infrastructure;
using FS.Sql.Internal;

namespace FS.Sql.Client.MySql
{
    /// <summary>
    ///     MySql 数据库提供者（不同数据库的特性）
    /// </summary>
    public class MySqlProvider : AbsDbProvider
    {
        public override DbProviderFactory GetDbProviderFactory => DbProviderFactories.GetFactory("MySql.Data.MySqlClient");

        public override string KeywordAegis(string fieldName)
        {
            return $"`{fieldName}`";
        }

        internal override ISqlBuilder CreateSqlBuilder(ExpressionBuilder expBuilder, string name)
        {
            return new MySqlSqlBuilder(this, expBuilder, name);
        }

        public override string CreateDbConnstring(string userID, string passWord, string server, string catalog, string dataVer, int connectTimeout = 60, int poolMinSize = 16, int poolMaxSize = 100, string port = "")
        {
            return $"Data Source='{server}';User Id='{userID}';Password='{passWord}';Database='{catalog}';charset='gbk'";
        }
    }
}