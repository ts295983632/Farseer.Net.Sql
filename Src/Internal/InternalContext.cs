using System;
using System.ComponentModel;
using System.Data;
using FS.Configs;
using FS.Sql.Data;
using FS.Sql.Infrastructure;
using FS.Sql.Map;

namespace FS.Sql.Internal
{
    /// <summary>
    ///     数据库上下文初始化程序
    /// </summary>
    internal class InternalContext : IDisposable
    {
        /// <summary>
        ///     上下文初始化器（只赋值，不初始化，有可能被重复创建两次）
        /// </summary>
        /// <param name="contextType">外部上下文类型</param>
        /// <param name="contextConnection">上下文数据库连接信息</param>
        public InternalContext(Type contextType, ContextConnection contextConnection)
        {
            this.ContextType = contextType;
            this._contextConnection = contextConnection;
            this.IsMergeCommand = true;
        }

        /// <summary>
        ///     初始化数据库环境（共享自其它上下文）、实例化子类中，所有Set属性
        /// </summary>
        /// <param name="currentContextType">外部上下文类型</param>
        /// <param name="masterContext">其它上下文（主上下文）</param>
        public InternalContext(Type currentContextType, InternalContext masterContext)
        {
            this.ContextType = currentContextType;
            this._contextConnection = masterContext._contextConnection;

            // 上下文映射关系
            ContextMap = new ContextDataMap(ContextType);
            // 手动编写SQL
            ManualSql = masterContext.ManualSql;
            // 默认SQL执行者
            Executeor = masterContext.Executeor;
            // 数据库提供者
            DbProvider = masterContext.DbProvider;
            // 队列管理者
            QueueManger = masterContext.QueueManger;

            IsInitializer = true;
        }

        /// <summary>
        ///     上下文数据库连接信息
        /// </summary>
        private readonly ContextConnection _contextConnection;

        /// <summary>
        ///     外部上下文类型
        /// </summary>
        public Type ContextType { get; }

        /// <summary>
        ///     是否初始化
        /// </summary>
        public bool IsInitializer { get; private set; }

        /// <summary>
        ///     数据库提供者（不同数据库的特性）
        /// </summary>
        public AbsDbProvider DbProvider { get; private set; }

        /// <summary>
        ///     执行数据库操作
        /// </summary>
        public IExecuteSql Executeor { get; private set; }

        /// <summary>
        ///     映射结构关系
        /// </summary>
        public ContextDataMap ContextMap { get; private set; }

        /// <summary>
        ///     队列管理
        /// </summary>
        public QueueManger QueueManger { get; private set; }

        /// <summary>
        ///     true:启用合并执行命令、并延迟加载
        /// </summary>
        public bool IsMergeCommand { get; internal set; }

        /// <summary>
        ///     手动编写SQL
        /// </summary>
        public ManualSql ManualSql { get; internal set; }

        /// <summary>
        ///     初始化数据库环境、实例化子类中，所有Set属性
        /// </summary>
        public void Initializer()
        {
            if (IsInitializer) { return; }

            // 数据库提供者
            DbProvider = AbsDbProvider.CreateInstance(_contextConnection.DbType, _contextConnection.DataVer);
            // 默认SQL执行者
            Executeor = new ExecuteSql(new DbExecutor(_contextConnection.ConnectionString, _contextConnection.DbType, _contextConnection.CommandTimeout, IsMergeCommand && DbProvider.IsSupportTransaction ? IsolationLevel.Serializable : IsolationLevel.Unspecified), this);
            // 代理SQL记录
            if (SystemConfigs.ConfigEntity.IsWriteDbLog) { Executeor = new ExecuteSqlLogProxy(Executeor); }
            // 代理异常记录
            if (SystemConfigs.ConfigEntity.IsWriteDbExceptionLog) { Executeor = new ExecuteSqlExceptionLogProxy(Executeor); }

            // 队列管理者
            QueueManger = new QueueManger(this);
            // 手动编写SQL
            ManualSql = new ManualSql(this);
            // 上下文映射关系
            this.ContextMap = new ContextDataMap(ContextType);

            IsInitializer = true;
        }

        /// <summary>
        ///     释放资源
        /// </summary>
        /// <param name="disposing">是否释放托管资源</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual void Dispose(bool disposing)
        {
            //释放托管资源
            if (disposing)
            {
                QueueManger.Dispose();
                Executeor.DataBase.Dispose();
            }
        }

        /// <summary>
        ///     释放资源
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}