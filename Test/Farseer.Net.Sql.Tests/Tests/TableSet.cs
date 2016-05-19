using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FS.Extends;
using FS.Sql.Client.PostgreSql;
using FS.Sql.Tests.DB;
using FS.Sql.Tests.DB.Members;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FS.Sql.Tests.Tests
{
    [TestClass]
    public class TableSet
    {
        [TestMethod]
        public void Insert_Normal()
        {
            int ID;
            var userInfo = new UserVO() { UserName = "yy", GetDate = DateTime.Now };

            // 将标识设置到标识字段
            Table.Data.User.Insert(userInfo, true);
            Assert.IsTrue(userInfo.ID > 0);

            if (!(Table.Data.DbProvider is PostgreSqlProvider))
            {
                // 设置out ID
                userInfo.ID++;
                Table.Data.User.Insert(userInfo, out ID);
                Assert.IsTrue(userInfo.ID > 0 && userInfo.ID == ID);
            }
            // 不同逻辑删除方式新入（主键为GUID）
            Table.Data.Orders.Insert(new OrdersVO { ID = Guid.NewGuid(), OrderNo = "12345678-1", CreateAt = DateTime.Now, CreateName = "用户1" });
            Table.Data.OrdersAt.Insert(new OrdersVO { ID = Guid.NewGuid(), OrderNo = "12345678-2", CreateAt = DateTime.Now, CreateName = "用户1" });
            Table.Data.OrdersBool.Insert(new OrdersVO { ID = Guid.NewGuid(), OrderNo = "12345678-3", CreateAt = DateTime.Now, CreateName = "用户1" });
            Table.Data.OrdersNum.Insert(new OrdersVO { ID = Guid.NewGuid(), OrderNo = "12345678-4", CreateAt = DateTime.Now, CreateName = "用户1" });
        }

        [TestMethod]
        public void Insert_Transaction()
        {
            using (var context = new Table())
            {
                int ID;
                var userInfo = new UserVO() { UserName = "yy", GetDate = DateTime.Now };

                // 将标识设置到标识字段
                context.User.Insert(userInfo, true);
                Assert.IsTrue(userInfo.ID > 0);

                if (!(Table.Data.DbProvider is PostgreSqlProvider))
                {
                    // 设置out ID
                    userInfo.ID++;
                    context.User.Insert(userInfo, out ID);
                    Assert.IsTrue(userInfo.ID > 0 && userInfo.ID == ID);
                }

                // 不同逻辑删除方式新入（主键为GUID）
                context.Orders.Insert(new OrdersVO { ID = Guid.NewGuid(), OrderNo = "12345678-1", CreateAt = DateTime.Now, CreateName = "用户1" });
                context.OrdersAt.Insert(new OrdersVO { ID = Guid.NewGuid(), OrderNo = "12345678-2", CreateAt = DateTime.Now, CreateName = "用户1" });
                context.OrdersBool.Insert(new OrdersVO { ID = Guid.NewGuid(), OrderNo = "12345678-3", CreateAt = DateTime.Now, CreateName = "用户1" });
                context.OrdersNum.Insert(new OrdersVO { ID = Guid.NewGuid(), OrderNo = "12345678-4", CreateAt = DateTime.Now, CreateName = "用户1" });
                context.SaveChanges();
            }
        }

        [TestMethod]
        public void AddUp_Normal()
        {
            // 查询实体
            var infoBefore = Table.Data.User.Desc(o => o.ID).ToEntity();
            Table.Data.User.Where(o => o.ID == infoBefore.ID).Update(new UserVO() { LogCount = 1 });
            infoBefore.LogCount = 1;

            Table.Data.User.Where(o => o.ID == infoBefore.ID).AddAssign(o => new { o.LogCount, }, 4).AddUp();
            Table.Data.User.Where(o => o.ID == infoBefore.ID).AddAssign(o => o.LogCount, 3).AddUp();
            var infoAfter = Table.Data.User.Desc(o => o.ID).ToEntity();
            Assert.IsTrue(infoAfter.LogCount == infoBefore.LogCount + 7);
        }

        [TestMethod]
        public void AddUp_Transaction()
        {
            using (var context = new Table())
            {
                // 查询实体
                var infoBefore = context.User.Desc(o => o.ID).ToEntity();
                context.User.Where(o => o.ID == infoBefore.ID).Update(new UserVO() { LogCount = 1 });
                infoBefore.LogCount = 1;

                context.User.Where(o => o.ID == infoBefore.ID).AddAssign(o => new { o.LogCount, }, 4).AddUp();
                context.User.Where(o => o.ID == infoBefore.ID).AddAssign(o => o.LogCount, 3).AddUp();
                var infoAfter = context.User.Desc(o => o.ID).ToEntity();
                context.SaveChanges();

                Assert.IsTrue(infoAfter.LogCount == infoBefore.LogCount + 7);
            }
        }

        [TestMethod]
        public void Delete_Normal()
        {
            // 普通Where
            var ID = Table.Data.User.Desc(o => o.ID).GetValue(o => o.ID);
            Table.Data.User.Where(o => o.ID == ID).Delete();
            Assert.IsFalse(Table.Data.User.Where(o => o.ID == ID).IsHaving());
            // 重载
            ID = Table.Data.User.Desc(o => o.ID).GetValue(o => o.ID);
            Table.Data.User.Delete(ID);
            Assert.IsFalse(Table.Data.User.Where(o => o.ID == ID).IsHaving());
            // 批量
            var IDs = Table.Data.User.Desc(o => o.ID).ToSelectList(5, o => o.ID);
            Table.Data.User.Delete(IDs);
            Assert.IsFalse(Table.Data.User.Where(o => IDs.Contains(o.ID)).IsHaving());

            var ID2 = Table.Data.Orders.Desc(o => o.ID).GetValue(o => o.ID);
            Table.Data.Orders.Delete(ID2);
            Assert.IsFalse(Table.Data.Orders.Where(o => o.ID == ID2).IsHaving());

            ID2 = Table.Data.OrdersAt.Desc(o => o.ID).GetValue(o => o.ID);
            Table.Data.OrdersAt.Delete(ID2);
            Assert.IsFalse(Table.Data.OrdersAt.Where(o => o.ID == ID2).IsHaving());
            Assert.IsTrue(Table.Data.Orders.Where(o => o.ID == ID2).IsHaving());

            Table.Data.OrdersBool.Delete(ID2);
            Assert.IsFalse(Table.Data.OrdersBool.Where(o => o.ID == ID2).IsHaving());
            Assert.IsTrue(Table.Data.Orders.Where(o => o.ID == ID2).IsHaving());

            Table.Data.OrdersNum.Delete(ID2);
            Assert.IsFalse(Table.Data.OrdersNum.Where(o => o.ID == ID2).IsHaving());
            Assert.IsTrue(Table.Data.Orders.Where(o => o.ID == ID2).IsHaving());
        }
        [TestMethod]
        public void Delete_Transaction()
        {
            using (var context = new Table())
            {
                // 普通Where
                var ID = context.User.Desc(o => o.ID).GetValue(o => o.ID);
                context.User.Where(o => o.ID == ID).Delete();
                Assert.IsFalse(context.User.Where(o => o.ID == ID).IsHaving());
                // 重载
                ID = context.User.Desc(o => o.ID).GetValue(o => o.ID);
                context.User.Delete(ID);
                Assert.IsFalse(context.User.Where(o => o.ID == ID).IsHaving());
                // 批量
                var IDs = context.User.Desc(o => o.ID).ToSelectList(5, o => o.ID);
                context.User.Delete(IDs);
                Assert.IsFalse(context.User.Where(o => IDs.Contains(o.ID)).IsHaving());

                var ID2 = context.Orders.Desc(o => o.ID).GetValue(o => o.ID);
                context.Orders.Delete(ID2);
                Assert.IsFalse(context.Orders.Where(o => o.ID == ID2).IsHaving());

                ID2 = context.OrdersAt.Desc(o => o.ID).GetValue(o => o.ID);
                context.OrdersAt.Delete(ID2);
                Assert.IsFalse(context.OrdersAt.Where(o => o.ID == ID2).IsHaving());
                Assert.IsTrue(context.Orders.Where(o => o.ID == ID2).IsHaving());

                context.OrdersBool.Delete(ID2);
                Assert.IsFalse(context.OrdersBool.Where(o => o.ID == ID2).IsHaving());
                Assert.IsTrue(context.Orders.Where(o => o.ID == ID2).IsHaving());

                context.OrdersNum.Delete(ID2);
                Assert.IsFalse(context.OrdersNum.Where(o => o.ID == ID2).IsHaving());
                Assert.IsTrue(context.Orders.Where(o => o.ID == ID2).IsHaving());
                context.SaveChanges();
            }
        }

        [TestMethod]
        public void GetValue_Normal()
        {
            Table.Data.User.Where(o => o.ID > 1 && o.ID > 2).GetValue(o => o.UserName).ToList();
        }
        [TestMethod]
        public void GetValue_Transaction()
        {
            using (var context = new Table())
            {
                context.User.Where(o => o.ID > 1 && o.ID > 2).GetValue(o => o.UserName).ToList();
                context.SaveChanges();
            }
        }

        [TestMethod]
        public void Statistics_Normal()
        {
            var result = Table.Data.User.Sum(o => o.ID + o.LogCount);
        }
        [TestMethod]
        public void Statistics_Transaction()
        {
            using (var context = new Table())
            {
                var result = context.User.Sum(o => o.ID + o.LogCount);
                context.SaveChanges();
            }
        }

        [TestMethod]
        public void ToEntity_Normal()
        {
            Table.Data.User.Select(o => o.ID).Asc(o => o.ID).ToEntity(1);
            Table.Data.User.Select(o => o.ID).Where(1).ToEntity();
            var info = Table.Data.User.Select(o => o.ID).Select(o => o.UserName).Where(o => o.ID > 1 || o.UserName.IsEquals("xx")).Desc(o => o.ID).ToEntity();
            Assert.IsTrue(info.ID > 1 && info.PassWord == null && info.GenderType == null && info.LoginIP == null && info.UserName != null && info.ID != null && info.LogCount == null);

            Table.Data.Orders.Where(Guid.NewGuid()).ToEntity();
            Table.Data.OrdersAt.Where(Guid.NewGuid()).ToEntity(Guid.NewGuid());
            Table.Data.OrdersBool.Where(Guid.NewGuid()).ToEntity(Guid.NewGuid());
            Table.Data.OrdersNum.Where(Guid.NewGuid()).ToEntity(Guid.NewGuid());
        }
        [TestMethod]
        public void ToEntity_Transaction()
        {
            using (var context = new Table())
            {
                context.User.Select(o => o.ID).Asc(o => o.ID).ToEntity(1);
                context.User.Select(o => o.ID).Where(1).ToEntity();
                var info = context.User.Select(o => o.ID).Select(o => o.UserName).Where(o => o.ID > 1 || o.UserName.IsEquals("xx")).Desc(o => o.ID).ToEntity();
                Assert.IsTrue(info.ID > 1 && info.PassWord == null && info.GenderType == null && info.LoginIP == null && info.UserName != null && info.ID != null && info.LogCount == null);

                context.Orders.Where(Guid.NewGuid()).ToEntity();
                context.OrdersAt.Where(Guid.NewGuid()).ToEntity(Guid.NewGuid());
                context.OrdersBool.Where(Guid.NewGuid()).ToEntity(Guid.NewGuid());
                context.OrdersNum.Where(Guid.NewGuid()).ToEntity(Guid.NewGuid());
                context.SaveChanges();
            }
        }

        [TestMethod]
        public void ToList_Normal()
        {
            Table.Data.User.Where(o => new List<int> { 1, 2, 3 }.Contains(o.ID) || new int[] { 1, 2, 3 }.Contains(o.ID)).ToList();
            List<int> _lstIDs = new List<int> { 1, 2, 3 };
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
            var ID = Table.Data.User.Select(o => new { o.ID }).ToList(1)[0].ID.GetValueOrDefault();
            // 筛选字段、条件、正序、倒序
            Expression<Func<UserVO, bool>> where = o => o.ID == ID;
            var lst = Table.Data.User.Select(o => new { o.ID, o.PassWord, o.GetDate }).Where(where).Desc(o => new { LoginCount = o.LogCount, o.GenderType }).Asc(o => o.ID).Desc(o => o.GetDate).ToList();
            Assert.IsTrue(lst.Count == 1 && lst[0].PassWord != null && lst[0].GenderType == null && lst[0].LoginIP == null && lst[0].UserName == null && lst[0].ID != null && lst[0].LogCount == null && lst[0].ID == ID);

            // 来一个复杂条件的数据
            Table.Data.User.Select(o => new { o.ID, o.PassWord, o.GetDate }).Where(o => o.ID == ID || o.LogCount < 1 || o.CreateAt < DateTime.Now || o.CreateAt > DateTime.Now.AddDays(-365) || o.UserName.Contains("x") || o.UserName.StartsWith("x") || o.UserName.EndsWith("x") || o.UserName.Length > 0 || o.GenderType == eumGenderType.Man || !o.PassWord.Contains("x") || _lstIDs.Contains(o.ID) || new List<int>().Contains(o.ID)).Desc(o => new { LoginCount = o.LogCount, o.GenderType }).Asc(o => o.ID).Desc(o => o.GetDate).ToList();

            // 取ID为：1、2、3 的数据
            Table.Data.User.Where(o => new List<int> { 1, 2, 3 }.Contains(o.ID)).ToList();

            Assert.IsTrue(Table.Data.User.Where(new List<int> { 1, 2, 3 }).ToList().Count <= 3);
            var count = 1;
            Assert.IsTrue(Table.Data.User.Where(_lstIDs).Where(o => (o.LogCount & count) == count).ToList().Count <= 3);

            // 取第2页的数据（每页显示3条数据）
            Assert.IsTrue(Table.Data.User.ToList(3, 2).Count <= 3);

            var recordCount = 0;
            count = 1;
            // 取前99999条数据，并返回总数据
            Assert.IsTrue(Table.Data.User.Select(o => o.ID).Where(o => o.ID > 10).Where(o => (o.LogCount & count) == count).ToList(99999, 1, out recordCount).Count == recordCount);
            Table.Data.User.Select(o => o.CreateAt).Where(o => o.ID > 10).Asc(o => o.ID).Desc(o => new { o.UserName, LoginCount = o.LogCount }).ToList(10, 3, out recordCount);


            // 不同逻辑删除方式新入（主键为GUID）
            var ID2 = Table.Data.Orders.Desc(o => o.ID).GetValue(o => o.ID);
            Table.Data.Orders.Where(o => o.ID != ID2).ToList(3, 5);
            Table.Data.OrdersBool.Where(o => o.ID != ID2).ToList(3, 5);
            Table.Data.OrdersBool.Where(o => o.ID != ID2).ToList(3, 5);
            Table.Data.OrdersNum.Where(o => o.ID != ID2).ToList(3, 5);
        }
        [TestMethod]
        public void ToList_Transaction()
        {
            using (var context = new Table())
            {
                List<int> _lstIDs = new List<int> { 1, 2, 3 };
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
                var ID = context.User.Select(o => new { o.ID }).ToList(1)[0].ID.GetValueOrDefault();
                // 筛选字段、条件、正序、倒序
                Expression<Func<UserVO, bool>> where = o => o.ID == ID;
                var lst = context.User.Select(o => new { o.ID, o.PassWord, o.GetDate }).Where(where).Desc(o => new { LoginCount = o.LogCount, o.GenderType }).Asc(o => o.ID).Desc(o => o.GetDate).ToList();
                Assert.IsTrue(lst.Count == 1 && lst[0].PassWord != null && lst[0].GenderType == null && lst[0].LoginIP == null && lst[0].UserName == null && lst[0].ID != null && lst[0].LogCount == null && lst[0].ID == ID);

                // 来一个复杂条件的数据
                context.User.Select(o => new { o.ID, o.PassWord, o.GetDate }).Where(o => o.ID == ID || o.LogCount < 1 || o.CreateAt < DateTime.Now || o.CreateAt > DateTime.Now.AddDays(-365) || o.UserName.Contains("x") || o.UserName.StartsWith("x") || o.UserName.EndsWith("x") || o.UserName.Length > 0 || o.GenderType == eumGenderType.Man || !o.PassWord.Contains("x") || _lstIDs.Contains(o.ID) || new List<int>().Contains(o.ID)).Desc(o => new { LoginCount = o.LogCount, o.GenderType }).Asc(o => o.ID).Desc(o => o.GetDate).ToList();

                // 取ID为：1、2、3 的数据
                context.User.Where(o => new List<int> { 1, 2, 3 }.Contains(o.ID)).ToList();

                Assert.IsTrue(context.User.Where(new List<int> { 1, 2, 3 }).ToList().Count <= 3);
                var count = 1;
                Assert.IsTrue(context.User.Where(_lstIDs).Where(o => (o.LogCount & count) == count).ToList().Count <= 3);

                // 取第2页的数据（每页显示3条数据）
                Assert.IsTrue(context.User.ToList(3, 2).Count <= 3);

                var recordCount = 0;
                count = 1;
                // 取前99999条数据，并返回总数据
                Assert.IsTrue(context.User.Select(o => o.ID).Where(o => o.ID > 10).Where(o => (o.LogCount & count) == count).ToList(99999, 1, out recordCount).Count == recordCount);
                context.User.Select(o => o.CreateAt).Where(o => o.ID > 10).Asc(o => o.ID).Desc(o => new { o.UserName, LoginCount = o.LogCount }).ToList(10, 3, out recordCount);


                // 不同逻辑删除方式新入（主键为GUID）
                var ID2 = context.Orders.Desc(o => o.ID).GetValue(o => o.ID);
                context.Orders.Where(o => o.ID != ID2).ToList(3, 5);
                context.OrdersBool.Where(o => o.ID != ID2).ToList(3, 5);
                context.OrdersBool.Where(o => o.ID != ID2).ToList(3, 5);
                context.OrdersNum.Where(o => o.ID != ID2).ToList(3, 5);
                context.SaveChanges();
            }
        }

        [TestMethod]
        public void ToSelectList_Normal()
        {
            var _lstIDs = new List<int> { 1, 2, 3 };
            // 取前十条 随机 非重复的数据
            Assert.IsTrue(Table.Data.User.Desc(o => o.ID).ToSelectList(10, o => o.ID).Count <= 10);
            // 取 随机 非重复的数据
            var IDValue = 0;
            Table.Data.User.Where(o => o.ID == (1 + 3) || o.ID > (IDValue + 3)).ToSelectList(0, o => o.ID);

            Table.Data.User.Where(o => o.ID < 20).Desc(o => new { LoginCount = o.LogCount, o.GenderType }).Asc(o => o.ID).Desc(o => o.GetDate).ToSelectList(11, o => new { o.ID, o.PassWord, o.GetDate });

            // 来一个复杂条件的数据
            Table.Data.User.Where(o => o.LogCount < 1 || o.CreateAt < DateTime.Now || o.CreateAt > DateTime.Now.AddDays(-365) || o.UserName.Contains("x") || o.UserName.StartsWith("x") || o.UserName.EndsWith("x") || o.UserName.Length > 0 || o.GenderType == eumGenderType.Man || !o.PassWord.Contains("x") || _lstIDs.Contains(o.ID) || new List<int>().Contains(o.ID)).Desc(o => new { LoginCount = o.LogCount, o.GenderType }).Asc(o => o.ID).Desc(o => o.GetDate).ToSelectList(o => new { o.ID, o.PassWord, o.GetDate });
            // 取ID为：1、2、3 的数据
            Table.Data.User.Where(o => new List<int> { 1, 2, 3 }.Contains(o.ID)).ToSelectList(o => new { o.ID, o.PassWord, o.GetDate });

            Assert.IsTrue(Table.Data.User.Where(new List<int> { 1, 2, 3 }).ToSelectList(o => new { o.ID, o.PassWord, o.GetDate }).Count <= 3);
            var count = 1;
            Assert.IsTrue(Table.Data.User.Where(_lstIDs).Where(o => (o.LogCount & count) == count).ToSelectList(o => new { o.ID, o.PassWord, o.GetDate }).Count <= 3);

            // 取第2页的数据（每页显示3条数据）
            Assert.IsTrue(Table.Data.User.ToSelectList(3, o => new { o.ID, o.PassWord, o.GetDate }).Count <= 3);

            // 不同逻辑删除方式新入（主键为GUID）
            var ID2 = Table.Data.Orders.Desc(o => o.ID).GetValue(o => o.ID);
            Table.Data.Orders.Where(o => o.ID != ID2).ToSelectList(3, o => new { o.ID, o.CreateName, o.CreateAt });
            Table.Data.OrdersBool.Where(o => o.ID != ID2).ToSelectList(3, o => new { o.ID, o.CreateName, o.CreateAt });
            Table.Data.OrdersBool.Where(o => o.ID != ID2).ToSelectList(3, o => new { o.ID, o.CreateName, o.CreateAt });
            Table.Data.OrdersNum.Where(o => o.ID != ID2).ToSelectList(3, o => new { o.ID, o.CreateName, o.CreateAt });
        }
        [TestMethod]
        public void ToSelectList_Transaction()
        {
            using (var context = new Table())
            {
                var _lstIDs = new List<int> { 1, 2, 3 };
                // 取前十条 随机 非重复的数据
                Assert.IsTrue(context.User.Desc(o => o.ID).ToSelectList(10, o => o.ID).Count <= 10);
                // 取 随机 非重复的数据
                var IDValue = 0;
                context.User.Where(o => o.ID == (1 + 3) || o.ID > (IDValue + 3)).ToSelectList(0, o => o.ID);

                context.User.Where(o => o.ID < 20).Desc(o => new { LoginCount = o.LogCount, o.GenderType }).Asc(o => o.ID).Desc(o => o.GetDate).ToSelectList(11, o => new { o.ID, o.PassWord, o.GetDate });

                // 来一个复杂条件的数据
                context.User.Where(o => o.LogCount < 1 || o.CreateAt < DateTime.Now || o.CreateAt > DateTime.Now.AddDays(-365) || o.UserName.Contains("x") || o.UserName.StartsWith("x") || o.UserName.EndsWith("x") || o.UserName.Length > 0 || o.GenderType == eumGenderType.Man || !o.PassWord.Contains("x") || _lstIDs.Contains(o.ID) || new List<int>().Contains(o.ID)).Desc(o => new { LoginCount = o.LogCount, o.GenderType }).Asc(o => o.ID).Desc(o => o.GetDate).ToSelectList(o => new { o.ID, o.PassWord, o.GetDate });
                // 取ID为：1、2、3 的数据
                context.User.Where(o => new List<int> { 1, 2, 3 }.Contains(o.ID)).ToSelectList(o => new { o.ID, o.PassWord, o.GetDate });

                Assert.IsTrue(context.User.Where(new List<int> { 1, 2, 3 }).ToSelectList(o => new { o.ID, o.PassWord, o.GetDate }).Count <= 3);
                var count = 1;
                Assert.IsTrue(context.User.Where(_lstIDs).Where(o => (o.LogCount & count) == count).ToSelectList(o => new { o.ID, o.PassWord, o.GetDate }).Count <= 3);

                // 取第2页的数据（每页显示3条数据）
                Assert.IsTrue(context.User.ToSelectList(3, o => new { o.ID, o.PassWord, o.GetDate }).Count <= 3);

                // 不同逻辑删除方式新入（主键为GUID）
                var ID2 = context.Orders.Desc(o => o.ID).GetValue(o => o.ID);
                context.Orders.Where(o => o.ID != ID2).ToSelectList(3, o => new { o.ID, o.CreateName, o.CreateAt });
                context.OrdersBool.Where(o => o.ID != ID2).ToSelectList(3, o => new { o.ID, o.CreateName, o.CreateAt });
                context.OrdersBool.Where(o => o.ID != ID2).ToSelectList(3, o => new { o.ID, o.CreateName, o.CreateAt });
                context.OrdersNum.Where(o => o.ID != ID2).ToSelectList(3, o => new { o.ID, o.CreateName, o.CreateAt });
                context.SaveChanges();
            }
        }

        [TestMethod]
        public void Update_Normal()
        {
            var userInfo = new UserVO() { UserName = "ffx", GetDate = DateTime.Now };
            var ID = Table.Data.User.Desc(o => o.ID).GetValue(o => o.ID);
            // 重载
            Table.Data.User.Update(userInfo, ID);
            // Where条件
            Table.Data.User.Where(o => o.ID == ID).Update(userInfo);
            // 批量
            var IDs = Table.Data.User.Desc(o => o.ID).ToSelectList(5, o => o.ID);
            Table.Data.User.Update(userInfo, IDs);

            var ID2 = Table.Data.Orders.Desc(o => o.ID).GetValue(o => o.ID);
            Table.Data.Orders.Update(new OrdersVO { ID = Guid.NewGuid(), OrderNo = "12345678-1", CreateAt = DateTime.Now, CreateName = "用户1" }, ID2);
            Table.Data.OrdersAt.Update(new OrdersVO { ID = Guid.NewGuid(), OrderNo = "12345678-2", CreateAt = DateTime.Now, CreateName = "用户1" }, ID2);
            Table.Data.OrdersBool.Update(new OrdersVO { ID = Guid.NewGuid(), OrderNo = "12345678-3", CreateAt = DateTime.Now, CreateName = "用户1" }, ID2);
            Table.Data.OrdersNum.Update(new OrdersVO { ID = Guid.NewGuid(), OrderNo = "12345678-4", CreateAt = DateTime.Now, CreateName = "用户1" }, ID2);

        }
        [TestMethod]
        public void Update_Transaction()
        {
            using (var context = new Table())
            {
                var userInfo = new UserVO() { UserName = "ffx", GetDate = DateTime.Now };
                var ID = context.User.Desc(o => o.ID).GetValue(o => o.ID);
                // 重载
                context.User.Update(userInfo, ID);
                // Where条件
                context.User.Where(o => o.ID == ID).Update(userInfo);
                // 批量
                var IDs = context.User.Desc(o => o.ID).ToSelectList(5, o => o.ID);
                context.User.Update(userInfo, IDs);

                var ID2 = context.Orders.Desc(o => o.ID).GetValue(o => o.ID);
                context.Orders.Update(new OrdersVO { ID = Guid.NewGuid(), OrderNo = "12345678-1", CreateAt = DateTime.Now, CreateName = "用户1" }, ID2);
                context.OrdersAt.Update(new OrdersVO { ID = Guid.NewGuid(), OrderNo = "12345678-2", CreateAt = DateTime.Now, CreateName = "用户1" }, ID2);
                context.OrdersBool.Update(new OrdersVO { ID = Guid.NewGuid(), OrderNo = "12345678-3", CreateAt = DateTime.Now, CreateName = "用户1" }, ID2);
                context.OrdersNum.Update(new OrdersVO { ID = Guid.NewGuid(), OrderNo = "12345678-4", CreateAt = DateTime.Now, CreateName = "用户1" }, ID2);

                context.SaveChanges();
            }
        }
    }
}