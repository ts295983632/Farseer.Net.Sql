using System.Collections.Generic;
using System.Data;
using FS.Extends;
using FS.Sql.Data;
using FS.Sql.Infrastructure;
using FS.Utils.Common;

namespace FS.Sql.Internal
{
    /// <summary> 将SQL发送到数据库 </summary>
    internal sealed class ExecuteSql : IExecuteSql
    {
        /// <summary>
        ///     将SQL发送到数据库
        /// </summary>
        /// <param name="dataBase">数据库操作</param>
        /// <param name="contextProvider">数据库上下文</param>
        internal ExecuteSql(DbExecutor dataBase, InternalContext contextProvider)
        {
            DataBase = dataBase;
            _contextProvider = contextProvider;
        }

        /// <summary>
        ///     数据库操作
        /// </summary>
        public DbExecutor DataBase { get; }

        /// <summary>
        ///     数据库上下文
        /// </summary>
        private readonly InternalContext _contextProvider;

        /// <summary>
        ///     返回影响行数
        /// </summary>
        /// <param name="sqlParam">SQL语句与参数</param>
        public int Execute(ISqlParam sqlParam)
        {
            var param = sqlParam.Param?.ToArray();
            return sqlParam.Sql.Length < 1 ? 0 : DataBase.ExecuteNonQuery(CommandType.Text, sqlParam.Sql.ToString(), param);
        }

        /// <summary>
        ///     返回影响行数
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="procBuilder">SQL语句与参数</param>
        /// <param name="entity">实体类</param>
        public int Execute<TEntity>(ProcBuilder procBuilder, TEntity entity) where TEntity : class, new()
        {
            // 生成SQL 输入、输出参数化
            var sqlParam = procBuilder.InitParam(entity);
            var param = sqlParam.Param?.ToArray();
            var value = DataBase.ExecuteNonQuery(CommandType.StoredProcedure, sqlParam.Name, param);
            procBuilder.SetParamToEntity(entity);
            return value;
        }

        /// <summary>
        ///     返回DataTable
        /// </summary>
        /// <param name="sqlParam">SQL语句与参数</param>
        public DataTable ToTable(ISqlParam sqlParam)
        {
            var param = sqlParam.Param?.ToArray();
            return DataBase.GetDataTable(CommandType.Text, sqlParam.Sql.ToString(), param);
        }

        /// <summary>
        ///     返回影响行数
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="procBuilder">SQL语句与参数</param>
        /// <param name="entity">实体类</param>
        public DataTable ToTable<TEntity>(ProcBuilder procBuilder, TEntity entity) where TEntity : class, new()
        {
            // 生成SQL 输入、输出参数化
            var sqlParam = procBuilder.InitParam(entity);
            var param = sqlParam.Param?.ToArray();
            var value = DataBase.GetDataTable(CommandType.StoredProcedure, sqlParam.Name, param);
            procBuilder.SetParamToEntity(entity);
            return value;
        }

        public List<TEntity> ToList<TEntity>(ISqlParam sqlParam) where TEntity : class, new()
        {
            var param = sqlParam.Param?.ToArray();
            return SqlExtend.ToList<TEntity>(DataBase.GetReader(CommandType.Text, sqlParam.Sql.ToString(), param));
        }
        public List<TEntity> ToList<TEntity>(ProcBuilder procBuilder, TEntity entity) where TEntity : class, new()
        {
            // 生成SQL 输入、输出参数化
            var sqlParam = procBuilder.InitParam(entity);
            var param = sqlParam.Param?.ToArray();
            var value = SqlExtend.ToList<TEntity>(DataBase.GetReader(CommandType.StoredProcedure, sqlParam.Name, param));
            procBuilder.SetParamToEntity(entity);
            return value;
        }

        /// <summary>
        ///     返回单条数据
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="sqlParam">SQL语句与参数</param>
        public TEntity ToEntity<TEntity>(ISqlParam sqlParam) where TEntity : class, new()
        {
            var param = sqlParam.Param?.ToArray();
            TEntity t;
            using (var reader = DataBase.GetReader(CommandType.Text, sqlParam.Sql.ToString(), param)) { t = reader.ToEntity<TEntity>(); }
            DataBase.Close(false);
            return t;
        }

        /// <summary>
        ///     返回影响行数
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="procBuilder">SQL语句与参数</param>
        /// <param name="entity">实体类</param>
        public TEntity ToEntity<TEntity>(ProcBuilder procBuilder, TEntity entity) where TEntity : class, new()
        {
            // 生成SQL 输入、输出参数化
            var sqlParam = procBuilder.InitParam(entity);
            var param = sqlParam.Param?.ToArray();
            TEntity t;
            using (var reader = DataBase.GetReader(CommandType.StoredProcedure, sqlParam.Name, param)) { t = reader.ToEntity<TEntity>(); }
            DataBase.Close(false);
            procBuilder.SetParamToEntity(entity);
            return t;
        }


        /// <summary>
        ///     查询单个字段值
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="sqlParam">SQL语句与参数</param>
        /// <param name="defValue">默认值</param>
        public T GetValue<T>(ISqlParam sqlParam, T defValue = default(T))
        {
            var param = sqlParam.Param?.ToArray();
            var value = DataBase.ExecuteScalar(CommandType.Text, sqlParam.Sql.ToString(), param);
            return ConvertHelper.ConvertType(value, defValue);
        }

        /// <summary>
        ///     查询单个字段值
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="procBuilder">存储过程生成器</param>
        /// <param name="entity">实体</param>
        /// <param name="defValue">默认值</param>
        public T GetValue<TEntity, T>(ProcBuilder procBuilder, TEntity entity, T defValue = default(T)) where TEntity : class, new()
        {
            // 生成SQL 输入、输出参数化
            var sqlParam = procBuilder.InitParam(entity);
            var param = sqlParam.Param?.ToArray();
            var value = DataBase.ExecuteScalar(CommandType.StoredProcedure, sqlParam.Name, param);
            procBuilder.SetParamToEntity(entity);
            return ConvertHelper.ConvertType(value, defValue);
        }
    }
}