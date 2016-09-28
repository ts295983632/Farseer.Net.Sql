using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using FS.Log;
using FS.Log.Entity;
using FS.Sql.Data;
using FS.Sql.Infrastructure;

namespace FS.Sql.Internal
{
    /// <summary>
    ///     数据库操作（支持异常SQL日志记录）
    /// </summary>
    internal sealed class ExecuteSqlExceptionLogProxy : IExecuteSql
    {
        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="db">数据库执行者</param>
        internal ExecuteSqlExceptionLogProxy(IExecuteSql db)
        {
            _dbExecutor = db;
        }

        private readonly IExecuteSql _dbExecutor;

        public DbExecutor DataBase => _dbExecutor.DataBase;

        public int Execute(ISqlParam sqlParam)
        {
            try { return _dbExecutor.Execute(sqlParam); }
            catch (Exception ex)
            {
                WriteException(ex, sqlParam);
                throw;
            }
        }

        public int Execute<TEntity>(ProcBuilder procBuilder, TEntity entity) where TEntity : class, new()
        {
            try { return _dbExecutor.Execute(procBuilder, entity); }
            catch (Exception ex)
            {
                WriteException(ex, procBuilder);
                throw;
            }
        }

        public DataTable ToTable(ISqlParam sqlParam)
        {
            try { return _dbExecutor.ToTable(sqlParam); }
            catch (Exception ex)
            {
                WriteException(ex, sqlParam);
                throw;
            }
        }

        public DataTable ToTable<TEntity>(ProcBuilder procBuilder, TEntity entity) where TEntity : class, new()
        {
            try { return _dbExecutor.ToTable(procBuilder, entity); }
            catch (Exception ex)
            {
                WriteException(ex, procBuilder);
                throw;
            }
        }

        public List<TEntity> ToList<TEntity>(ISqlParam sqlParam) where TEntity : class, new()
        {
            try { return _dbExecutor.ToList<TEntity>(sqlParam); }
            catch (Exception ex)
            {
                WriteException(ex, sqlParam);
                throw;
            }
        }
        public List<TEntity> ToList<TEntity>(ProcBuilder procBuilder, TEntity entity) where TEntity : class, new()
        {
            try { return _dbExecutor.ToList(procBuilder, entity); }
            catch (Exception ex)
            {
                WriteException(ex, procBuilder);
                throw;
            }
        }

        public TEntity ToEntity<TEntity>(ISqlParam sqlParam) where TEntity : class, new()
        {
            try { return _dbExecutor.ToEntity<TEntity>(sqlParam); }
            catch (Exception ex)
            {
                WriteException(ex, sqlParam);
                throw;
            }
        }
        public TEntity ToEntity<TEntity>(ProcBuilder procBuilder, TEntity entity) where TEntity : class, new()
        {
            try { return _dbExecutor.ToEntity<TEntity>(procBuilder, entity); }
            catch (Exception ex)
            {
                WriteException(ex, procBuilder);
                throw;
            }
        }

        public T GetValue<T>(ISqlParam sqlParam, T defValue = default(T))
        {
            try { return _dbExecutor.GetValue(sqlParam, defValue); }
            catch (Exception ex)
            {
                WriteException(ex, sqlParam);
                throw;
            }
        }
        public T GetValue<TEntity, T>(ProcBuilder procBuilder, TEntity entity, T defValue = default(T)) where TEntity : class, new()
        {
            try { return _dbExecutor.GetValue(procBuilder, entity, defValue); }
            catch (Exception ex)
            {
                WriteException(ex, procBuilder);
                throw;
            }
        }

        /// <summary> 写入日志 </summary>
        private void WriteException(Exception ex, ISqlParam sqlParam)
        {
            new SqlErrorLog(ex, sqlParam.Name, CommandType.Text, sqlParam.Sql.ToString(), sqlParam.Param ?? new List<DbParameter>()).AddToQueue();
        }
        
        /// <summary> 写入日志 </summary>
        private void WriteException(Exception ex, ProcBuilder procBuilder)
        {
            new SqlErrorLog(ex, procBuilder.Name, CommandType.StoredProcedure, "", procBuilder.Param ?? new List<DbParameter>()).AddToQueue();
        }
    }
}