using System;
using System.Collections.Generic;
using FS.Utils.Common;
using FS.Infrastructure;
namespace FS.Sql.Tests.DB.Members
{
    public class UserVOByDataRow : FS.Sql.Tests.DB.Members.UserVO
    {

        public static List<FS.Sql.Tests.DB.Members.UserVO> ToList(MapingData[] mapData)
        {
            var lst = new List<FS.Sql.Tests.DB.Members.UserVO>(mapData[0].DataList.Count);
            for (int i = 0; i < mapData[0].DataList.Count; i++) { lst.Add(ToEntity(mapData, i)); }
            return lst;
        }

        public static FS.Sql.Tests.DB.Members.UserVO ToEntity(MapingData[] mapData, int rowsIndex = 0)
        {
            var entity = new FS.Sql.Tests.DB.Members.UserVO();
            foreach (var map in mapData)
            {
                var col = map.DataList[rowsIndex];
                if (col == null) { continue; }
                switch (map.ColumnName)
                {
                    case "ID":
                        if (col is System.Int32) { entity.ID = (System.Int32)col; } else { System.Int32 ID_Out; if (System.Int32.TryParse(col.ToString(), out ID_Out)) { entity.ID = ID_Out; } }
                        break;
                    case "UserName":
                        entity.UserName = col.ToString(); break;
                    case "PassWord":
                        entity.PassWord = col.ToString(); break;
                    case "GenderType":
                        if (typeof(FS.Sql.Tests.DB.eumGenderType).GetEnumUnderlyingType() == col.GetType()) { entity.GenderType = (FS.Sql.Tests.DB.eumGenderType)col; } else { FS.Sql.Tests.DB.eumGenderType GenderType_Out; if (Enum.TryParse(col.ToString(), out GenderType_Out)) { entity.GenderType = GenderType_Out; } }
                        break;
                    case "LoginCount":
                        if (col is System.Int32) { entity.LogCount = (System.Int32)col; } else { System.Int32 LoginCount_Out; if (System.Int32.TryParse(col.ToString(), out LoginCount_Out)) { entity.LogCount = LoginCount_Out; } }
                        break;
                    case "SiteIDs":
                        entity.SiteIDs = ConvertHelper.ToList<System.Int32>(col.ToString()); break;
                    case "LoginIP":
                        entity.LoginIP = col.ToString(); break;
                    case "GetDate":
                        if (col is System.DateTime) { entity.GetDate = (System.DateTime)col; } else { System.DateTime GetDate_Out; if (System.DateTime.TryParse(col.ToString(), out GetDate_Out)) { entity.GetDate = GetDate_Out; } }
                        break;
                    case "CreateAt":
                        if (col is System.DateTime) { entity.CreateAt = (System.DateTime)col; } else { System.DateTime CreateAt_Out; if (System.DateTime.TryParse(col.ToString(), out CreateAt_Out)) { entity.CreateAt = CreateAt_Out; } }
                        break;
                    case "UserRole":
                        break;
                    case "OrderID":
                        entity.Orders = ConvertHelper.ToList<FS.Sql.Tests.DB.Members.OrdersVO>(col.ToString()); break;

                }
            }
            return entity;
        }
    }
}
