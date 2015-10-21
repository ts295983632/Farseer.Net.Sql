using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FS.Sql.Tests.DB;
using FS.Sql.Tests.DB.Members;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FS.Extends;

namespace FS.Sql.Tests
{
    [TestClass]
    public class ToList
    {
        private readonly List<int> _lstIDs = new List<int> {1, 2, 3};

        [TestMethod]
        public void Normal_ToList()
        {
            // 取前十条 随机 非重复的数据
            Assert.IsTrue(Table.Data.User.Desc(o => o.ID).ToList(10, true, true).Count <= 10);
            // 取 随机 非重复的数据
            var IDValue = 0;
            Table.Data.User.Where(o => o.ID == (1 + 3) || o.ID > (IDValue + 3)).ToList(0, true, true);
            // 取 随机 的数据
            Table.Data.User.ToList(0, true);
            // 取 非重复 的数据
            Table.Data.User.ToList(0, false, true);

            // 只取ID
            var ID = Table.Data.User.Select(o => new {o.ID}).ToList(1)[0].ID.GetValueOrDefault();
            // 筛选字段、条件、正序、倒序
            Expression<Func<UserVO, bool>> where = o => o.ID == ID;
            var lst = Table.Data.User.Select(o => new {o.ID, o.PassWord, o.GetDate}).Where(where).Desc(o => new {LoginCount = o.LogCount, o.GenderType}).Asc(o => o.ID).Desc(o => o.GetDate).ToList();
            Assert.IsTrue(lst.Count == 1 && lst[0].PassWord != null && lst[0].GenderType == null && lst[0].LoginIP == null && lst[0].UserName == null && lst[0].ID != null && lst[0].LogCount == null && lst[0].ID == ID);

            // 来一个复杂条件的数据
            Table.Data.User.Select(o => new {o.ID, o.PassWord, o.GetDate}).Where(o => o.ID == ID || o.LogCount < 1 || o.CreateAt < DateTime.Now || o.CreateAt > DateTime.Now.AddDays(-365) || o.UserName.Contains("x") || o.UserName.StartsWith("x") || o.UserName.EndsWith("x") || o.UserName.Length > 0 || o.GenderType == eumGenderType.Man || !o.PassWord.Contains("x") || _lstIDs.Contains(o.ID) || new List<int>().Contains(o.ID)).Desc(o => new {LoginCount = o.LogCount, o.GenderType}).Asc(o => o.ID).Desc(o => o.GetDate).ToList();

            // 取ID为：1、2、3 的数据
            Table.Data.User.Where(o => new List<int> {1, 2, 3}.Contains(o.ID)).ToList();

            Assert.IsTrue(Table.Data.User.Where(new List<int> {1, 2, 3}).ToList().Count <= 3);
            var count = 1;
            Assert.IsTrue(Table.Data.User.Where(_lstIDs).Where(o => (o.LogCount & count) == count).ToList().Count <= 3);

            // 取第2页的数据（每页显示3条数据）
            Assert.IsTrue(Table.Data.User.ToList(3, 2).Count <= 3);

            var recordCount = 0;
            count = 1;
            // 取前99999条数据，并返回总数据
            Assert.IsTrue(Table.Data.User.Select(o => o.ID).Where(o => o.ID > 10).Where(o => (o.LogCount & count) == count).ToList(99999, 1, out recordCount).Count == recordCount);
            Table.Data.User.Select(o => o.CreateAt).Where(o => o.ID > 10).Asc(o => o.ID).Desc(o => new {o.UserName, LoginCount = o.LogCount}).ToList(10, 3, out recordCount);


            // 不同逻辑删除方式新入（主键为GUID）
            var ID2 = Table.Data.Orders.Desc(o => o.ID).GetValue(o => o.ID);
            Table.Data.Orders.Where(o => o.ID != ID2).ToList(3, 5);
            Table.Data.OrdersBool.Where(o => o.ID != ID2).ToList(3, 5);
            Table.Data.OrdersBool.Where(o => o.ID != ID2).ToList(3, 5);
            Table.Data.OrdersNum.Where(o => o.ID != ID2).ToList(3, 5);

            // 存储过程
            Assert.IsTrue(Table.Data.ListUser.ToList().Count > 0);
            // 视图
            Assert.IsTrue(Table.Data.Account.ToList(3, 5).Count > 0);
            // 手动SQL
            Table.Data.ManualSql.ToList<UserVO>("SELECT * FROM Members_User WHERE ID !=3");
        }

