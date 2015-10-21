using System.Data.Common;
using System.Text;
using FS.Sql.Infrastructure;
using FS.Sql.Internal;

namespace FS.Sql.Client.SqLite
{
    /// <summary>
    ///     SqLite 数据库提供者（不同数据库的特性）
    /// </summary>
    public class SqLiteProvider : AbsDbProvider
    {
        public override DbProviderFactory GetDbProviderFactory => DbProviderFactories.GetFactory("System.Data.SQLite");

        internal override ISqlBuilder CreateSqlBuilder(ExpressionBuilder expBuilder, string name)
        {
            return new SqLiteSqlBuilder(this, expBuilder, name);
        }

        public override string CreateDbConnstring(string userID, string passWord, string server, string catalog, string dataVer, int connectTimeout = 60, int poolMinSize = 16, int poolMaxSize = 100, string port = "")
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Data Source={0};Min Pool Size={1};Max Pool Size={2};", GetFilePath(server), poolMinSize, poolMaxSize);
            if (!string.IsNullOrWhiteSpace(passWord)) { sb.AppendFormat("Password={0};", passWord); }
            if (!string.IsNullOrWhiteSpace(dataVer)) { sb.AppendFormat("Version={0};", dataVer); }
            return sb.ToString();
        }
    }
}