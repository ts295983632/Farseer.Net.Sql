using System.Linq;
using FS.Extends;
using FS.Sql.Tests.DB;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FS.Sql.Tests
{
    [TestClass]
    public class Delete
    {
        [TestMethod]
        public void Normal_Delete()
        {
            int? ID;
            // 普通Where
            ID = Table.Data.User.Desc(o => o.ID).GetValue(o => o.ID);
            Table.Data.User.Where(o => o.ID == ID).Delete();
            Assert.IsFalse(Table.Data.User.Where(o => o.ID == ID).IsHaving());
            // 重载
            ID = Table.Data.User.Desc(o => o.ID).GetValue(o => o.ID);
            Table.Data.User.Delete(ID);
            Assert.IsFalse(Table.Data.User.Where(o => o.ID == ID).IsHaving());
            // 批量
            var IDs = Table.Data.User.Desc(o => o.ID).ToSelectList(5, o => o.ID);
            Table.Data.User.Delete(IDs);
            Assert.IsFalse(Table.Data.User.Where(o => IDs.Contains(o.ID)).IsHaving());

            // 缓存表
            ID = Table.Data.UserRole.Cache.OrderByDescending(o => o.ID).GetValue(o => o.ID);
            Table.Data.UserRole.Where(o => o.ID == ID).Delete();
            Assert.IsFalse(Table.Data.UserRole.Cache.Where(o => o.ID == ID).IsHaving());

            // 不同逻辑方式（主键为GUID）
            var ID2 = Table.Data.Orders.Desc(o => o.ID).GetValue(o => o.ID);
            Table.Data.Orders.Delete(ID2);
            Assert.IsFalse(Table.Data.Orders.Where(o => o.ID == ID2).IsHaving());
            ID2 = Table.Data.OrdersAt.Desc(o => o.ID).GetValue(o => o.ID);

            Table.Data.OrdersAt.Delete(ID2);
            Assert.IsFalse(Table.Data.OrdersAt.Where(o => o.ID == ID2).IsHaving());
            Assert.IsTrue(Table.Data.Orders.Where(o => o.ID == ID2).IsHaving());
            Table.Data.OrdersBool.Delete(ID2);
            Assert.IsFalse(Table.Data.OrdersBool.Where(o => o.ID == ID2).IsHaving());
            Assert.IsTrue(Table.Data.Orders.Where(o => o.ID == ID2).IsHaving());
            Table.Data.OrdersNum.Delete(ID2);
            Assert.IsFalse(Table.Data.OrdersNum.Where(o => o.ID == ID2).IsHaving());
            Assert.IsTrue(Table.Data.Orders.Where(o => o.ID == ID2).IsHaving());
            Table.Data.OrdersBoolCache.Delete(ID2);
            Assert.IsFalse(Table.Data.OrdersBoolCache.Cache.Where(o => o.ID == ID2).IsHaving());
            Assert.IsTrue(Table.Data.Orders.Where(o => o.ID == ID2).IsHaving());

            // 手动SQL
            ID = Table.Data.User.Desc(o => o.ID).GetValue(o => o.ID);
            var table = Table.Data;
            table.ManualSql.Execute("DELETE FROM Members_User WHERE id = @ID", table.DbProvider.CreateDbParam("ID", ID));
            Assert.IsFalse(Table.Data.User.Where(o => o.ID == ID).IsHaving());
        }

        [TestMethod]
        public void Transaction_Delete()
        {
            using (var context = new Table())
            {
                int? ID;
                // 普通Where
                ID = context.User.Desc(o => o.ID).GetValue(o => o.ID);
                context.User.Where(o => o.ID == ID).Delete();
                Assert.IsFalse(context.User.Where(o => o.ID == ID).IsHaving());
                // 重载
                ID = context.User.Desc(o => o.ID).GetValue(o => o.ID);
                context.User.Delete(ID);
                Assert.IsFalse(context.User.Where(o => o.ID == ID).IsHaving());
                // 批量
                var IDs = context.User.Desc(o => o.ID).ToSelectList(5, o => o.ID);
                context.User.Delete(IDs);
                Assert.IsFalse(context.User.Where(o => IDs.Contains(o.ID)).IsHaving());

                // 缓存表
                ID = context.UserRole.Cache.OrderByDescending(o => o.ID).GetValue(o => o.ID);
                context.UserRole.Where(o => o.ID == ID).Delete();
                Assert.IsFalse(context.UserRole.Cache.Where(o => o.ID == ID).IsHaving());

                // 不同逻辑方式（主键为GUID）
                var ID2 = context.Orders.Desc(o => o.ID).GetValue(o => o.ID);
                context.Orders.Delete(ID2);
                Assert.IsFalse(context.Orders.Where(o => o.ID == ID2).IsHaving());
                ID2 = context.OrdersAt.Desc(o => o.ID).GetValue(o => o.ID);

                context.OrdersAt.Delete(ID2);
                Assert.IsFalse(context.OrdersAt.Where(o => o.ID == ID2).IsHaving());
                Assert.IsTrue(context.Orders.Where(o => o.ID == ID2).IsHaving());
                context.OrdersBool.Delete(ID2);
                Assert.IsFalse(context.OrdersBool.Where(o => o.ID == ID2).IsHaving());
                Assert.IsTrue(context.Orders.Where(o => o.ID == ID2).IsHaving());
                context.OrdersNum.Delete(ID2);
                Assert.IsFalse(context.OrdersNum.Where(o => o.ID == ID2).IsHaving());
                Assert.IsTrue(context.Orders.Where(o => o.ID == ID2).IsHaving());
                context.OrdersBoolCache.Delete(ID2);
                Assert.IsFalse(context.OrdersBoolCache.Cache.Where(o => o.ID == ID2).IsHaving());
                Assert.IsTrue(context.Orders.Where(o => o.ID == ID2).IsHaving());

                // 手动SQL
                ID = context.User.Desc(o => o.ID).GetValue(o => o.ID);
                var table = context;
                table.ManualSql.Execute("DELETE FROM Members_User WHERE id = @ID", table.DbProvider.CreateDbParam("ID", ID));
                Assert.IsFalse(context.User.Where(o => o.ID == ID).IsHaving());
                context.SaveChanges();
            }
        }
    }
}