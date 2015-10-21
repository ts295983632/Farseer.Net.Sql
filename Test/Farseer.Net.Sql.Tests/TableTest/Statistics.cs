using FS.Sql.Tests.DB;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Farseer.Net.Sql.Tests.TableTest
{
    [TestClass]
    public class Statistics
    {
        [TestMethod]
        public void Sum()
        {
            var result = Table.Data.User.Sum(o => o.ID + o.LogCount);
        }
    }
}