using System;
using System.Linq;
using FS.Extends;
using FS.Sql.Tests.DB;
using FS.Sql.Tests.DB.Members;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FS.Sql.Tests.Features
{
    [TestClass]
    public class Cache
    {
        [TestMethod]
        public void All()
        {
            var ID = Table.Data.UserRole.Cache.Max(o => o.ID);
            Table.Data.UserRole.Where(o => o.ID > 0).Update(new UserRoleVO {ID = ID, Caption = "testUpdate"});
            Table.Data.UserRole.Append(o => o.Level, 5).AddUp(1, o => o.UserCount, 1);
            var lst = Table.Data.UserRole.Cache;
            var count = lst.Count();
            var userCount = Table.Data.UserRole.Cache.GetValue(1, o => o.UserCount);

            using (var context = new Table())
            {
                context.UserRole.Insert(new UserRoleVO {Caption = "test", Descr = "cachetest"});
                context.UserRole.Where(o => o.Caption == "test").Update(new UserRoleVO {Caption = "testUpdate"});

                context.UserRole.AddUp(1, o => o.UserCount, 1);
                context.UserRole.Update(new UserRoleVO {Caption = DateTime.Now.ToShortDateString(), Descr = DateTime.Now.ToShortDateString(), ID = 1});
                context.SaveChanges();
            }

            Table.Data.UserRole.AddUp(1, o => o.UserCount, 1);

            Assert.IsTrue(count + 1 == Table.Data.UserRole.Cache.Count(), "缓存同步失败");
            Assert.IsTrue(userCount + 2 == Table.Data.UserRole.Cache.GetValue(1, o => o.UserCount), "缓存同步失败");
        }
    }
}