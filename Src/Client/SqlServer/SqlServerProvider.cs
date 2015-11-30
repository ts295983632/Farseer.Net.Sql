using System.Data.Common;
using System.Reflection;
using System.Text;
using FS.Cache;
using FS.Sql.Infrastructure;
using FS.Sql.Internal;

namespace FS.Sql.Client.SqlServer
{
    /// <summary>
    ///     SqlServer 数据库提供者（不同数据库的特性）
    /// </summary>
    public class SqlServerProvider : AbsDbProvider
    {
        public override DbProviderFactory GetDbProviderFactory => System.Data.SqlClient.SqlClientFactory.Instance;

        internal override ISqlBuilder CreateSqlBuilder(ExpressionBuilder expBuilder, string name)
        {
            return new SqlServerSqlBuilder(this, expBuilder, name);
        }
        public override bool IsSupportTransaction => true;
        public override string CreateDbConnstring(string userID, string passWord, string server, string catalog, string dataVer, int connectTimeout = 60, int poolMinSize = 16, int poolMaxSize = 100, string port = "")
        {
            var sb = new StringBuilder();
            if (string.IsNullOrWhiteSpace(userID) && string.IsNullOrWhiteSpace(passWord)) { sb.Append(string.Format("Pooling=true;Integrated Security=True;")); }
            else
            { sb.Append($"User ID={userID};Password={passWord};Pooling=true;"); }

            sb.Append($"Data Source={server};Initial Catalog={catalog};");

            if (poolMinSize > 0) { sb.Append($"Min Pool Size={poolMinSize};"); }
            if (poolMaxSize > 0) { sb.Append($"Max Pool Size={poolMaxSize};"); }
            if (connectTimeout > 0) { sb.Append($"Connect Timeout={connectTimeout};"); }
            return sb.ToString();
        }
    }
}