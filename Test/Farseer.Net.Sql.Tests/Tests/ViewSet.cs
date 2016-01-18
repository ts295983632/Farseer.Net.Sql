using System;
using FS.Sql.Tests.DB;
using FS.Sql.Tests.DB.Members;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FS.Sql.Tests.Tests
{
    [TestClass]
    public class ViewSet
    {
        [TestMethod]
        public void Insert_Normal()
        {
        }
        [TestMethod]
        public void Insert_Transaction()
        {
            using (var context = new Table())
            {

                context.SaveChanges();
            }
        }

        [TestMethod]
        public void AddUp_Normal()
        {
        }
        [TestMethod]
        public void AddUp_Transaction()
        {
            using (var context = new Table())
            {

                context.SaveChanges();
            }
        }

        [TestMethod]
        public void Delete_Normal()
        {
        }
        [TestMethod]
        public void Delete_Transaction()
        {
            using (var context = new Table())
            {

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
            var accountInfo = Table.Data.Account.Select(o => o.ID).Select(o => o.Name).Where(o => o.ID > 1).ToEntity();
            Assert.IsTrue(accountInfo.ID > 1 && accountInfo.Pwd == null && accountInfo.Name != null && accountInfo.ID != null);
        }
        [TestMethod]
        public void ToEntity_Transaction()
        {
            using (var context = new Table())
            {
                var accountInfo = context.Account.Select(o => o.ID).Select(o => o.Name).Where(o => o.ID > 1).ToEntity();
                Assert.IsTrue(accountInfo.ID > 1 && accountInfo.Pwd == null && accountInfo.Name != null && accountInfo.ID != null);
                context.SaveChanges();
            }
        }

        [TestMethod]
        public void ToList_Normal()
        {
            Assert.IsTrue(Table.Data.Account.ToList(3, 5).Count > 0);
        }
        [TestMethod]
        public void ToList_Transaction()
        {
            using (var context = new Table())
            {
                Assert.IsTrue(context.Account.ToList(3, 5).Count > 0);
                context.SaveChanges();
            }
        }

        [TestMethod]
        public void ToSelectList_Normal()
        {
            Assert.IsTrue(Table.Data.Account.ToSelectList(3, o => new { o.ID, o.GetDate, o.Name }).Count > 0);
        }
        [TestMethod]
        public void ToSelectList_Transaction()
        {
            using (var context = new Table())
            {
                Assert.IsTrue(context.Account.ToSelectList(3, o => new { o.ID, o.GetDate, o.Name }).Count > 0);
                context.SaveChanges();
            }
        }

        [TestMethod]
        public void Update_Normal()
        {
        }
        [TestMethod]
        public void Update_Transaction()
        {
            using (var context = new Table())
            {

                context.SaveChanges();
            }
        }
    }
}