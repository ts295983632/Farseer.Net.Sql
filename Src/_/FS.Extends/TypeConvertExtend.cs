using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using FS.Cache;
using FS.Utils.Common;
using System.Reflection;
using FS.Sql.Internal;

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
        public static List<TEntity> ToList<TEntity>(this IDataReader reader)
        {
            var mapData = ConvertHelper.DataReaderToDictionary(reader);
            var type = new EntityDynamics().GetEntityType<TEntity>();
            return (List<TEntity>)InstanceStaticCacheManger.Cache(type, "ToList", (object)mapData);
        }

        /// <summary>
        ///     数据填充
        /// </summary>
        /// <param name="reader">源IDataReader</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        public static TEntity ToEntity<TEntity>(this IDataReader reader)
        {
            var mapData = ConvertHelper.DataReaderToDictionary(reader);
            var type = new EntityDynamics().GetEntityType<TEntity>();
            return (TEntity)InstanceStaticCacheManger.Cache(type, "ToEntity", (object)mapData, 0);
        }

        /// <summary>
        ///     DataTable转换为List实体类
        /// </summary>
        /// <param name="dt">源DataTable</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        public static List<TEntity> ToList<TEntity>(this DataTable dt)
        {
            var mapData = ConvertHelper.DataTableToDictionary(dt);
            var type = new EntityDynamics().GetEntityType<TEntity>();
            return (List<TEntity>)InstanceStaticCacheManger.Cache(type, "ToList", (object)mapData);
        }

        /// <summary>
        ///     DataTable转换为数组实体类
        /// </summary>
        /// <param name="dt">源DataTable</param>
        /// <typeparam name="TEntity">实体类</typeparam>
        public static TEntity[] ToArray<TEntity>(this DataTable dt)
        {
            var mapData = ConvertHelper.DataTableToDictionary(dt);
            var type = new EntityDynamics().GetEntityType<TEntity>();
            return ((List<TEntity>)InstanceStaticCacheManger.Cache(type, "ToList", (object)mapData)).ToArray();
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
    }
}