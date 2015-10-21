using System;
using FS.Sql.Tests.DB;
using FS.Sql.Tests.DB.Members;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FS.Sql.Tests
{
    [TestClass]
    public class AllContext
    {
        [TestMethod]
        public void Test()
        {
            using (var context = new Table())
            {
                //var info = Table.Data.User.Where(o => o.ID > 0 && o.CreateAt < DateTime.Now).Desc(o => new { o.ID, LoginCount = o.LogCount }).Asc(o => o.GenderType).ToEntity();
                var info = context.User.Where(o => o.ID > 0 && o.CreateAt < DateTime.Now).Desc(o => new {o.ID, LoginCount = o.LogCount}).Asc(o => o.GenderType).ToEntity();
                info.PassWord = "77777";
                context.User.Where(o => o.ID == 1).Update(info);

                Table.TransactionInstance(context).ValueUser.GetValue(new ValueUserVO {ID = 1}, "");

                info.ID = null;
                info.PassWord = "00000New";
                context.User.Insert(info);

                context.User.Where(o => o.UserName.Contains("ste")).Count();
                context.User.Where(o => "ste".Contains(o.UserName)).Count();
                var a = new A {b = "steden"};
                context.User.Where(o => a.b.Contains(o.UserName)).Count();

                Table.TransactionInstance(context).InsertUser.Execute(new InsertUserVO {UserName = "now111", PassWord = "old222"});

                context.User.Where(o => o.ID == 1).Update(info);
                context.SaveChanges();
            }
        }
    }

    public class A
    {
        public string b { get; set; }
    }
}