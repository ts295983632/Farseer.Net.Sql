using System;
using System.Linq;
using FS.Extends;
using FS.Sql.Tests.DB;
using FS.Sql.Tests.DB.Members;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FS.Sql.Tests
{
    [TestClass]
    public class ToEntity
    {
        [TestMethod]
        public void Normal_ToEntity()
        {
            Table.Data.User.Select(o => o.ID).Asc(o => o.ID).ToEntity(1);
            Table.Data.User.Select(o => o.ID).Where(1).ToEntity();
            var info = Table.Data.User.Select(o => o.ID).Select(o => o.LogCount).Where(o => o.ID > 1 || o.UserName.IsEquals("xx")).Desc(o => o.ID).ToEntity();
            Assert.IsTrue(info.ID > 1 && info.PassWord == null && info.GenderType == null && info.LoginIP == null && info.UserName == null && info.ID != null && info.LogCount != null);

            // 缓存表
            Table.Data.UserRole.Cache.First();
            Table.Data.Orders.Where(Guid.NewGuid()).ToEntity();
            Table.Data.OrdersAt.Where(Guid.NewGuid()).ToEntity(Guid.NewGuid());
            Table.Data.OrdersBool.Where(Guid.NewGuid()).ToEntity(Guid.NewGuid());
            Table.Data.OrdersNum.Where(Guid.NewGuid()).ToEntity(Guid.NewGuid());
            Table.Data.OrdersBoolCache.Cache.First(o => o.ID != Guid.NewGuid());

            // View
            var accountInfo = Table.Data.Account.Select(o => o.ID).Select(o => o.Name).Where(o => o.ID > 1).ToEntity();
            Assert.IsTrue(accountInfo.ID > 1 && accountInfo.Pwd == null && accountInfo.Name != null && accountInfo.ID != null);

            // 存储过程
            Assert.IsTrue(Table.Data.InfoUser.ToEntity(new InfoUserVO {ID = 3}).ID == 3);

            // 配置SQL
            Table.Data.GetNewUser.ToEntity();

            // 手动SQL
            var table = Table.Data;
            table.ManualSql.ToEntity<UserVO>("Select * From Members_User Where UserName = @UserName", table.DbProvider.CreateDbParam("UserName", "steden1"));
        }


        [TestMethod]
        public void Transaction_Insert()
        {
            using (var context = new Table())
            {
                context.User.Select(o => o.ID).Asc(o => o.ID).ToEntity(1);
                context.User.Select(o => o.ID).Where(1).ToEntity();
                var info = context.User.Select(o => o.ID).Select(o => o.LogCount).Where(o => o.ID > 1 || o.UserName.IsEquals("xx")).Desc(o => o.ID).ToEntity();
                Assert.IsTrue(info.ID > 1 && info.PassWord == null && info.GenderType == null && info.LoginIP == null && info.UserName == null && info.ID != null && info.LogCount != null);

                // 缓存表
                context.UserRole.Cache.First();
                context.Orders.Where(Guid.NewGuid()).ToEntity();
                context.OrdersAt.Where(Guid.NewGuid()).ToEntity(Guid.NewGuid());
                context.OrdersBool.Where(Guid.NewGuid()).ToEntity(Guid.NewGuid());
                context.OrdersNum.Where(Guid.NewGuid()).ToEntity(Guid.NewGuid());
                context.OrdersBoolCache.Cache.First(o => o.ID != Guid.NewGuid());

                // View
                var accountInfo = context.Account.Select(o => o.ID).Select(o => o.Name).Where(o => o.ID > 1).ToEntity();
                Assert.IsTrue(accountInfo.ID > 1 && accountInfo.Pwd == null && accountInfo.Name != null && accountInfo.ID != null);

                // 存储过程
                Assert.IsTrue(context.InfoUser.ToEntity(new InfoUserVO {ID = 3}).ID == 3);

                // 配置SQL
                context.GetNewUser.ToEntity();

                // 手动SQL
                var table = context;
                table.ManualSql.ToEntity<UserVO>("Select * From Members_User Where UserName = @UserName", table.DbProvider.CreateDbParam("UserName", "steden1"));

                context.SaveChanges();
            }
        }
    }
}