using FS.Configs;
using FS.Sql.Infrastructure;

// ReSharper disable once CheckNamespace

namespace FS.Cache
{
    /// <summary>
    ///     数据库连接字符串缓存
    /// </summary>
    public class ConnStringCacheManger : AbsCacheManger<int, string>
    {
        /// <summary>
        ///     线程锁
        /// </summary>
        private static readonly object LockObject = new object();

        private ConnStringCacheManger(int key) : base(key)
        {
        }

        protected override string SetCacheLock()
        {
            lock (LockObject)
            {
                if (CacheList.ContainsKey(Key)) return CacheList[Key];

                DbInfo dbInfo = Key;
                if (dbInfo == null) { Log.LogManger.Log.Error("未设置数据库配置文件"); return null; }
                CacheList.Add(Key, AbsDbProvider.CreateInstance(dbInfo.DataType, dbInfo.DataVer).CreateDbConnstring(dbInfo.UserID, dbInfo.PassWord, dbInfo.Server, dbInfo.Catalog, dbInfo.DataVer, dbInfo.Additional, dbInfo.ConnectTimeout, dbInfo.PoolMinSize, dbInfo.PoolMaxSize, dbInfo.Port));
            }

            return CacheList[Key];
        }

        /// <summary>
        ///     获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        public static string Cache(int key)
        {
            return new ConnStringCacheManger(key).GetValue();
        }
    }
}