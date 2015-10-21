using System.Reflection;
using FS.Configs;
using FS.Sql.Infrastructure;

namespace FS.Sql
{
    public class SqlSet : AbsDbSet
    {
        /// <summary>
        ///     SQL语句配置文件
        /// </summary>
        protected SqlMap Map;

        /// <summary>
        ///     使用属性类型的创建
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="pInfo">属性类型</param>
        internal SqlSet(DbContext context, PropertyInfo pInfo)
        {
            SetContext(context, pInfo);
            Map = Context.ContextType.FullName + "." + SetMap.Name;
        }

        /// <summary>
        ///     返回查询的值
        /// </summary>
        /// <param name="t">失败时返回的值</param>
        public T GetValue<T>(T t = default(T))
        {
            return Context.ManualSql.GetValue(Map.Sql, t);
        }

        /// <summary>
        ///     执行存储过程
        /// </summary>
        public int Execute()
        {
            return Context.ManualSql.Execute(Map.Sql);
        }
    }
}