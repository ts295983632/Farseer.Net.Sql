using System;
using System.Collections.Generic;
using FS.Sql.Tests.DB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FS.Extends;

namespace FS.Sql.Tests
{
    [TestClass]
    public class ToSelectList
    {
        private readonly List<int> _lstIDs = new List<int> {1, 2, 3};

        [TestMethod]
        public void Normal_ToSelectList()
        {
            // 取前十条 随机 非重复的数据
            Assert.IsTrue(Table.Data.User.Desc(o => o.ID).ToSelectList(10, o => o.ID).Count <= 10);
            // 取 随机 非重复的数据
            var IDValue = 0;
            Table.Data.User.Where(o => o.ID == (1 + 3) || o.ID > (IDValue + 3)).ToSelectList(0, o => o.ID);

            Table.Data.User.Where(o => o.ID < 20).Desc(o => new {LoginCount = o.LogCount, o.GenderType}).Asc(o => o.ID).Desc(o => o.GetDate).ToSelectList(11, o => new {o.ID, o.PassWord, o.GetDate});

            // 来一个复杂条件的数据
            Table.Data.User.Where(o => o.LogCount < 1 || o.CreateAt < DateTime.Now || o.CreateAt > DateTime.Now.AddDays(-365) || o.UserName.Contains("x") || o.UserName.StartsWith("x") || o.UserName.EndsWith("x") || o.UserName.Length > 0 || o.GenderType == eumGenderType.Man || !o.PassWord.Contains("x") || _lstIDs.Contains(o.ID) || new List<int>().Contains(o.ID)).Desc(o => new {LoginCount = o.LogCount, o.GenderType}).Asc(o => o.ID).Desc(o => o.GetDate).ToSelectList(o => new {o.ID, o.PassWord, o.GetDate});

            // 取ID为：1、2、3 的数据
            Table.Data.User.Where(o => new List<int> {1, 2, 3}.Contains(o.ID)).ToSelectList(o => new {o.ID, o.PassWord, o.GetDate});

            Assert.IsTrue(Table.Data.User.Where(new List<int> {1, 2, 3}).ToSelectList(o => new {o.ID, o.PassWord, o.GetDate}).Count <= 3);
            var count = 1;
            Assert.IsTrue(Table.Data.User.Where(_lstIDs).Where(o => (o.LogCount & count) == count).ToSelectList(o => new {o.ID, o.PassWord, o.GetDate}).Count <= 3);

            // 取第2页的数据（每页显示3条数据）
            Assert.IsTrue(Table.Data.User.ToSelectList(3, o => new {o.ID, o.PassWord, o.GetDate}).Count <= 3);

            // 不同逻辑删除方式新入（主键为GUID）
            var ID2 = Table.Data.Orders.Desc(o => o.ID).GetValue(o => o.ID);
            Table.Data.Orders.Where(o => o.ID != ID2).ToSelectList(3, o => new {o.ID, o.CreateName, o.CreateAt});
            Table.Data.OrdersBool.Where(o => o.ID != ID2).ToSelectList(3, o => new {o.ID, o.CreateName, o.CreateAt});
            Table.Data.OrdersBool.Where(o => o.ID != ID2).ToSelectList(3, o => new {o.ID, o.CreateName, o.CreateAt});
            Table.Data.OrdersNum.Where(o => o.ID != ID2).ToSelectList(3, o => new {o.ID, o.CreateName, o.CreateAt});

            // 视图
            Assert.IsTrue(Table.Data.Account.ToSelectList(3, o => new {o.ID, o.GetDate, o.Name}).Count > 0);
        }

        [TestMethod]
        public void Transaction_ToSelectList()
        {
            using (var context = new Table())
            {
                // 取前十条 随机 非重复的数据
                Assert.IsTrue(context.User.Desc(o => o.ID).ToSelectList(10, o => o.ID).Count <= 10);
                // 取 随机 非重复的数据
                var IDValue = 0;
                context.User.Where(o => o.ID == (1 + 3) || o.ID > (IDValue + 3)).ToSelectList(0, o => o.ID);

                context.User.Where(o => o.ID < 20).Desc(o => new {LoginCount = o.LogCount, o.GenderType}).Asc(o => o.ID).Desc(o => o.GetDate).ToSelectList(11, o => new {o.ID, o.PassWord, o.GetDate});

                // 来一个复杂条件的数据
                context.User.Where(o => o.LogCount < 1 || o.CreateAt < DateTime.Now || o.CreateAt > DateTime.Now.AddDays(-365) || o.UserName.Contains("x") || o.UserName.StartsWith("x") || o.UserName.EndsWith("x") || o.UserName.Length > 0 || o.GenderType == eumGenderType.Man || !o.PassWord.Contains("x") || _lstIDs.Contains(o.ID) || new List<int>().Contains(o.ID)).Desc(o => new {LoginCount = o.LogCount, o.GenderType}).Asc(o => o.ID).Desc(o => o.GetDate).ToSelectList(o => new {o.ID, o.PassWord, o.GetDate});

                // 取ID为：1、2、3 的数据
                context.User.Where(o => new List<int> {1, 2, 3}.Contains(o.ID)).ToSelectList(o => new {o.ID, o.PassWord, o.GetDate});

                Assert.IsTrue(context.User.Where(new List<int> {1, 2, 3}).ToSelectList(o => new {o.ID, o.PassWord, o.GetDate}).Count <= 3);
                var count = 1;
                Assert.IsTrue(context.User.Where(_lstIDs).Where(o => (o.LogCount & count) == count).ToSelectList(o => new {o.ID, o.PassWord, o.GetDate}).Count <= 3);

                // 取第2页的数据（每页显示3条数据）
                Assert.IsTrue(context.User.ToSelectList(3, o => new {o.ID, o.PassWord, o.GetDate}).Count <= 3);

                // 不同逻辑删除方式新入（主键为GUID）
                var ID2 = context.Orders.Desc(o => o.ID).GetValue(o => o.ID);
                context.Orders.Where(o => o.ID != ID2).ToSelectList(3, o => new {o.ID, o.CreateName, o.CreateAt});
                context.OrdersBool.Where(o => o.ID != ID2).ToSelectList(3, o => new {o.ID, o.CreateName, o.CreateAt});
                context.OrdersBool.Where(o => o.ID != ID2).ToSelectList(3, o => new {o.ID, o.CreateName, o.CreateAt});
                context.OrdersNum.Where(o => o.ID != ID2).ToSelectList(3, o => new {o.ID, o.CreateName, o.CreateAt});

                // 视图
                Assert.IsTrue(context.Account.ToSelectList(3, o => new {o.ID, o.GetDate, o.Name}).Count > 0);

                context.SaveChanges();
            }
        }
    }
}