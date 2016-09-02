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

            var mapData = ConvertHelper.DataTableToDictionary(dt);

            for (int i = 0; i < 5; i++)
            {
                ConvertDataTable();
                SpeedTest.ConsoleTime("手动实现转换：DataTable转实体", 1, ConvertDataTable);
                ExpressionConvertDataTable();
                SpeedTest.ConsoleTime("表达式树委托：DataTable转实体", 1, ExpressionConvertDataTable);
                AutoConvertDataTable();
                SpeedTest.ConsoleTime("动态编译转换：DataTable转实体", 1, AutoConvertDataTable);
            }
        }


        private static void ConvertDataTable()
        {
            var mapData = ConvertHelper.DataTableToDictionary(dt);
            var lst = UserVOByDataRow.ToList(mapData);
        }
        private static void ExpressionConvertDataTable()
        {
            var lst = new List<UserVO>(dt.Rows.Count);
            foreach (DataRow item in dt.Rows) { lst.Add(ToInfo<UserVO>(item)); }
        }
        private static void AutoConvertDataTable()
        {
            var lst = dt.ToList<UserVO>();
        }

        private static void Init()
        {
            dt = new DataTable();
            dt.Columns.Add(new DataColumn(nameof(UserVO.CreateAt), typeof(DateTime)));
            dt.Columns.Add(new DataColumn(nameof(UserVO.GenderType), typeof(byte)));
            dt.Columns.Add(new DataColumn(nameof(UserVO.ID), typeof(int)));
            dt.Columns.Add(new DataColumn("LoginCount", typeof(int)));
            dt.Columns.Add(new DataColumn(nameof(UserVO.LoginIP), typeof(string)));
            dt.Columns.Add(new DataColumn(nameof(UserVO.PassWord), typeof(string)));
            dt.Columns.Add(new DataColumn(nameof(UserVO.UserName), typeof(string)));
            //dt.Columns.Add(new DataColumn(nameof(UserVO.SiteIDs), typeof(string)));
        }

        private static void AddData(int count = 50000)
        {
            for (var i = 0; i < count; i++)
            {
                var lst = new List<int> { Rand.GetRandom(), Rand.GetRandom(), Rand.GetRandom() };
                dt.Rows.Add(DateTime.Now, i % 2 == 0 ? eumGenderType.Man : eumGenderType.Woman, i, Rand.GetRandom(), Rand.CreateRandomString(15), Rand.CreateRandomString(32), Rand.CreateRandomString(12)/*, lst.ToString(",")*/);
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
