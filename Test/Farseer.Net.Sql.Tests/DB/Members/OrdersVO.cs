using System;
using FS.Infrastructure;
using FS.Sql.Map.Attribute;

namespace FS.Sql.Tests.DB.Members
{
    /// <summary>
    ///     订单
    /// </summary>
    public class OrdersVO : IEntity<Guid?>
    {
        /// <summary> 订单ID </summary>
        [Field(IsPrimaryKey = true, UpdateStatusType = StatusType.ReadCondition)]
        public Guid? ID { get; set; }

        /// <summary> 订单编号 </summary>
        [Field(UpdateStatusType = StatusType.ReadOnly)]
        public string OrderNo { get; set; }

        /// <summary> 订单总价 </summary>
        [Field(UpdateStatusType = StatusType.ReadOnly)]
        public decimal? Price { get; set; }

        /// <summary> 创建人ID </summary>
        [Field(Name = "CreateID")]
        public UserVO User { get; set; }

        /// <summary> 创建人名称 </summary>
        [Field(UpdateStatusType = StatusType.ReadOnly)]
        public string CreateName { get; set; }

        /// <summary> 创建时间 </summary>
        [Field(UpdateStatusType = StatusType.ReadOnly)]
        public DateTime? CreateAt { get; set; }

        /// <summary> 是否删除 </summary>
        [Field(Name = "IsDeleteByBool")]
        public bool IsDrop { get; set; }
    }
}