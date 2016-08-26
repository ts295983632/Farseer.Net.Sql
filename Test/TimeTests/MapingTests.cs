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

namespace TimeTests
{
    public class MapingTests
    {
        static DataTable dt;
        public static void Tests()
        {
            Init();
            AddData();

            UserVOByDataRow.ConvertDataRow(dt.Rows[0]);
            SpeedTest.ConsoleTime("手动实现转换：DataTable转实体", 100, () =>
            {
                var lst = UserVOByDataRow.ConvertDataTable(dt);
            });
            SpeedTest.ConsoleTime("表达式树委托：DataTable转实体", 100, () =>
            {
                var lst = new List<UserVO>(dt.Rows.Count);
                foreach (DataRow item in dt.Rows)
                {
                    lst.Add(ToInfo<UserVO>(item));
                }
            });
            dt.ToList<UserVO>();
            SpeedTest.ConsoleTime("动态编译转换：DataTable转实体", 100, () =>
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

        private static void AddData(int count = 100)
        {
            for (int i = 0; i < count; i++)
            {
                var lst = new List<int> { Rand.GetRandom(), Rand.GetRandom(), Rand.GetRandom() };
                dt.Rows.Add(DateTime.Now, eumGenderType.Man, i, Rand.GetRandom(), Rand.CreateRandomString(15), Rand.CreateRandomString(32), Rand.CreateRandomString(12), lst.ToString(","));
            }
        }

        /// <summary>
        ///     将DataRow转成实体类
        /// </summary>
        /// <typeparam name="TEntity">实体类</typeparam>
        /// <param name="dr">源DataRow</param>
        public static TEntity ToInfo<TEntity>(DataRow dr) where TEntity : class, new()
        {
            var map = SetMapCacheManger.Cache(typeof(TEntity));
            var t = new TEntity();

            //赋值字段
            foreach (var kic in map.MapList)
            {
                if (!kic.Key.CanWrite) { continue; }
                var filedName = kic.Value.Field.IsFun ? kic.Key.Name : kic.Value.Field.Name;
                if (dr.Table.Columns.Contains(filedName))
                {
                    var oVal = dr[filedName].ConvertType(kic.Key.PropertyType);
                    if (oVal == null) { continue; }
                    PropertySetCacheManger.Cache(kic.Key, t, oVal);
                }
            }
            return t;
        }
    }
}
