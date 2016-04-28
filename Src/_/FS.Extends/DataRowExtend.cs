using System;
using System.Data;
using FS.Cache;

// ReSharper disable once CheckNamespace

namespace FS.Extends
{
    public static partial class SqlExtend
    {
        /// <summary>
        ///     将DataRow转成实体类
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="dr">源DataRow</param>
        public static TEntity ToInfo<TEntity>(this DataRow dr) where TEntity : class, new()
        {
            var map = SetMapCacheManger.Cache(typeof (TEntity));
            var t = (TEntity) Activator.CreateInstance(typeof (TEntity));

            //赋值字段
            foreach (var kic in map.MapList)
            {
                if (dr.Table.Columns.Contains(kic.Value.Field.Name))
                {
                    if (!kic.Key.CanWrite) { continue; }
                    PropertySetCacheManger.Cache(kic.Key, t, dr[kic.Value.Field.Name].ConvertType(kic.Key.PropertyType));
                }
            }
            return t ?? new TEntity();
        }
    }
}