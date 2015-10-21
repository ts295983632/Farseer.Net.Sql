using System;
using System.Linq;
using FS.Extends;
using FS.Sql.Tests.DB;
using FS.Sql.Tests.DB.Members;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FS.Sql.Tests
{
    [TestClass]
    public class Update
    {
        [TestMethod]
        public void Normal_Update()
        {
            var userInfo = new UserVO() {UserName = "ffx", GetDate = DateTime.Now};
            var ID = Table.Data.User.Desc(o => o.ID).GetValue(o => o.ID);
            // 重载
            Table.Data.User.Update(userInfo, ID);
            // Where条件
            Table.Data.User.Where(o => o.ID == ID).Update(userInfo);
            // 批量
            var IDs = Table.Data.User.Desc(o => o.ID).ToSelectList(5, o => o.ID);
            Table.Data.User.Update(userInfo, IDs);

            // 缓存表
            ID = Table.Data.UserRole.Cache.OrderByDescending(o => o.ID).GetValue(o => o.ID);
            Table.Data.UserRole.Update(new UserRoleVO {Caption = "标题", Descr = "不清楚"}, ID);

            // 不同逻辑删除方式新入（主键为GUID）
            var ID2 = Table.Data.Orders.Desc(o => o.ID).GetValue(o => o.ID);
            Table.Data.Orders.Update(new OrdersVO {ID = Guid.NewGuid(), OrderNo = "12345678-1", CreateAt = DateTime.Now, CreateName = "用户1"}, ID2);
            Table.Data.OrdersAt.Update(new OrdersVO {ID = Guid.NewGuid(), OrderNo = "12345678-2", CreateAt = DateTime.Now, CreateName = "用户1"}, ID2);
            Table.Data.OrdersBool.Update(new OrdersVO {ID = Guid.NewGuid(), OrderNo = "12345678-3", CreateAt = DateTime.Now, CreateName = "用户1"}, ID2);
            Table.Data.OrdersNum.Update(new OrdersVO {ID = Guid.NewGuid(), OrderNo = "12345678-4", CreateAt = DateTime.Now, CreateName = "用户1"}, ID2);
            Table.Data.OrdersBoolCache.Update(new OrdersVO {ID = Guid.NewGuid(), OrderNo = "12345678-5", CreateAt = DateTime.Now, CreateName = "用户1"}, ID2);

            // 手动SQL
            ID = Table.Data.User.Desc(o => o.ID).GetValue(o => o.ID);
            var table = Table.Data;
            table.ManualSql.Execute("UPDATE Members_User SET UserName=@UserName,PassWord =@PassWord WHERE ID = @ID", table.DbProvider.CreateDbParam("UserName", "steden1"), table.DbProvider.CreateDbParam("PassWord", "steden1"), table.DbProvider.CreateDbParam("ID", ID));
        }

        [TestMethod]
        public void Transaction_Update()
        {
            using (var context = new Table())
            {
                var userInfo = new UserVO() {UserName = "ffx", GetDate = DateTime.Now};
                var ID = context.User.Desc(o => o.ID).GetValue(o => o.ID);
                // 重载
                context.User.Update(userInfo, ID);
                // Where条件
                context.User.Where(o => o.ID == ID).Update(userInfo);
                // 批量
                var IDs = context.User.Desc(o => o.ID).ToSelectList(5, o => o.ID);
                context.User.Update(userInfo, IDs);

                // 缓存表
                ID = context.UserRole.Cache.OrderByDescending(o => o.ID).GetValue(o => o.ID);
                context.UserRole.Update(new UserRoleVO {Caption = "标题", Descr = "不清楚"}, ID);

                // 不同逻辑删除方式新入（主键为GUID）
                var ID2 = context.Orders.Desc(o => o.ID).GetValue(o => o.ID);
                context.Orders.Update(new OrdersVO {ID = Guid.NewGuid(), OrderNo = "12345678-1", CreateAt = DateTime.Now, CreateName = "用户1"}, ID2);
                context.OrdersAt.Update(new OrdersVO {ID = Guid.NewGuid(), OrderNo = "12345678-2", CreateAt = DateTime.Now, CreateName = "用户1"}, ID2);
                context.OrdersBool.Update(new OrdersVO {ID = Guid.NewGuid(), OrderNo = "12345678-3", CreateAt = DateTime.Now, CreateName = "用户1"}, ID2);
                context.OrdersNum.Update(new OrdersVO {ID = Guid.NewGuid(), OrderNo = "12345678-4", CreateAt = DateTime.Now, CreateName = "用户1"}, ID2);
                context.OrdersBoolCache.Update(new OrdersVO {ID = Guid.NewGuid(), OrderNo = "12345678-5", CreateAt = DateTime.Now, CreateName = "用户1"}, ID2);

                // 手动SQL
                ID = context.User.Desc(o => o.ID).GetValue(o => o.ID);
                var table = context;
                table.ManualSql.Execute("UPDATE Members_User SET UserName=@UserName,PassWord =@PassWord WHERE ID = @ID", table.DbProvider.CreateDbParam("UserName", "steden1"), table.DbProvider.CreateDbParam("PassWord", "steden1"), table.DbProvider.CreateDbParam("ID", ID));
                context.SaveChanges();
            }
        }
    }
}