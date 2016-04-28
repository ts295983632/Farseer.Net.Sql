using System.Collections.Generic;
using FS.Sql.Client.SqlServer;
using FS.Sql.Features;
using FS.Sql.Map;
using FS.Sql.Tests.DB.Members;

namespace FS.Sql.Tests.DB
{
    /// <summary>
    ///     数据库上下文
    /// </summary>
    public class Table : DbContext<Table>
    {
        /// <summary>     默认为0时，可不显示填写这一项，当前只是为了演示 </summary>
        public Table() : base(new SqlServerProvider().CreateDbConnstring(null, null, ".", "Farseer", null, null))
        {
        }
        /// <summary>
        ///     加载表时触发
        /// </summary>
        protected override void CreateModelInit(Dictionary<string, SetDataMap> map)
        {
            // 设置一张表的名称、主键、外键
            map["User"].SetName("Members_User");
            map["UserRole"].SetName("Members_Role");
            map["Orders"].SetName("Members_Orders");

            map["OrdersAt"].SetName("Members_Orders").SetSortDelete("IsDeleteByAt", eumSortDeleteType.DateTime, null);
            map["OrdersBool"].SetName("Members_Orders").SetSortDelete("IsDeleteByBool", eumSortDeleteType.Bool, true);
            map["OrdersNum"].SetName("Members_Orders").SetSortDelete("IsDeleteByNum", eumSortDeleteType.Number, 3);
            map["OrdersBoolCache"].SetName("Members_Orders").SetSortDelete("IsDeleteByBool", eumSortDeleteType.Bool, true);


            map["InfoUser"].SetName("sp_Info_User");
            map["InsertUser"].SetName("sp_Insert_User");
            map["ListUser"].SetName("sp_List_User");
            map["ValueUser"].SetName("sp_Value_User");

            map["Account"].SetName("View_Account");
        }

        /// <summary> 普通表 </summary>
        public TableSet<UserVO> User { get; set; }

        /// <summary> 缓存表 </summary>
        public TableSetCache<UserRoleVO> UserRole { get; set; }


        /// <summary> 普通表（GUID主键） </summary>
        public TableSet<OrdersVO> Orders { get; set; }

        /// <summary> 逻辑删除（日期标记）普通表 </summary>
        public TableSet<OrdersVO> OrdersAt { get; set; }

        /// <summary> 逻辑删除（布尔标记）普通表 </summary>
        public TableSet<OrdersVO> OrdersBool { get; set; }

        /// <summary> 逻辑删除（数字标记）普通表 </summary>
        public TableSet<OrdersVO> OrdersNum { get; set; }

        /// <summary> 逻辑删除（布尔标记）缓存表 </summary>
        public TableSetCache<OrdersVO> OrdersBoolCache { get; set; }

        /// <summary> 存储过程（查询单条数据） </summary>
        public ProcSet<InfoUserVO> InfoUser { get; set; }

        /// <summary> 存储过程（插入数据） </summary>
        public ProcSet<InsertUserVO> InsertUser { get; set; }

        /// <summary> 存储过程（查询多条数据） </summary>
        public ProcSet<ListUserVO> ListUser { get; set; }

        /// <summary> 存储过程（查询单值数据） </summary>
        public ProcSet<ValueUserVO> ValueUser { get; set; }


        /// <summary> SQL配置（查询单条数据） </summary>
        public SqlSet<UserVO> GetNewUser { get; set; }

        /// <summary> SQL配置（插入数据） </summary>
        public SqlSet InsertNewUser { get; set; }


        /// <summary> 普通视图 </summary>
        public ViewSet<AccountVO> Account { get; set; }
    }
}