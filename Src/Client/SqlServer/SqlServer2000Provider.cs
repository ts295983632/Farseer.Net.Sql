using FS.Sql.Infrastructure;
using FS.Sql.Internal;

namespace FS.Sql.Client.SqlServer
{
    /// <summary>
    ///     SqlServer 数据库提供者（不同数据库的特性）
    /// </summary>
    public class SqlServer2000Provider : SqlServerProvider
    {
        internal override ISqlBuilder CreateSqlBuilder(ExpressionBuilder expBuilder, string name)
        {
            return new SqlServerSqlBuilder2000(this, expBuilder, name);
        }
    }
}