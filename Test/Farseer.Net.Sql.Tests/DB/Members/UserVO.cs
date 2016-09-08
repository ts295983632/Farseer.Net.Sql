using System;
using System.Collections;
using System.Collections.Generic;
using FS.Infrastructure;
using FS.Sql.Map.Attribute;

namespace FS.Sql.Tests.DB.Members
{
    public class UserVO : IEntity<int?>,IEnumerable
    {
        /// <summary> 用户ID </summary>
        [Field(IsPrimaryKey = true, IsDbGenerated = true)]
        public int? ID { get; set; }

        /// <summary> 用户名 </summary>
        [Field()]
        public string UserName { get; set; }

        /// <summary> 密码 </summary>
        public string PassWord { get; set; }

        /// <summary> 会员类型 </summary>
        public eumGenderType? GenderType { get; set; }

        /// <summary> 登陆次数 </summary>
        [Field(Name = "LoginCount")]
        public int? LogCount { get; set; }

        /// <summary> 登陆次数 </summary>
        public List<int> SiteIDs { get; set; }

        /// <summary> 登陆IP </summary>
        public string LoginIP { get; set; }

        /// <summary> 登陆IP </summary>
        [Field(Name = "getdate()", IsFun = true)]
        //[Field(Name = "getdate()", IsFun = true)]
        public DateTime? GetDate { get; set; }

        /// <summary> 创建时间 </summary>
        public DateTime? CreateAt { get; set; }

        /// <summary> 用户组 </summary>
        //[ForeignKey(ForeignName = "RoleID", PrimaryTableName = "Members_Role")]
        public UserRoleVO UserRole { get; set; }

        /// <summary> 组ID </summary>
        [Field(Name = "OrderID")]
        //[ForeignKey(ForeignName = "OrderID", PrimaryTableName = "Members_Orders", PrimaryFieldName = "ID")]
        public List<OrdersVO> Orders { get; set; }
        public IEnumerator GetEnumerator() { throw new NotImplementedException(); }
    }
}