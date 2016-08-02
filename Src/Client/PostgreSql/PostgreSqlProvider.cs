using System.Data.Common;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using FS.Cache;
using FS.Sql.Client.MySql;
using FS.Sql.Infrastructure;
using FS.Sql.Internal;

namespace FS.Sql.Client.PostgreSql
{
    /// <summary>
    ///     MySql 数据库提供者（不同数据库的特性）
    /// </summary>
    public class PostgreSqlProvider : AbsDbProvider
    {
        public override DbProviderFactory DbProviderFactory => (DbProviderFactory)Assembly.Load("Npgsql").GetType("Npgsql.NpgsqlFactory").GetField("Instance").GetValue(null);//(DbProviderFactory)InstanceCacheManger.Cache(Assembly.Load("Npgsql").GetType("Npgsql.NpgsqlFactory"));
        public override AbsFunctionProvider FunctionProvider => new PostgreSqlFunctionProvider();
        public override bool IsSupportTransaction => true;
        public override string KeywordAegis(string fieldName)
        {
            if (Regex.IsMatch(fieldName, "[\\(\\)\\,\\[\\]\\+\\= ]*")) { return fieldName; }
            return $"\"{fieldName}\""; }

        internal override AbsSqlBuilder CreateSqlBuilder(ExpressionBuilder expBuilder, string name) => new PostgreSqlBuilder(this, expBuilder, name);

        public override string CreateDbConnstring(string userID, string passWord, string server, string catalog, string dataVer, string additional, int connectTimeout = 60, int poolMinSize = 16, int poolMaxSize = 100, string port = "")
        {
            var sb = new StringBuilder();
            sb.Append($"Server={server};User id={userID};password={passWord};Database={catalog};");
            if (!string.IsNullOrWhiteSpace(port)) { sb.Append($"Port={port};"); }
            sb.Append(additional);
            return sb.ToString();
        }
    }
}