        [TestMethod]
        public void Transaction_ToList()
        {
            using (var context = new Table())
            {
                // 取前十条 随机 非重复的数据
                Assert.IsTrue(context.User.Desc(o => o.ID).ToList(10, true, true).Count <= 10);
                // 取 随机 非重复的数据
                var IDValue = 0;
                context.User.Where(o => o.ID == (1 + 3) || o.ID > (IDValue + 3)).ToList(0, true, true);
                // 取 随机 的数据
                context.User.ToList(0, true);
                // 取 非重复 的数据
                context.User.ToList(0, false, true);

                // 只取ID
                var ID = context.User.Select(o => new {o.ID}).ToList(1)[0].ID.GetValueOrDefault();
                // 筛选字段、条件、正序、倒序
                Expression<Func<UserVO, bool>> where = o => o.ID == ID;
                var lst = context.User.Select(o => new {o.ID, o.PassWord, o.GetDate}).Where(where).Desc(o => new {LoginCount = o.LogCount, o.GenderType}).Asc(o => o.ID).Desc(o => o.GetDate).ToList();
                Assert.IsTrue(lst.Count == 1 && lst[0].PassWord != null && lst[0].GenderType == null && lst[0].LoginIP == null && lst[0].UserName == null && lst[0].ID != null && lst[0].LogCount == null && lst[0].ID == ID);

                // 来一个复杂条件的数据
                context.User.Select(o => new {o.ID, o.PassWord, o.GetDate}).Where(o => o.ID == ID || o.LogCount < 1 || o.CreateAt < DateTime.Now || o.CreateAt > DateTime.Now.AddDays(-365) || o.UserName.Contains("x") || o.UserName.StartsWith("x") || o.UserName.EndsWith("x") || o.UserName.Length > 0 || o.GenderType == eumGenderType.Man || !o.PassWord.Contains("x") || _lstIDs.Contains(o.ID) || new List<int>().Contains(o.ID)).Desc(o => new {LoginCount = o.LogCount, o.GenderType}).Asc(o => o.ID).Desc(o => o.GetDate).ToList();

                // 取ID为：1、2、3 的数据
                context.User.Where(o => new List<int> {1, 2, 3}.Contains(o.ID)).ToList();

                Assert.IsTrue(context.User.Where(new List<int> {1, 2, 3}).ToList().Count <= 3);
                var count = 1;
                Assert.IsTrue(context.User.Where(_lstIDs).Where(o => (o.LogCount & count) == count).ToList().Count <= 3);

                // 取第2页的数据（每页显示3条数据）
                Assert.IsTrue(context.User.ToList(3, 2).Count <= 3);

                var recordCount = 0;
                count = 1;
                // 取前99999条数据，并返回总数据
                Assert.IsTrue(context.User.Select(o => o.ID).Where(o => o.ID > 10).Where(o => (o.LogCount & count) == count).ToList(99999, 1, out recordCount).Count == recordCount);
                context.User.Select(o => o.CreateAt).Where(o => o.ID > 10).Asc(o => o.ID).Desc(o => new {o.UserName, LoginCount = o.LogCount}).ToList(10, 3, out recordCount);


                // 不同逻辑删除方式新入（主键为GUID）
                var ID2 = context.Orders.Desc(o => o.ID).GetValue(o => o.ID);
                context.Orders.Where(o => o.ID != ID2).ToList(3, 5);
                context.OrdersBool.Where(o => o.ID != ID2).ToList(3, 5);
                context.OrdersBool.Where(o => o.ID != ID2).ToList(3, 5);
                context.OrdersNum.Where(o => o.ID != ID2).ToList(3, 5);

                // 存储过程
                Assert.IsTrue(context.ListUser.ToList().Count > 0);
                // 视图
                Assert.IsTrue(context.Account.ToList(3, 5).Count > 0);
                // 手动SQL
                var table = context;
                table.ManualSql.ToList<UserVO>("SELECT * FROM Members_User WHERE ID !=3");

                context.SaveChanges();
            }
        }
    }
}