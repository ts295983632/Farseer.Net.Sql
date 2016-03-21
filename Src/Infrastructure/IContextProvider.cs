using FS.Sql.Map;

namespace FS.Sql.Infrastructure
{
    /// <summary>
    ///     数据库上下文
    /// </summary>
    internal interface IContextProvider
    {
        /// <summary>
        ///     主下文映射关系
        /// </summary>
        ContextDataMap ContextMap { get; }

        /// <summary>
        ///     数据库提供者（不同数据库的特性）
        /// </summary>
        AbsDbProvider DbProvider { get; }

        /// <summary>
        /// 执行数据库操作
        /// </summary>
        //IExecuteSql Executeor { get; }
    }
}