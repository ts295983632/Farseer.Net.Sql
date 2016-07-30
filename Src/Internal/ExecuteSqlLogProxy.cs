using System;
using System.Data;
using System.Diagnostics;
using FS.Log;
using FS.Log.Entity;
using FS.Sql.Data;
using FS.Sql.Infrastructure;

namespace FS.Sql.Internal
{
    /// <summary> 将SQL发送到数据库（代理类、记录SQL、执行时间） </summary>
    internal sealed class ExecuteSqlLogProxy : IExecuteSql
    {
        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="db">数据库执行者</param>
        internal ExecuteSqlLogProxy(IExecuteSql db)
        {
            _dbExecutor = db;
        }

        private readonly IExecuteSql _dbExecutor;
        public DbExecutor DataBase => _dbExecutor.DataBase;

        /// <summary>
        ///     计算执行时间
        /// </summary>
        private TReturn SpeedTest<TReturn>(ISqlParam sqlParam, Func<TReturn> func)
        {
            //Console.WriteLine($"sql:{sqlParam.Sql.ToString()}   param{sqlParam.Param}、\r\n\r\n");
            var timer = new Stopwatch();
            timer.Start();
            var val = func();
            timer.Stop();
            new SqlRunLogEntity(sqlParam.Name, CommandType.Text, sqlParam.Sql.ToString(), sqlParam.Param, timer.ElapsedMilliseconds).AddToQueue();
            return val;
        }

        /// <summary>
        ///     计算执行时间
        /// </summary>
        private TReturn SpeedTest<TReturn>(ProcBuilder procBuilder, Func<TReturn> func)
        {
            var timer = new Stopwatch();
            timer.Start();
            var val = func();
            timer.Stop();

            new SqlRunLogEntity(procBuilder.Name, CommandType.StoredProcedure, "", procBuilder.Param, timer.ElapsedMilliseconds).AddToQueue();
            return val;
        }

        public int Execute(ISqlParam sqlParam)
        {
            return SpeedTest(sqlParam, () => _dbExecutor.Execute(sqlParam));
        }

        public int Execute<TEntity>(ProcBuilder procBuilder, TEntity entity) where TEntity : class, new()
        {
            return SpeedTest(procBuilder, () => _dbExecutor.Execute(procBuilder, entity));
        }

        public DataTable ToTable(ISqlParam sqlParam)
        {
            return SpeedTest(sqlParam, () => _dbExecutor.ToTable(sqlParam));
        }

        public DataTable ToTable<TEntity>(ProcBuilder procBuilder, TEntity entity) where TEntity : class, new()
        {
            return SpeedTest(procBuilder, () => _dbExecutor.ToTable(procBuilder, entity));
        }

        TEntity IExecuteSql.ToEntity<TEntity>(ISqlParam sqlParam)
        {
            return SpeedTest(sqlParam, () => _dbExecutor.ToEntity<TEntity>(sqlParam));
        }

        public TEntity ToEntity<TEntity>(ProcBuilder procBuilder, TEntity entity) where TEntity : class, new()
        {
            return SpeedTest(procBuilder, () => _dbExecutor.ToEntity(procBuilder, entity));
        }

        public T GetValue<T>(ISqlParam sqlParam, T defValue = default(T))
        {
            return SpeedTest(sqlParam, () => _dbExecutor.GetValue(sqlParam, defValue));
        }

        public T GetValue<TEntity, T>(ProcBuilder procBuilder, TEntity entity, T defValue = default(T)) where TEntity : class, new()
        {
            return SpeedTest(procBuilder, () => _dbExecutor.GetValue(procBuilder, entity, defValue));
        }
    }
}