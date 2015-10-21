using FS.Sql.Map.Attribute;

namespace FS.Sql.Tests.DB.Members
{
    public class AccountVO
    {
        /// <summary> 用户ID </summary>
        [Field(IsPrimaryKey = true, IsDbGenerated = true)]
        public int? ID { get; set; }

        /// <summary> 用户名 </summary>
        public string Name { get; set; }

        /// <summary> 密码 </summary>
        public string Pwd { get; set; }

        /// <summary> 登陆IP </summary>
        [Field(Name = "getdate()", IsFun = true)]
        public string GetDate { get; set; }
    }
}