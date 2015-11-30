using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using FS.Infrastructure;
using FS.Log;
using FS.Sql.Infrastructure;

namespace FS.Sql.Data
{
    /// <summary>
    ///     数据库操作
    /// </summary>
    public sealed class DbExecutor : IDisposable
    {
        /// <summary>
        ///     构造函数
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="commandTimeout">数据库执行时间，单位秒</param>
        /// <param name="tranLevel">开启事务等级</param>
        public DbExecutor(string connectionString, eumDbType dbType = eumDbType.SqlServer, int commandTimeout = 30, IsolationLevel tranLevel = IsolationLevel.Unspecified)
        {
            _connectionString = connectionString;
            _commandTimeout = commandTimeout;
            DataType = dbType;

            OpenTran(tranLevel);
        }

        /// <summary>
        ///     数据库执行时间，单位秒
        /// </summary>
        private readonly int _commandTimeout;

        /// <summary>
        ///     连接字符串
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        ///     数据类型
        /// </summary>
        public readonly eumDbType DataType;

        /// <summary>
        ///     数据提供者
        /// </summary>
        private DbProviderFactory _factory;

        /// <summary>
        ///     是否开启事务
        /// </summary>
        internal bool IsTransaction { get; private set; }

        /// <summary>
        ///     Sql执行对像
        /// </summary>
        private DbCommand _comm;

        /// <summary>
        ///     数据库连接对像
        /// </summary>
        private DbConnection _conn;

        /// <summary>
        ///     开启事务。
        /// </summary>
        /// <param name="tranLevel">事务方式</param>
        public void OpenTran(IsolationLevel tranLevel)
        {
            if (tranLevel != IsolationLevel.Unspecified)
            {
                Open();
                _comm.Transaction = _conn.BeginTransaction(tranLevel);
                IsTransaction = true;
            }
        }

        /// <summary>
        ///     关闭事务。
        /// </summary>
        public void CloseTran()
        {
            if (IsTransaction) { _comm?.Transaction?.Dispose(); }
            IsTransaction = false;
        }

        /// <summary>
        ///     打开数据库连接
        /// </summary>
        private void Open()
        {
            if (_conn == null)
            {
                _factory = AbsDbProvider.CreateInstance(DataType).GetDbProviderFactory;
                _comm = _factory.CreateCommand();
                _comm.CommandTimeout = _commandTimeout;

                _conn = _factory.CreateConnection();
                _conn.ConnectionString = _connectionString;
                _comm.Connection = _conn;
            }
            if (_conn.State == ConnectionState.Closed)
            {
                _conn.Open();
                _comm.Parameters.Clear();
            }
        }

        /// <summary>
        ///     关闭数据库连接
        /// </summary>
        public void Close(bool dispose)
        {
            _comm?.Parameters.Clear();
            if ((dispose || _comm.Transaction == null) && _conn != null && _conn.State != ConnectionState.Closed)
            {
                _comm.Dispose();
                _comm = null;
                _conn.Close();
                _conn.Dispose();
                _conn = null;
            }
        }

        /// <summary>
        ///     提交事务
        ///     如果未开启事务则会引发异常
        /// </summary>
        public void Commit()
        {
            if (_comm.Transaction == null) { throw new Exception("未开启事务"); }
            _comm.Transaction.Commit();
        }

        /// <summary>
        ///     返回第一行第一列数据
        /// </summary>
        /// <param name="cmdType">执行方式</param>
        /// <param name="cmdText">SQL或者存储过程名称</param>
        /// <param name="parameters">参数</param>
        [SuppressMessage("Microsoft.Security", "CA2100:检查 SQL 查询是否存在安全漏洞")]
        public object ExecuteScalar(CommandType cmdType, string cmdText, params DbParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(cmdText)) { return null; }
            try
            {
                Open();
                _comm.CommandType = cmdType;
                _comm.CommandText = cmdText;
                if (parameters != null) { _comm.Parameters.AddRange(parameters); }

                return _comm.ExecuteScalar();
            }
            catch (Exception exp) { LogManger.Log.Error(exp); return null; }
            finally { Close(false); }
        }

