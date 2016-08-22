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
using FS.Cache;
using FS.Sql.Internal;
using TimeTests.DB;

namespace TimeTests
{
    public class MapingTests
    {
        static DataTable dt;
        public static void Tests()
        {
            Init();
            AddData();
            new UserDataVO(dt.Rows[0]);
            SpeedTest.ConsoleTime("硬编码DataTable转实体", 1, () =>
            {
                var lst = new List<UserVO>(dt.Rows.Count);
                foreach (DataRow item in dt.Rows)
                {
                    lst.Add(new UserDataVO(item));
                }
            });
            dt.ToList<UserVO>();
            SpeedTest.ConsoleTime("表达式树委托DataTable转实体", 1, () =>
            {
                var lst = dt.ToList<UserVO>();
            });
        }

        private static void Init()
        {
            dt = new DataTable();
            dt.Columns.Add(nameof(UserVO.CreateAt));
            dt.Columns.Add(nameof(UserVO.GenderType));
            dt.Columns.Add(nameof(UserVO.ID));
            dt.Columns.Add(nameof(UserVO.LogCount));
            dt.Columns.Add(nameof(UserVO.LoginIP));
            dt.Columns.Add(nameof(UserVO.PassWord));
            dt.Columns.Add(nameof(UserVO.UserName));
            dt.Columns.Add(nameof(UserVO.SiteIDs));
        }

        private static void AddData(int count = 10000)
        {
            for (int i = 0; i < count; i++)
            {
                var lst = new List<int> {Rand.GetRandom(), Rand.GetRandom(), Rand.GetRandom()};
                dt.Rows.Add(DateTime.Now, eumGenderType.Man, i, Rand.GetRandom(), Rand.CreateRandomString(15), Rand.CreateRandomString(32), Rand.CreateRandomString(12), lst.ToString(","));
            }
        }
    }
}
