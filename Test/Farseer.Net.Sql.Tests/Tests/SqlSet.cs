using System;
using FS.Sql.Tests.DB;
using FS.Sql.Tests.DB.Members;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FS.Sql.Tests.Tests
{
    [TestClass]
    public class SqlSet
    {
        [TestMethod]
        public void Insert_Normal()
        {
            // 配置SQL
            Table.Data.InsertNewUser.Execute();
        }
        [TestMethod]
        public void Insert_Transaction()
        {
            using (var context = new Table())
            {
                // 配置SQL
                context.InsertNewUser.Execute();
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
            var ID = Table.Data.User.Desc(o => o.ID).GetValue(o => o.ID);
            var table = Table.Data;
            table.ManualSql.Execute("DELETE FROM Members_User WHERE id = @ID", table.DbProvider.CreateDbParam("ID", ID));
            Assert.IsFalse(Table.Data.User.Where(o => o.ID == ID).IsHaving());
        }
        [TestMethod]
        public void Delete_Transaction()
        {
            using (var context = new Table())
            {
                var ID = context.User.Desc(o => o.ID).GetValue(o => o.ID);
                var table = context;
                table.ManualSql.Execute("DELETE FROM Members_User WHERE id = @ID", table.DbProvider.CreateDbParam("ID", ID));
                Assert.IsFalse(context.User.Where(o => o.ID == ID).IsHaving());
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
            Table.Data.GetNewUser.ToEntity();
        }
        [TestMethod]
        public void ToEntity_Transaction()
        {
            using (var context = new Table())
            {
                context.GetNewUser.ToEntity();
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