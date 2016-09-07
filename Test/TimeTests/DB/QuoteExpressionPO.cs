// ********************************************
// 作者：何达贤（steden） QQ：11042427
// 时间：2016-07-19 16:39
// ********************************************

using System;
using FS.Sql.Map.Attribute;

namespace TimeTests.DB
{
    /// <summary>报价表达式表</summary>
    public class QuoteExpressionPO
    {
        /// <summary>表达式ID</summary>
        [Field(Name = "ID")]
        public string ID { get; set; }
        /// <summary>报价ID</summary>
        [Field(Name = "QUOTE_ID")]
        public string QuoteID { get; set; }
        /// <summary>重量开始值</summary>
        [Field(Name = "START_WEIGHT")]
        public decimal? StartWeight { get; set; }
        /// <summary>重量结束值</summary>
        [Field(Name = "END_WEIGHT")]
        public decimal? EndWeight { get; set; }
        /// <summary>是否续重</summary>
        [Field(Name = "BL_WEIGHT")]
        public bool? BlWeight { get; set; }
        /// <summary>价格(元/KG)</summary>
        [Field(Name = "WEIGHT_PRICE")]
        public decimal? WeightPrice { get; set; }
        /// <summary>票价(元/票)</summary>
        [Field(Name = "PIECE_PRICE")]
        public decimal? PiecePrice { get; set; }
        /// <summary>首重价(元)</summary>
        [Field(Name = "FIRST_PRICE")]
        public decimal? FirstPrice { get; set; }
        /// <summary>续重价(元/KG) 保留3位小数</summary>
        [Field(Name = "CONTINUED_PRICE")]
        public decimal? ContinuedPrice { get; set; }
        /// <summary>计算表达式</summary>
        [Field(Name = "CALCULATE_EXPRESSION")]
        public string CalculateExpression { get; set; }
    }
}