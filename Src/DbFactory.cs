using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using FS.Cache;
using FS.Configs;
using FS.Infrastructure;
using FS.Sql.Data;

namespace FS.Sql
{
    public static class DbFactory
    {
        /// <summary>
        ///     返回数据库执行
        /// </summary>
        /// <param name="dbIndex">数据库配置项</param>
        /// <param name="tranLevel">开启事务等级</param>
        public static DbExecutor CreateDbExecutor(int dbIndex = 0, IsolationLevel tranLevel = IsolationLevel.ReadCommitted)
        {
            DbInfo dbInfo = dbIndex;
            return new DbExecutor(ConnStringCacheManger.Cache(dbIndex), dbInfo.DataType, dbInfo.CommandTimeout, tranLevel);
        }

        /// <summary>
        ///     返回数据库类型名称
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        public static DbProviderFactory CreateDbProviderFactory(eumDbType dbType)
        {
            switch (dbType)
            {
                case eumDbType.MySql:
                    return DbProviderFactories.GetFactory("MySql.Data.MySqlClient");
                case eumDbType.OleDb:
                    return DbProviderFactories.GetFactory("System.Data.OleDb");
                case eumDbType.Oracle:
                    return DbProviderFactories.GetFactory("System.Data.OracleClient");
                case eumDbType.SQLite:
                    return DbProviderFactories.GetFactory("System.Data.SQLite");
                case eumDbType.SqlServer:
                    return DbProviderFactories.GetFactory("System.Data.SqlClient");
            }
            return DbProviderFactories.GetFactory("System.Data.SqlClient");
        }

        /// <summary>
        ///     获取数据库连接对象
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <param name="connectionString">连接字符串</param>
        public static DbConnection GetDbConnection(eumDbType dbType, string connectionString)
        {
            DbConnection conn;
            switch (dbType)
            {
                case eumDbType.SqlServer:
                    conn = new SqlConnection(connectionString);
                    ;
                    break;
                default:
                    conn = CreateDbProviderFactory(dbType).CreateConnection();
                    break;
            }
            conn.ConnectionString = connectionString;
            return conn;
        }

        /// <summary>
        ///     压缩数据库
        /// </summary>
        /// <param name="dataType">数据库类型</param>
        /// <param name="connetionString">连接字符串</param>
        public static void Compression(string connetionString, eumDbType dataType = eumDbType.SqlServer)
        {
            var db = new DbExecutor(connetionString, dataType, 30);
            switch (dataType)
            {
                case eumDbType.SQLite:
                    db.ExecuteNonQuery(CommandType.Text, "VACUUM", null);
                    break;

                default:
                    throw new NotImplementedException("该数据库不支持该方法！");
            }
        }

        /// <summary>
        ///     压缩数据库
        /// </summary>
        /// <param name="dbIndex">数据库配置</param>
        public static void Compression(int dbIndex)
        {
            DbInfo dbInfo = dbIndex;
            Compression(ConnStringCacheManger.Cache(dbIndex), dbInfo.DataType);
        }
    }
}