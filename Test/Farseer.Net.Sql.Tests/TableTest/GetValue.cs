using System.Linq;
using FS.Sql.Tests.DB;
using FS.Sql.Tests.DB.Members;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FS.Sql.Tests.TableTest
{
    [TestClass]
    public class GetValue
    {
        [TestMethod]
        public void All()
        {
            Table.Data.User.Where(o => o.ID > 1 && o.ID > 2).GetValue(o => o.UserName).ToList();
            Assert.IsTrue(!string.IsNullOrWhiteSpace(Table.Data.ValueUser.GetValue(new ValueUserVO {ID = 1}, "")));
        }
    }
}