using System;
using FS.Sql.Tests.DB;
using FS.Sql.Tests.DB.Members;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FS.Sql.Tests
{
    [TestClass]
    public class Insert
    {
        [TestMethod]
        public void Normal_Insert()
        {
            int ID;
            var userInfo = new UserVO() {UserName = "yy", GetDate = DateTime.Now};
            var insertUser = new InsertUserVO() {PassWord = "123xx", UserName = "steden"};

            // 将标识设置到标识字段
            Table.Data.User.Insert(userInfo, true);
            Assert.IsTrue(userInfo.ID > 0);

            // 设置out ID
            userInfo.ID++;
            Table.Data.User.Insert(userInfo, out ID);
            Assert.IsTrue(userInfo.ID > 0 && userInfo.ID == ID);

            // 缓存表
            Table.Data.UserRole.Insert(new UserRoleVO {Caption = "标题", Descr = "不清楚"});

            // 不同逻辑删除方式新入（主键为GUID）
            Table.Data.Orders.Insert(new OrdersVO {ID = Guid.NewGuid(), OrderNo = "12345678-1", CreateAt = DateTime.Now, CreateName = "用户1"});
            Table.Data.OrdersAt.Insert(new OrdersVO {ID = Guid.NewGuid(), OrderNo = "12345678-2", CreateAt = DateTime.Now, CreateName = "用户1"});
            Table.Data.OrdersBool.Insert(new OrdersVO {ID = Guid.NewGuid(), OrderNo = "12345678-3", CreateAt = DateTime.Now, CreateName = "用户1"});
            Table.Data.OrdersNum.Insert(new OrdersVO {ID = Guid.NewGuid(), OrderNo = "12345678-4", CreateAt = DateTime.Now, CreateName = "用户1"});
            Table.Data.OrdersBoolCache.Insert(new OrdersVO {ID = Guid.NewGuid(), OrderNo = "12345678-5", CreateAt = DateTime.Now, CreateName = "用户1"});

            // 存储过程
            Table.Data.InsertUser.Execute(insertUser);
            Assert.IsTrue(insertUser.ID > 0);

            // 配置SQL
            Table.Data.InsertNewUser.Execute();

            // 手动SQL
            var table = Table.Data;
            table.ManualSql.Execute("INSERT INTO Members_User (UserName,PassWord) VALUES(@UserName,@PassWord)", table.DbProvider.CreateDbParam("UserName", "steden1"), table.DbProvider.CreateDbParam("PassWord", "steden1"));
        }

        [TestMethod]
        public void Transaction_Insert()
        {
            using (var context = new Table())
            {
                int ID;
                var userInfo = new UserVO() {UserName = "yy", GetDate = DateTime.Now};
                var insertUser = new InsertUserVO() {PassWord = "123xx", UserName = "steden"};

                // 将标识设置到标识字段
                context.User.Insert(userInfo, true);
                Assert.IsTrue(userInfo.ID > 0);

                // 设置out ID
                userInfo.ID++;
                context.User.Insert(userInfo, out ID);
                Assert.IsTrue(userInfo.ID > 0 && userInfo.ID == ID);

                // 缓存表
                context.UserRole.Insert(new UserRoleVO {Caption = "标题", Descr = "不清楚"});

                // 不同逻辑删除方式新入（主键为GUID）
                context.Orders.Insert(new OrdersVO {ID = Guid.NewGuid(), OrderNo = "12345678-1", CreateAt = DateTime.Now, CreateName = "用户1"});
                context.OrdersAt.Insert(new OrdersVO {ID = Guid.NewGuid(), OrderNo = "12345678-2", CreateAt = DateTime.Now, CreateName = "用户1"});
                context.OrdersBool.Insert(new OrdersVO {ID = Guid.NewGuid(), OrderNo = "12345678-3", CreateAt = DateTime.Now, CreateName = "用户1"});
                context.OrdersNum.Insert(new OrdersVO {ID = Guid.NewGuid(), OrderNo = "12345678-4", CreateAt = DateTime.Now, CreateName = "用户1"});
                context.OrdersBoolCache.Insert(new OrdersVO {ID = Guid.NewGuid(), OrderNo = "12345678-5", CreateAt = DateTime.Now, CreateName = "用户1"});

                // 存储过程
                Table.TransactionInstance(context).InsertUser.Execute(insertUser);
                Assert.IsTrue(insertUser.ID > 0);

                // 配置SQL
                Table.TransactionInstance(context).InsertNewUser.Execute();

                // 手动SQL
                var table = Table.TransactionInstance(context);
                table.ManualSql.Execute("INSERT INTO Members_User (UserName,PassWord) VALUES(@UserName,@PassWord)", table.DbProvider.CreateDbParam("UserName", "steden1"), table.DbProvider.CreateDbParam("PassWord", "steden1"));

                context.SaveChanges();
            }
        }
    }
}