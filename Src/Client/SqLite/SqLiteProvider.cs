using System;
using System.Data.Common;
using System.Reflection;
using System.Text;
using FS.Cache;
using FS.Sql.Infrastructure;
using FS.Sql.Internal;

namespace FS.Sql.Client.SqLite
{
    /// <summary>
    ///     SqLite 数据库提供者（不同数据库的特性）
    /// </summary>
    public class SqLiteProvider : AbsDbProvider
    {
        //public override DbProviderFactory GetDbProviderFactory => DbProviderFactories.GetFactory("System.Data.SQLite");
        public override DbProviderFactory DbProviderFactory =>  (DbProviderFactory)  InstanceCacheManger.Cache(Assembly.Load("System.Data.SQLite").GetType("System.Data.SQLite.SQLiteFactory"));
        public override AbsFunctionProvider FunctionProvider => new SqLiteFunctionProvider();
        internal override AbsSqlBuilder CreateSqlBuilder(ExpressionBuilder expBuilder, string name)=> new SqLiteBuilder(this, expBuilder, name);
        public override bool IsSupportTransaction => true;
        public override string CreateDbConnstring(string userID, string passWord, string server, string catalog, string dataVer, string additional, int connectTimeout = 60, int poolMinSize = 16, int poolMaxSize = 100, string port = "")
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Data Source={0};Min Pool Size={1};Max Pool Size={2};", GetFilePath(server), poolMinSize, poolMaxSize);
            if (!string.IsNullOrWhiteSpace(passWord)) { sb.AppendFormat("Password={0};", passWord); }
            if (!string.IsNullOrWhiteSpace(dataVer)) { sb.AppendFormat("Version={0};", dataVer); }
            sb.Append(additional);
            return sb.ToString();
        }
    }
}