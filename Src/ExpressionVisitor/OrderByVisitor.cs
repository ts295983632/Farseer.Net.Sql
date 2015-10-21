using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using FS.Sql.Infrastructure;
using FS.Sql.Map;
using FS.Utils.Common;

namespace FS.Sql.ExpressionVisitor
{
    /// <summary>
    ///     提供字段排序时表达式树的解析
    /// </summary>
    public class OrderByVisitor : AbsSqlVisitor
    {
        /// <summary>
        ///     提供字段排序时表达式树的解析
        /// </summary>
        /// <param name="dbProvider">数据库提供者（不同数据库的特性）</param>
        /// <param name="map">字段映射</param>
        /// <param name="paramList">SQL参数列表</param>
        public OrderByVisitor(AbsDbProvider dbProvider, SetDataMap map, List<DbParameter> paramList) : base(dbProvider, map, paramList)
        {
        }

        /// <summary>
        ///     排序解析入口
        /// </summary>
        /// <param name="lstExp"></param>
        public virtual string Visit(Dictionary<System.Linq.Expressions.Expression, bool> lstExp)
        {
            if (lstExp == null) { return null; }
            foreach (var keyValue in lstExp)
            {
                Visit(keyValue.Key);
                SqlList.Push($"{SqlList.Pop()} {(keyValue.Value ? "ASC" : "DESC")}");
            }

            return ConvertHelper.ToString(SqlList.Reverse(), ", ");
        }
    }
}