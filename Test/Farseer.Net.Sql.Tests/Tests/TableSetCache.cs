using System;
using System.Linq;
using FS.Extends;
using FS.Sql.Tests.DB;
using FS.Sql.Tests.DB.Members;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FS.Sql.Tests.Tests
{
    [TestClass]
    public class TableSetCache
    {
        [TestMethod]
        public void Insert_Normal()
        {
            // 缓存表
            Table.Data.UserRole.Insert(new UserRoleVO { Caption = "标题", Descr = "不清楚" });
            // 不同逻辑删除方式新入（主键为GUID）
            Table.Data.OrdersBoolCache.Insert(new OrdersVO { ID = Guid.NewGuid(), OrderNo = "12345678-5", CreateAt = DateTime.Now, CreateName = "用户1" });

        }
        [TestMethod]
        public void Insert_Transaction()
        {
            using (var context = new Table())
            {
                // 缓存表
                context.UserRole.Insert(new UserRoleVO { Caption = "标题", Descr = "不清楚" });
                // 不同逻辑删除方式新入（主键为GUID）
                context.OrdersBoolCache.Insert(new OrdersVO { ID = Guid.NewGuid(), OrderNo = "12345678-5", CreateAt = DateTime.Now, CreateName = "用户1" });

                context.SaveChanges();
            }
        }

        [TestMethod]
        public void AddUp_Normal()
        {
            var ID = Table.Data.UserRole.Cache.Max(o => o.ID);
            Table.Data.UserRole.Where(o => o.ID > 0).Update(new UserRoleVO { ID = ID, Caption = "testUpdate" });
            Table.Data.UserRole.Append(o => o.Level, 5).AddUp(1, o => o.UserCount, 1);
            var lst = Table.Data.UserRole.Cache;
            var count = lst.Count();
            var userCount = Table.Data.UserRole.Cache.GetValue(1, o => o.UserCount);

            Table.Data.UserRole.Insert(new UserRoleVO { Caption = "test", Descr = "cachetest" });
            Table.Data.UserRole.Where(o => o.Caption == "test").Update(new UserRoleVO { Caption = "testUpdate" });

            Table.Data.UserRole.AddUp(1, o => o.UserCount, 1);
            Table.Data.UserRole.Update(new UserRoleVO { Caption = DateTime.Now.ToShortDateString(), Descr = DateTime.Now.ToShortDateString(), ID = 1 });

            Table.Data.UserRole.AddUp(1, o => o.UserCount, 1);

            Assert.IsTrue(count + 1 == Table.Data.UserRole.Cache.Count(), "缓存同步失败");
            Assert.IsTrue(userCount + 2 == Table.Data.UserRole.Cache.GetValue(1, o => o.UserCount), "缓存同步失败");
        }
        [TestMethod]
        public void AddUp_Transaction()
        {
            using (var context = new Table())
            {
                var ID = context.UserRole.Cache.Max(o => o.ID);
                context.UserRole.Where(o => o.ID > 0).Update(new UserRoleVO { ID = ID, Caption = "testUpdate" });
                context.UserRole.Append(o => o.Level, 5).AddUp(1, o => o.UserCount, 1);
                var lst = context.UserRole.Cache;
                var count = lst.Count();
                var userCount = context.UserRole.Cache.GetValue(1, o => o.UserCount);

                context.UserRole.Insert(new UserRoleVO { Caption = "test", Descr = "cachetest" });
                context.UserRole.Where(o => o.Caption == "test").Update(new UserRoleVO { Caption = "testUpdate" });

                context.UserRole.AddUp(1, o => o.UserCount, 1);
                context.UserRole.Update(new UserRoleVO { Caption = DateTime.Now.ToShortDateString(), Descr = DateTime.Now.ToShortDateString(), ID = 1 });

                context.UserRole.AddUp(1, o => o.UserCount, 1);

                Assert.IsTrue(count + 1 == context.UserRole.Cache.Count(), "缓存同步失败");
                Assert.IsTrue(userCount + 2 == context.UserRole.Cache.GetValue(1, o => o.UserCount), "缓存同步失败");
                context.SaveChanges();
            }
        }

        [TestMethod]
        public void Delete_Normal()
        {
            var ID = Table.Data.UserRole.Cache.OrderByDescending(o => o.ID).GetValue(o => o.ID);
            Table.Data.UserRole.Where(o => o.ID == ID).Delete();
            Assert.IsFalse(Table.Data.UserRole.Cache.Where(o => o.ID == ID).IsHaving());

            var ID2 = Table.Data.Orders.Desc(o => o.ID).GetValue(o => o.ID);
            Table.Data.OrdersBoolCache.Delete(ID2);
            Assert.IsTrue(Table.Data.Orders.Where(o => o.ID == ID2).IsHaving());
            Assert.IsFalse(Table.Data.OrdersBoolCache.Cache.Where(o => o.ID == ID2).IsHaving());
        }
        [TestMethod]
        public void Delete_Transaction()
        {
            using (var context = new Table())
            {
                var ID = context.UserRole.Cache.OrderByDescending(o => o.ID).GetValue(o => o.ID);
                context.UserRole.Where(o => o.ID == ID).Delete();
                Assert.IsFalse(context.UserRole.Cache.Where(o => o.ID == ID).IsHaving());

                var ID2 = context.Orders.Desc(o => o.ID).GetValue(o => o.ID);
                context.OrdersBoolCache.Delete(ID2);
                Assert.IsTrue(context.Orders.Where(o => o.ID == ID2).IsHaving());
                Assert.IsFalse(context.OrdersBoolCache.Cache.Where(o => o.ID == ID2).IsHaving());
                context.SaveChanges();
            }
        }

        [TestMethod]
        public void GetValue_Normal()
        {
        }
        [TestMethod]
        public void GetValue_Transaction()
        {
            using (var context = new Table())
            {

                context.SaveChanges();
            }
        }

        [TestMethod]
        public void Statistics_Normal()
        {
        }
        [TestMethod]
        public void Statistics_Transaction()
        {
            using (var context = new Table())
            {

                context.SaveChanges();
            }
        }

        [TestMethod]
        public void ToEntity_Normal()
        {
            Table.Data.UserRole.Cache.First();
            Table.Data.OrdersBoolCache.Cache.First(o => o.ID != Guid.NewGuid());
        }
        [TestMethod]
        public void ToEntity_Transaction()
        {
            using (var context = new Table())
            {
                context.UserRole.Cache.First();
                context.OrdersBoolCache.Cache.First(o => o.ID != Guid.NewGuid());
                context.SaveChanges();
            }
        }

        [TestMethod]
        public void ToList_Normal()
        {
        }
        [TestMethod]
        public void ToList_Transaction()
        {
            using (var context = new Table())
            {

                context.SaveChanges();
            }
        }

        [TestMethod]
        public void ToSelectList_Normal()
        {
        }
        [TestMethod]
        public void ToSelectList_Transaction()
        {
            using (var context = new Table())
            {

                context.SaveChanges();
            }
        }

        [TestMethod]
        public void Update_Normal()
        {
            var ID = Table.Data.UserRole.Cache.OrderByDescending(o => o.ID).GetValue(o => o.ID);
            var ID2 = Table.Data.Orders.Desc(o => o.ID).GetValue(o => o.ID);
            Table.Data.UserRole.Update(new UserRoleVO { Caption = "标题", Descr = "不清楚" }, ID);
            Table.Data.OrdersBoolCache.Update(new OrdersVO { ID = Guid.NewGuid(), OrderNo = "12345678-5", CreateAt = DateTime.Now, CreateName = "用户1" }, ID2);

        }
        [TestMethod]
        public void Update_Transaction()
        {
            using (var context = new Table())
            {
                var ID = context.UserRole.Cache.OrderByDescending(o => o.ID).GetValue(o => o.ID);
                var ID2 = context.Orders.Desc(o => o.ID).GetValue(o => o.ID);
                context.UserRole.Update(new UserRoleVO { Caption = "标题", Descr = "不清楚" }, ID);
                context.OrdersBoolCache.Update(new OrdersVO { ID = Guid.NewGuid(), OrderNo = "12345678-5", CreateAt = DateTime.Now, CreateName = "用户1" }, ID2);

                context.SaveChanges();
            }
        }
    }
}