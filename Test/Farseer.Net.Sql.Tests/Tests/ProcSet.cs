using System;
using FS.Sql.Tests.DB;
using FS.Sql.Tests.DB.Members;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FS.Sql.Tests.Tests
{
    [TestClass]
    public class ProcSet
    {
        [TestMethod]
        public void Insert_Normal()
        {
            var insertUser = new InsertUserVO() { PassWord = "123xx", UserName = "steden" };
            // 存储过程
            Table.Data.InsertUser.Execute(insertUser);
            Assert.IsTrue(insertUser.ID > 0);
        }
        [TestMethod]
        public void Insert_Transaction()
        {
            using (var context = new Table())
            {
                var insertUser = new InsertUserVO() { PassWord = "123xx", UserName = "steden" };
                // 存储过程
                context.InsertUser.Execute(insertUser);
                Assert.IsTrue(insertUser.ID > 0);
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
            Assert.IsTrue(!string.IsNullOrWhiteSpace(Table.Data.ValueUser.GetValue(new ValueUserVO { ID = 1 }, "")));
        }
        [TestMethod]
        public void GetValue_Transaction()
        {
            using (var context = new Table())
            {
                Assert.IsTrue(!string.IsNullOrWhiteSpace(context.ValueUser.GetValue(new ValueUserVO { ID = 1 }, "")));
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
            Assert.IsTrue(Table.Data.InfoUser.ToEntity(new InfoUserVO { ID = 3 }).ID == 3);
        }
        [TestMethod]
        public void ToEntity_Transaction()
        {
            using (var context = new Table())
            {
                Assert.IsTrue(context.InfoUser.ToEntity(new InfoUserVO { ID = 3 }).ID == 3);
                context.SaveChanges();
            }
        }

        [TestMethod]
        public void ToList_Normal()
        {
            Assert.IsTrue(Table.Data.ListUser.ToList().Count > 0);
        }
        [TestMethod]
        public void ToList_Transaction()
        {
            using (var context = new Table())
            {
                Assert.IsTrue(context.ListUser.ToList().Count > 0);
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