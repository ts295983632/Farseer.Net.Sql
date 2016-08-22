using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using FS.Cache;
using FS.Utils.Common;
using System.Reflection;

// ReSharper disable once CheckNamespace

namespace FS.Extends
{
    public static partial class SqlExtend
    {
        /// <summary>
        ///     数据填充
        /// </summary>
        /// <param name="reader">源IDataReader</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        public static TEntity ToEntity<TEntity>(this IDataReader reader) where TEntity : class, new()
        {
            var map = SetMapCacheManger.Cache(typeof(TEntity));

            var t = new TEntity();
            var isHaveValue = false;

            if (reader.Read())
            {
                //赋值字段
                foreach (var kic in map.MapList)
                {
                    var filedName = kic.Value.Field.IsFun ? kic.Key.Name : kic.Value.Field.Name;
                    if (HaveName(reader, filedName))
                    {
                        if (!kic.Key.CanWrite) { continue; }
                        var oVal = ConvertHelper.ConvertType(reader[filedName], kic.Key.PropertyType);
                        if (oVal == null) { continue; } //  当值类型，目标值为null时，需要做默认值处理
                        PropertySetCacheManger.Cache(kic.Key, t, oVal);
                        isHaveValue = true;
                    }
                }
            }
            reader.Close();
            return isHaveValue ? t : null;
        }

        /// <summary>
        ///     将集合类转换成DataTable
        /// </summary>
        /// <param name="lst">集合</param>
        /// <returns></returns>
        public static DataTable ToTable<TEntity>(this List<TEntity> lst) where TEntity : class
        {
            var dt = new DataTable();
            if (lst.Count == 0) { return dt; }
            var map = SetMapCacheManger.Cache(typeof(TEntity));
            var lstFields = map.MapList.Where(o => o.Value.Field.IsMap).ToList();
            foreach (var field in lstFields)
            {
                var type = field.Key.PropertyType;
                // 对   List 类型处理
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) { type = type.GetGenericArguments()[0]; }
                dt.Columns.Add(field.Value.Field.Name, type);
            }

            foreach (var info in lst)
            {
                dt.Rows.Add(dt.NewRow());
                foreach (var field in lstFields)
                {
                    var value = ConvertHelper.GetValue(info, field.Key.Name, (object)null);
                    if (value == null) { continue; }
                    if (!dt.Columns.Contains(field.Value.Field.Name)) { dt.Columns.Add(field.Value.Field.Name); }
                    dt.Rows[dt.Rows.Count - 1][field.Value.Field.Name] = value;
                }
            }
            return dt;
        }

        /// <summary>
        ///     判断IDataReader是否存在某列
        /// </summary>
        public static bool HaveName(this IDataReader reader, string name)
        {
            for (var i = 0; i < reader.FieldCount; i++) { if (ConvertHelper.IsEquals(reader.GetName(i), name)) { return true; } }
            return false;
        }

        /// <summary>
        ///     DataTable转换为List实体类
        /// </summary>
        /// <param name="dt">源DataTable</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        public static List<TEntity> ToList<TEntity>(this DataTable dt) where TEntity : class, new()
        {
            var list = new List<TEntity>(dt.Rows.Count);
            foreach (DataRow dr in dt.Rows)
            {
                list.Add(dr.ToInfo<TEntity>());
            }
            return list;
        }

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
                    var oVal = dr[filedName].ConvertType(kic.Key.PropertyType);
                    if (oVal == null) { continue; }
                    PropertySetCacheManger.Cache(kic.Key, t, oVal);
                }
            }
            return t;
        }

        /// <summary>
        ///     DataTable转换为List实体类
        /// </summary>
        /// <param name="dt">源DataTable</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        public static List<TEntity> ToListForCodeDom<TEntity>(this DataTable dt) where TEntity : class, new()
        {
            var list = new List<TEntity>(dt.Rows.Count);
            // 找到TEntity的派生类
            // 生成TEntity的派生类
            // 实例化并传入dr
            foreach (DataRow dr in dt.Rows)
            {
                list.Add(dr.ToInfo<TEntity>());
            }
            return list;
        }

        /// <summary>
        ///     DataTable转换为数组实体类
        /// </summary>
        /// <param name="dt">源DataTable</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        public static TEntity[] ToArray<TEntity>(this DataTable dt) where TEntity : class, new()
        {
            var arr = new TEntity[dt.Rows.Count];
            var map = SetMapCacheManger.Cache(typeof(TEntity));
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                arr[i] = dt.Rows[i].ToInfo<TEntity>();
            }
            return arr;
        }

        /// <summary>
        ///     IDataReader转换为实体类
        /// </summary>
        /// <param name="ds">源DataSet</param>
        /// <typeparam name="T">实体类</typeparam>
        public static List<T> ToList<T>(this DataSet ds) where T : class, new()
        {
            return ds.Tables.Count == 0 ? null : ds.Tables[0].ToList<T>();
        }

        /// <summary>
        ///     将XML转成实体
        /// </summary>
        public static List<TEntity> ToList<TEntity>(this XElement element) where TEntity : class
        {
            var orm = SetMapCacheManger.Cache(typeof(TEntity));
            var list = new List<TEntity>();

            foreach (var el in element.Elements())
            {
                var t = (TEntity)Activator.CreateInstance(typeof(TEntity));

                //赋值字段
                foreach (var kic in orm.MapList)
                {
                    var type = kic.Key.PropertyType;
                    if (!kic.Key.CanWrite) { continue; }
                    //switch (kic.Value.PropertyExtend)
                    {
                        //case eumPropertyExtend.Attribute:
                        //    if (el.Attribute(kic.Value.Name) == null) { continue; }
                        //kic.Key.SetValue(t, el.Attribute(kic.Value.Name).Value.ConvertType(type), null);
                        //    PropertySetCacheManger.Cache(kic.Key, t, el.Attribute(kic.Value.Name).Value.ConvertType(type));

                        //    break;
                        // case eumPropertyExtend.Element:
                        if (el.Element(kic.Value.Field.Name) == null) { continue; }
                        //kic.Key.SetValue(t, el.Element(kic.Value.Name).Value.ConvertType(type), null);
                        PropertySetCacheManger.Cache(kic.Key, t, el.Element(kic.Value.Field.Name).Value.ConvertType(type));
                        break;
                    }
                }
                list.Add(t);
            }
            return list;
        }
    }
}