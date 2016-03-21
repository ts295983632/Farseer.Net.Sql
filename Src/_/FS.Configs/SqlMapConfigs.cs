using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace

namespace FS.Configs
{
    /// <summary> SQL语句配置文件  </summary>
    public abstract class SqlMapConfigs : AbsConfigs<SqlMapConfig>
    {
    }

    /// <summary> SQL语句配置 </summary>
    [Serializable]
    public class SqlMapConfig
    {
        /// <summary> SQL语句配置列表 </summary>
        public readonly List<SqlMap> SqlMapList = new List<SqlMap>();
    }

    /// <summary> SQL语句配置 </summary>
    [Serializable]
    public class SqlMap
    {
        /// <summary> 映射SqlSet的命名空间 + 属性名称 </summary>
        public string Name = "映射SqlSet的命名空间 + 属性名称";

        /// <summary> SQL语句串 </summary>
        public string Sql = "这里填写SQL语句";

        /// <summary> 通过索引返回实体 </summary>
        public static implicit operator SqlMap(string name)
        {
            return SqlMapConfigs.ConfigEntity.SqlMapList.Find(o => o.Name == name);
        }
    }
}