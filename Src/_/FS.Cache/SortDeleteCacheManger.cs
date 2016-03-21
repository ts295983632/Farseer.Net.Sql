using System;
using FS.Sql.Features;

// ReSharper disable once CheckNamespace

namespace FS.Cache
{
    /// <summary>
    ///     创建逻辑删除功能
    /// </summary>
    public class SortDeleteCacheManger : AbsCacheManger<int, SortDelete>
    {
        /// <summary>
        ///     线程锁
        /// </summary>
        private static readonly object LockObject = new object();

        private readonly eumSortDeleteType _field;
        private readonly object _value;
        private readonly Type _defineType;
        private readonly string _name;

        /// <summary>
        ///     创建逻辑删除功能
        /// </summary>
        /// <param name="name">软删除标记字段名称</param>
        /// <param name="field">数据库字段类型</param>
        /// <param name="value">标记值</param>
        /// <param name="defineType">实体类型</param>
        private SortDeleteCacheManger(string name, eumSortDeleteType field, object value, Type defineType)
        {
            this._field = field;
            this._value = value;
            _defineType = defineType;
            _name = name;

            Key = 0;
            Key += name.GetHashCode();
            Key += field.GetHashCode();
            if (value != null) { Key += value.GetHashCode(); }
            Key += defineType.GetHashCode();
        }

        protected override SortDelete SetCacheLock()
        {
            lock (LockObject)
            {
                if (CacheList.ContainsKey(Key)) { return CacheList[Key]; }

                //缓存中没有找到，新建一个实例
                var sortDelete = new SortDelete() {Name = _name, FieldType = _field, Value = _value};
                sortDelete.Init(_defineType);

                return (CacheList[Key] = sortDelete);
            }
        }

        /// <summary>
        ///     获取缓存
        /// </summary>
        /// <param name="name">软删除标记字段名称</param>
        /// <param name="field">数据库字段类型</param>
        /// <param name="value">标记值</param>
        /// <param name="defineType">实体类型</param>
        public static SortDelete Cache(string name, eumSortDeleteType field, object value, Type defineType)
        {
            return new SortDeleteCacheManger(name, field, value, defineType).GetValue();
        }
    }
}