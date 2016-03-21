using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using FS.Extends;
using FS.Sql.Internal;

namespace FS.Sql
{
    /// <summary>
    ///     手动编写SQL
    /// </summary>
    public class ManualSql
    {
        /// <summary>
        ///     手动编写SQL
        /// </summary>
        /// <param name="context">上下文</param>
        internal ManualSql(InternalContext context)
        {
            Context = context;
        }

        /// <summary>
        ///     数据库上下文
        /// </summary>
        private readonly InternalContext Context;

        /// <summary>
        ///     返回查询的值
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="t">失败时返回的值</param>
        public T GetValue<T>(string sql, T t = default(T), params DbParameter[] parameters)
        {
            return Context.QueueManger.Commit(null, (queue) => Context.Executeor.GetValue(new SqlParam(sql, parameters), t), false);
        }
        /// <summary>
        ///     返回查询的值
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="t">失败时返回的值</param>
        public Task<T> GetValueAsync<T>(string sql, T t = default(T), params DbParameter[] parameters)
        {
            return Task.Factory.StartNew(() => GetValue(sql,t, parameters));
        }

        /// <summary>
        ///     返回单条记录
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        public TEntity ToEntity<TEntity>(string sql, params DbParameter[] parameters) where TEntity : class, new()
        {
            return Context.QueueManger.Commit(null, (queue) => Context.Executeor.ToEntity<TEntity>(new SqlParam(sql, parameters)), false);
        }
        /// <summary>
        ///     返回单条记录
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        public Task<TEntity> ToEntityAsync<TEntity>(string sql, params DbParameter[] parameters) where TEntity : class, new()
        {
            return Task.Factory.StartNew(() => ToEntity<TEntity>(sql, parameters));
        }

        /// <summary>
        ///     返回多条记录
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        public List<TEntity> ToList<TEntity>(string sql, params DbParameter[] parameters) where TEntity : class, new()
        {
            return Context.QueueManger.Commit(null, (queue) => Context.Executeor.ToTable(new SqlParam(sql, parameters)).ToList<TEntity>(), false);
        }
        /// <summary>
        ///     返回多条记录
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        public Task<List<TEntity>> ToListAsync<TEntity>(string sql, params DbParameter[] parameters) where TEntity : class, new()
        {
            return Task.Factory.StartNew(() => ToList<TEntity>(sql, parameters));
        }

        /// <summary>
        ///     执行存储过程
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        public int Execute(string sql, params DbParameter[] parameters)
        {
            return Context.QueueManger.CommitLazy(null, (queue) => Context.Executeor.Execute(new SqlParam(sql, parameters)), false);
        }
        /// <summary>
        ///     执行存储过程
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        public Task<int> ExecuteAsync(string sql, params DbParameter[] parameters)
        {
            return Task.Factory.StartNew(() => Execute(sql, parameters));
        }
    }
}