// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2016-07-14 9:28
// ********************************************

using System.Collections.Generic;
using FS.Sql;
using FS.Sql.Map;

namespace TimeTests.DB
{
    /// <summary>
    ///     基数数据
    /// </summary>
    public class Context : DbContext<Context>
    {
        /// <summary>报价表达式表</summary>
        public TableSet<QuoteExpressionPO> QuoteExpression { get; set; }

        protected override void CreateModelInit(Dictionary<string, SetDataMap> map)
        {
            map["QuoteExpression"].SetName("ZTO_QUOTE_EXPRESSION");
        }
    }
}