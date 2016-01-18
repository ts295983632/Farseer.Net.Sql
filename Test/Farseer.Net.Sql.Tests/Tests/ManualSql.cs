using System;
using FS.Sql.Tests.DB;
using FS.Sql.Tests.DB.Members;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FS.Sql.Tests.Tests
{
    [TestClass]
    public class ManualSql
    {
        [TestMethod]
        public void Insert_Normal()
        {
            // 手动SQL
            var table = Table.Data;
            table.ManualSql.Execute("INSERT INTO Members_User (UserName,PassWord) VALUES(@UserName,@PassWord)", table.DbProvider.CreateDbParam("UserName", "steden1"), table.DbProvider.CreateDbParam("PassWord", "steden1"));
        }

        [TestMethod]
        public void Insert_Transaction()
        {
            using (var context = new Table())
            {
                context.ManualSql.Execute("INSERT INTO Members_User (UserName,PassWord) VALUES(@UserName,@PassWord)", context.DbProvider.CreateDbParam("UserName", "steden1"), context.DbProvider.CreateDbParam("PassWord", "steden1"));
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
            var table = Table.Data;
            table.ManualSql.ToEntity<UserVO>("Select * From Members_User Where UserName = @UserName", table.DbProvider.CreateDbParam("UserName", "steden1"));
        }
        [TestMethod]
        public void ToEntity_Transaction()
        {
            using (var context = new Table())
            {
                context.ManualSql.ToEntity<UserVO>("Select * From Members_User Where UserName = @UserName", context.DbProvider.CreateDbParam("UserName", "steden1"));
                context.SaveChanges();
            }
        }

        [TestMethod]
        public void ToList_Normal()
        {
            Table.Data.ManualSql.ToList<UserVO>("SELECT * FROM Members_User WHERE ID !=3");
        }
        [TestMethod]
        public void ToList_Transaction()
        {
            using (var context = new Table())
            {
                context.ManualSql.ToList<UserVO>("SELECT * FROM Members_User WHERE ID !=3");
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
            var ID = Table.Data.User.Desc(o => o.ID).GetValue(o => o.ID);
            var table = Table.Data;
            table.ManualSql.Execute($"UPDATE {table.DbProvider.KeywordAegis("Members_User")} SET {table.DbProvider.KeywordAegis("UserName")}= @UserName,{table.DbProvider.KeywordAegis("PassWord")} =@PassWord WHERE {table.DbProvider.KeywordAegis("ID")} = @ID", table.DbProvider.CreateDbParam("UserName", "steden1"), table.DbProvider.CreateDbParam("PassWord", "steden1"), table.DbProvider.CreateDbParam("ID", ID.GetValueOrDefault()));

        }
        [TestMethod]
        public void Update_Transaction()
        {
            using (var context = new Table())
            {
                var ID = context.User.Desc(o => o.ID).GetValue(o => o.ID);
                context.ManualSql.Execute($"UPDATE {context.DbProvider.KeywordAegis("Members_User")} SET {context.DbProvider.KeywordAegis("UserName")}= @UserName,{context.DbProvider.KeywordAegis("PassWord")} =@PassWord WHERE {context.DbProvider.KeywordAegis("ID")} = @ID", context.DbProvider.CreateDbParam("UserName", "steden1"), context.DbProvider.CreateDbParam("PassWord", "steden1"), context.DbProvider.CreateDbParam("ID", ID.GetValueOrDefault()));
                context.SaveChanges();
            }
        }
    }
}