using FS.Sql.Tests.DB;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FS.Sql.Tests.TableTest
{
    [TestClass]
    public class AddUp
    {
        [TestMethod]
        public void All()
        {
            Table.Data.User.Where(o => o.ID == 1).AddAssign(o => new {o.LogCount,}, 4).AddUp();
            Table.Data.User.Where(o => o.ID == 1).AddAssign(o => o.LogCount, 100).AddUp();

            using (var context = new Table())
            {
                // 查询实体
                var info = context.User.Desc(o => o.ID).ToEntity();
                // 更新LoginCount字段
                context.User.Where(o => o.ID == info.ID).AddAssign(o => new {LoginCount = o.LogCount}, 4).AddUp();
                context.SaveChanges();
                // 查询实体
                var info2 = context.User.Desc(o => o.ID).ToEntity();
                Assert.IsTrue(info2.LogCount == info.LogCount + 4);
            }
        }
    }
}