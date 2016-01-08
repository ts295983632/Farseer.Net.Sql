using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using FS.Cache;
using FS.Configs;
using FS.Extends;
using FS.Sql;
using FS.Sql.Tests.DB;
using FS.Sql.Tests.DB.Members;
using FS.Utils;
using FS.Utils.Common;
using FS.Utils.Component;

namespace TimeTests
{
    static class Program
    {
        static void Main(string[] args)
        {
            Table.Data.Orders.Delete();
            TestInsert2(1000);
            Console.ReadKey();
            Console.WriteLine(Table.Data.Orders.Count());
        }
        static void TestTableInstance(int count = 10000)
        {
            var tableType = typeof(Table);

            var type = typeof(TableSet<UserVO>);

            SpeedTest.ConsoleTime("手动创建", count, () =>
            {
                var table = new Table();
            });
            SpeedTest.ConsoleTime("表达式树创建", count, () =>
            {
                var table = (Table)InstanceCacheManger.Cache(tableType);
            });
            SpeedTest.ConsoleTime("反射创建", count, () =>
            {
                var table = (Table)Activator.CreateInstance(tableType);
            });
        }
        static void TestSetValueCache(int count = 10000000)
        {
            var user = new UserVO();
            var propertyInfo = user.GetType().GetProperty("UserName");
            SpeedTest.ConsoleTime("手动赋值", count, () =>
            {
                user.UserName = "jj";
            });
            SpeedTest.ConsoleTime("表达式树赋值", count, () =>
            {
                PropertySetCacheManger.Cache(propertyInfo, user, "jj");
            });
            SpeedTest.ConsoleTime("反射赋值", count, () =>
            {
                propertyInfo.SetValue(user, "jj", null);
            });
        }
        static void TestGetValueCache(int count = 1000000)
        {
            var user = new UserVO();
            var propertyInfo = user.GetType().GetProperty("UserName");
            SpeedTest.ConsoleTime("手动取值", count, () =>
            {
                var a = user.UserName;
            });
            SpeedTest.ConsoleTime("表达式树取值", count, () =>
            {
                var a = PropertyGetCacheManger.Cache(propertyInfo, user);
            });
            SpeedTest.ConsoleTime("反射取值", count, () =>
            {
                var a = propertyInfo.GetValue(user, null);
            });
        }
        static void TestToList(int count = 100000)
        {
            var dt = Table.Data.User.ToTable();
            for (var i = 0; i < count; i++) { dt.Rows.Add(1, "xxxxxx", "ffffffffff", 0, 0, "xxxx", 0, DateTime.Now); }

            SpeedTest.ConsoleTime("自动赋值", 1, () => { dt.ToList<UserVO>(); });
            SpeedTest.ConsoleTime("手动赋值", 1, () =>
            {
                var lst = new List<UserVO>();
                foreach (DataRow row in dt.Rows)
                {
                    var info = new UserVO
                    {
                        ID = row["ID"].ConvertType(0),
                        UserName = row["UserName"].ConvertType(""),
                        PassWord = row["PassWord"].ConvertType(""),
                        GenderType = row["GenderType"].ConvertType(eumGenderType.Man),
                        LogCount = row["LoginCount"].ConvertType(0),
                        LoginIP = row["LoginIP"].ConvertType(""),
                        CreateAt = row["CreateAt"].ConvertType(DateTime.Now),
                    };
                    lst.Add(info);
                }
            });
        }
        static void TestDynamicClass(int count = 100)
        {
            var val = 1;
            Expression<Func<UserVO, object>> exp = o => o.ID;
            Expression<Func<UserVO, object>> exp2 = o => new { o.ID, LoginCount = o.LogCount };

            var result = ExpressionHelper.MergeAssignExpression(exp2, val);

        }
        static void TestDicKey(int count = 10000000)
        {
            var type = typeof(Program);
            var fName = type.FullName;
            var hashCode = type.GetHashCode();

            var dicString = new Dictionary<string, object>();
            dicString[fName] = null;
            SpeedTest.ConsoleTime("Dictionary<string, object>", count, () =>
            {
                var a = dicString[fName];
            });


            var dicType = new Dictionary<Type, object>();
            dicType[type] = null;
            SpeedTest.ConsoleTime("Dictionary<Type, object>", count, () =>
            {
                var a = dicType[type];
            });


            var dicInt = new Dictionary<int, object>();
            dicInt[hashCode] = null;
            SpeedTest.ConsoleTime("Dictionary<int, object>", count, () =>
            {
                var a = dicInt[hashCode];
            });
        }

        static void TestInsert(int count = 10000000)
        {
            for (int i = 0; i < count; i++)
            {
                Console.WriteLine(i);
                var a = Table.Data.Orders.Insert(new OrdersVO { ID = Guid.NewGuid(), OrderNo = "1-" + i.ToString(), CreateAt = DateTime.Now, CreateName = "用户1" });
            }
        }
        static void TestInsert2(int count = 10000000)
        {
            for (int i = 0; i < count; i++)
            {
                var db = new Table();
                db.CancelTransaction();
                //using (var db = new Table())
                {
                    Console.WriteLine(i);
                    var a = db.Orders.Insert(new OrdersVO { ID = Guid.NewGuid(), OrderNo = "1-" + i.ToString(), CreateAt = DateTime.Now, CreateName = "用户1" });
                    //db.SaveChanges();
                }
            }
        }
    }
}
