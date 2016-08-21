﻿using System;
using System.Data;
using FS.Cache;
using FS.Utils.Common;

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
            var map = SetMapCacheManger.Cache(typeof(TEntity));
            var t = new TEntity();

            //赋值字段
            foreach (var kic in map.MapList)
            {
                if (!kic.Key.CanWrite) { continue; }
                var filedName = kic.Value.Field.IsFun ? kic.Key.Name : kic.Value.Field.Name;
                if (dr.Table.Columns.Contains(filedName))
                {
                    var oVal = ConvertHelper.ConvertType(dr[filedName], kic.Key.PropertyType);
                    if (oVal == null) { continue; }
                    PropertySetCacheManger.Cache(kic.Key, t, oVal);
                }
            }
            return t ?? new TEntity();
        }
    }
}