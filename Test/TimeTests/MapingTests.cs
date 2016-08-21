using FS.Extends;
using FS.Sql.Tests.DB;
using FS.Sql.Tests.DB.Members;
using FS.Utils.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TimeTests
{
    public class MapingTests
    {
        static DataTable dt;
        public static void Tests()
        {
            Console.WriteLine(typeof(UserDataVO).Namespace);
            new AssemblyName()
            System.Reflection.Assembly.LoadFrom("Farseer.Net.CodeDom");
            Init();
            AddData();
            SpeedTest.ConsoleTime("硬编码转换", 1, () =>
            {
                var lst = new List<UserDataVO>();
                foreach (DataRow item in dt.Rows)
                {
                    lst.Add(new UserDataVO(item));
                }
            });

            SpeedTest.ConsoleTime("表达式树委托转换", 1, () =>
            {
                dt.ToList<UserDataVO>();
            });
        }

        private static void Init()
        {
            dt = new DataTable();
            dt.Columns.Add(nameof(UserDataVO.CreateAt));
            dt.Columns.Add(nameof(UserDataVO.GenderType));
            dt.Columns.Add(nameof(UserDataVO.ID));
            dt.Columns.Add(nameof(UserDataVO.LogCount));
            dt.Columns.Add(nameof(UserDataVO.LoginIP));
            dt.Columns.Add(nameof(UserDataVO.PassWord));
            dt.Columns.Add(nameof(UserDataVO.UserName));
        }

        private static void AddData(int count = 10000)
        {
            for (int i = 0; i < count; i++)
            {
                dt.Rows.Add(DateTime.Now, eumGenderType.Man, i, Rand.GetRandom(), Rand.CreateRandomString(15), Rand.CreateRandomString(32), Rand.CreateRandomString(12));
            }
        }
    }
}
