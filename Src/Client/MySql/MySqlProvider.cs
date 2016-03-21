using System.Data.Common;
using System.Reflection;
using FS.Cache;
using FS.Sql.Infrastructure;
using FS.Sql.Internal;

namespace FS.Sql.Client.MySql
{
    /// <summary>
    ///     MySql 数据库提供者（不同数据库的特性）
    /// </summary>
    public class MySqlProvider : AbsDbProvider
    {
        public override DbProviderFactory DbProviderFactory => (DbProviderFactory)InstanceCacheManger.Cache(Assembly.Load("MySql.Data.MySqlClient").GetType("MySql.Data.MySqlClient.MySqlClientFactory"));
        public override AbsFunctionProvider FunctionProvider => new MySqlFunctionProvider();
        public override bool IsSupportTransaction => true;
        public override string KeywordAegis(string fieldName) => $"`{fieldName}`";
        internal override AbsSqlBuilder CreateSqlBuilder(ExpressionBuilder expBuilder, string name) => new MySqlBuilder(this, expBuilder, name);

        public override string CreateDbConnstring(string userID, string passWord, string server, string catalog, string dataVer, string additional, int connectTimeout = 60, int poolMinSize = 16, int poolMaxSize = 100, string port = "")
        {
            // 2016年1月13日
            // 感谢：QQ21995346 ★Master★ 同学，发现了BUG
            // 场景：连接连字符串，被强制指定了：charset='gbk'
            // 解决：移除charset='gbk'，并在DbConfig配置中增加自定义连接方式
            return $"Data Source='{server}';User Id='{userID}';Password='{passWord}';Database='{catalog}';{additional}";
        }
    }
}