        /// <summary>
        ///     返回受影响的行数
        /// </summary>
        /// <param name="cmdType">执行方式</param>
        /// <param name="cmdText">SQL或者存储过程名称</param>
        /// <param name="parameters">参数</param>
        [SuppressMessage("Microsoft.Security", "CA2100:检查 SQL 查询是否存在安全漏洞")]
        public int ExecuteNonQuery(CommandType cmdType, string cmdText, params DbParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(cmdText)) { return 0; }
            try
            {
                Open();
                _comm.CommandType = cmdType;
                _comm.CommandText = cmdText;
                if (parameters != null) { _comm.Parameters.AddRange(parameters); }

                return _comm.ExecuteNonQuery();
            }
            catch (Exception exp) { LogManger.Log.Error(exp); return 0; }
            finally { Close(false); }
        }

        /// <summary>
        ///     返回数据(IDataReader)
        /// </summary>
        /// <param name="cmdType">执行方式</param>
        /// <param name="cmdText">SQL或者存储过程名称</param>
        /// <param name="parameters">参数</param>
        [SuppressMessage("Microsoft.Security", "CA2100:检查 SQL 查询是否存在安全漏洞")]
        public IDataReader GetReader(CommandType cmdType, string cmdText, params DbParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(cmdText)) { return null; }
            try
            {
                Open();
                _comm.CommandType = cmdType;
                _comm.CommandText = cmdText;
                if (parameters != null) { _comm.Parameters.AddRange(parameters); }

                return IsTransaction ? _comm.ExecuteReader() : _comm.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception exp) { LogManger.Log.Error(exp); return null; }
        }

        /// <summary>
        ///     返回数据(DataSet)
        /// </summary>
        /// <param name="cmdType">执行方式</param>
        /// <param name="cmdText">SQL或者存储过程名称</param>
        /// <param name="parameters">参数</param>
        [SuppressMessage("Microsoft.Security", "CA2100:检查 SQL 查询是否存在安全漏洞")]
        public DataSet GetDataSet(CommandType cmdType, string cmdText, params DbParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(cmdText)) { return new DataSet(); }
            try
            {
                Open();
                _comm.CommandType = cmdType;
                _comm.CommandText = cmdText;
                if (parameters != null) { _comm.Parameters.AddRange(parameters); }
                var ada = _factory.CreateDataAdapter();
                ada.SelectCommand = _comm;
                var ds = new DataSet();
                ada.Fill(ds);
                return ds;
            }
            catch (Exception exp) { LogManger.Log.Error(exp); return null; }
            finally { Close(false); }
        }

        /// <summary>
        ///     返回数据(DataTable)
        /// </summary>
        /// <param name="cmdType">执行方式</param>
        /// <param name="cmdText">SQL或者存储过程名称</param>
        /// <param name="parameters">参数</param>
        public DataTable GetDataTable(CommandType cmdType, string cmdText, params DbParameter[] parameters)
        {
            var ds = GetDataSet(cmdType, cmdText, parameters);
            return ds.Tables.Count == 0 ? new DataTable() : ds.Tables[0];
        }

        /// <summary>
        ///     指量操作数据（仅支付Sql Server)
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="dt">数据</param>
        public void ExecuteSqlBulkCopy(string tableName, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) { return; }

            try
            {
                Open();
                using (var bulkCopy = new SqlBulkCopy((SqlConnection)_conn, SqlBulkCopyOptions.Default, (SqlTransaction)_comm.Transaction))
                {
                    bulkCopy.DestinationTableName = tableName;
                    bulkCopy.BatchSize = dt.Rows.Count;
                    bulkCopy.BulkCopyTimeout = 3600;
                    bulkCopy.WriteToServer(dt);
                }
            }
            catch (Exception exp) { LogManger.Log.Error(exp); }
            finally { Close(false); }
        }

        private void Dispose(bool disposing)
        {
            //释放托管资源
            if (disposing) { Close(true); }
        }

        /// <summary>
        ///     注销
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    /// <summary> 字段类型 </summary>
    public enum FieldType
    {
        /// <summary> 整型 </summary>
        [Display(Name = "Int")]
        Int,

        /// <summary> 布尔型 </summary>
        [Display(Name = "Bit")]
        Bit,

        /// <summary> 可变字符串 </summary>
        [Display(Name = "Varchar")]
        Varchar,

        /// <summary> 可变字符串（双字节） </summary>
        [Display(Name = "Nvarchar")]
        Nvarchar,

        /// <summary> 不可变字符串 </summary>
        [Display(Name = "Char")]
        Char,

        /// <summary> 不可变字符串（双字节） </summary>
        [Display(Name = "NChar")]
        NChar,

        /// <summary> 不可变文本 </summary>
        [Display(Name = "Text")]
        Text,

        /// <summary> 不可变文本 </summary>
        [Display(Name = "Ntext")]
        Ntext,

        /// <summary> 日期 </summary>
        [Display(Name = "DateTime")]
        DateTime,

        /// <summary> 短整型 </summary>
        [Display(Name = "Smallint")]
        Smallint,

        /// <summary> 浮点 </summary>
        [Display(Name = "Float")]
        Float,
    }
}