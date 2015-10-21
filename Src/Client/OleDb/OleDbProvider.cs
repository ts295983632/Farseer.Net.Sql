using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using FS.Sql.ExpressionVisitor;
using FS.Sql.Infrastructure;
using FS.Sql.Internal;
using FS.Sql.Map;

namespace FS.Sql.Client.OleDb
{
    /// <summary>
    ///     OleDb 数据库提供者（不同数据库的特性）
    /// </summary>
    public class OleDbProvider : AbsDbProvider
    {
        public override DbProviderFactory GetDbProviderFactory => DbProviderFactories.GetFactory("System.Data.OleDb");

        internal override ISqlBuilder CreateSqlBuilder(ExpressionBuilder expBuilder, string name)
        {
            return new OleDbSqlBuilder(this, expBuilder, name);
        }

        public override string CreateDbConnstring(string userID, string passWord, string server, string catalog, string dataVer, int connectTimeout = 60, int poolMinSize = 16, int poolMaxSize = 100, string port = "")
        {
            var sb = new StringBuilder();
            switch (dataVer)
            {
                case "3.0":
                {
                    sb.Append("Provider=Microsoft.Jet.OLEDB.4.0;");
                    break;
                } //Extended Properties=Excel 3.0;
                case "4.0":
                {
                    sb.Append("Provider=Microsoft.Jet.OLEDB.4.0;");
                    break;
                } //Extended Properties=Excel 4.0;
                case "5.0":
                {
                    sb.Append("Provider=Microsoft.Jet.OLEDB.4.0;");
                    break;
                } //Extended Properties=Excel 5.0;
                case "95":
                {
                    sb.Append("Provider=Microsoft.Jet.OLEDB.4.0;");
                    break;
                } //Extended Properties=Excel 5.0;
                case "97":
                {
                    sb.Append("Provider=Microsoft.Jet.OLEDB.3.51;");
                    break;
                }
                case "2003":
                {
                    sb.Append("Provider=Microsoft.Jet.OLEDB.4.0;");
                    break;
                } //Extended Properties=Excel 8.0;
                //  2007+   DR=YES
                default:
                {
                    sb.Append("Provider=Microsoft.ACE.OLEDB.12.0;");
                    break;
                } //Extended Properties=Excel 12.0;
            }
            sb.Append($"Data Source={GetFilePath(server)};");
            if (!string.IsNullOrWhiteSpace(userID)) { sb.Append($"User ID={userID};"); }
            if (!string.IsNullOrWhiteSpace(passWord)) { sb.Append($"Password={passWord};"); }

            return sb.ToString();
        }

        public override WhereVisitor CreateWhereVisitor(SetDataMap map, List<DbParameter> paramList) => new OleDbWhereVisitor(this, map, paramList);
    }
}