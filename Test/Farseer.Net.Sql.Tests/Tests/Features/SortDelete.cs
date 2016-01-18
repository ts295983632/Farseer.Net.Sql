using System;
using System.Linq;
using FS.Sql.Tests.DB;
using FS.Sql.Tests.DB.Members;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FS.Sql.Tests.Tests.Features
{
    [TestClass]
    public class AddUpTest
    {
        [TestMethod]
        public void Bool()
        {
            var info = new OrdersVO() {CreateAt = DateTime.Now, CreateName = "fk", OrderNo = "12345678", Price = 0};

            var allCount = Table.Data.Orders.Count();
            var newCount = Table.Data.OrdersBool.Count();
            info.ID = Guid.NewGuid();
            Table.Data.OrdersBool.Insert(info);
            Assert.IsTrue(++newCount == Table.Data.OrdersBool.Count());
            Assert.IsTrue(++allCount == Table.Data.Orders.Count());

            info = Table.Data.OrdersBool.Where(o => o.ID == info.ID).ToEntity();
            Assert.IsNotNull(info);

            Table.Data.OrdersBool.Where(o => o.ID == info.ID).Delete();

            var ID = info.ID;
            info = Table.Data.OrdersBool.Where(o => o.ID == ID).ToEntity();
            Assert.IsNull(info);


            info = Table.Data.Orders.Where(o => o.ID == ID).ToEntity();
            Assert.IsNotNull(info);
        }

        [TestMethod]
        public void At()
        {
            var info = new OrdersVO() {CreateAt = DateTime.Now, CreateName = "fk", OrderNo = "12345678", Price = 0};


            var allCount = Table.Data.Orders.Count();
            var newCount = Table.Data.OrdersAt.Count();
            info.ID = Guid.NewGuid();
            Table.Data.OrdersAt.Insert(info);
            Assert.IsTrue(++newCount == Table.Data.OrdersAt.Count());
            Assert.IsTrue(++allCount == Table.Data.Orders.Count());

            info = Table.Data.OrdersAt.Where(o => o.ID == info.ID).ToEntity();
            Assert.IsNotNull(info);

            Table.Data.OrdersAt.Where(o => o.ID == info.ID).Delete();

            var ID = info.ID;
            info = Table.Data.OrdersAt.Where(o => o.ID == ID).ToEntity();
            Assert.IsNull(info);


            info = Table.Data.Orders.Where(o => o.ID == ID).ToEntity();
            Assert.IsNotNull(info);
        }

        [TestMethod]
        public void Num()
        {
            var info = new OrdersVO() {CreateAt = DateTime.Now, CreateName = "fk", OrderNo = "12345678", Price = 0};

            var allCount = Table.Data.Orders.Count();
            var newCount = Table.Data.OrdersNum.Count();
            info.ID = Guid.NewGuid();
            Table.Data.OrdersNum.Insert(info);
            Assert.IsTrue(++newCount == Table.Data.OrdersNum.Count());
            Assert.IsTrue(++allCount == Table.Data.Orders.Count());

            info = Table.Data.OrdersNum.Where(o => o.ID == info.ID).ToEntity();
            Assert.IsNotNull(info);

            Table.Data.OrdersNum.Where(o => o.ID == info.ID).Delete();

            var ID = info.ID;
            info = Table.Data.OrdersNum.Where(o => o.ID == ID).ToEntity();
            Assert.IsNull(info);


            info = Table.Data.Orders.Where(o => o.ID == ID).ToEntity();
            Assert.IsNotNull(info);
        }

        [TestMethod]
        public void Update()
        {
            var ID = Table.Data.OrdersAt.GetValue(o => o.ID);
            Table.Data.OrdersAt.Where(o => o.ID == ID).Update(new OrdersVO() {CreateAt = DateTime.Now});
            Table.Data.OrdersAt.Where(o => o.ID == ID).AddUp(o => o.Price, 1);


            ID = Table.Data.OrdersBool.GetValue(o => o.ID);
            Table.Data.OrdersBool.Where(o => o.ID == ID).Update(new OrdersVO() {CreateAt = DateTime.Now});
            Table.Data.OrdersBool.Where(o => o.ID == ID).AddUp(o => o.Price, 1);


            ID = Table.Data.OrdersNum.GetValue(o => o.ID);
            Table.Data.OrdersNum.Where(o => o.ID == ID).Update(new OrdersVO() {CreateAt = DateTime.Now});
            Table.Data.OrdersNum.Where(o => o.ID == ID).AddUp(o => o.Price, 1);
        }

        [TestMethod]
        public void Cache()
        {
            var info = new OrdersVO() {CreateAt = DateTime.Now, CreateName = "fk", OrderNo = "12345678", Price = 0};

            var lstCache = Table.Data.OrdersBoolCache.Cache;
            var dropID = Table.Data.Orders.Where(o => o.IsDrop == true).GetValue(o => o.ID);
            Assert.IsFalse(lstCache.Any(o => o.ID == dropID));

            var allCount = Table.Data.Orders.Count();
            var cacheCount = Table.Data.OrdersBoolCache.Cache.Count();
            info.ID = Guid.NewGuid();
            Table.Data.OrdersBoolCache.Insert(info);
            Assert.IsTrue(++cacheCount == Table.Data.OrdersBoolCache.Cache.Count());
            Assert.IsTrue(++allCount == Table.Data.Orders.Count());

            info = Table.Data.OrdersBoolCache.Cache.FirstOrDefault(o => o.ID == info.ID);
            Assert.IsNotNull(info);

            Table.Data.OrdersBoolCache.Where(o => o.ID == info.ID).Delete();

            var ID = info.ID;
            info = Table.Data.OrdersBoolCache.Cache.FirstOrDefault(o => o.ID == ID);
            Assert.IsNull(info);


            info = Table.Data.Orders.Where(o => o.ID == ID).ToEntity();
            Assert.IsNotNull(info);
        }
    }
}