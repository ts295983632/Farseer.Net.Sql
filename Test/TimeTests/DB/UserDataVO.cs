using System;
using System.Data;
using System.Collections.Generic;
using FS.Utils.Common;
using FS.Extends;
namespace FS.Sql.Tests.DB.Members
{
    public class UserVOByDataRow : FS.Sql.Tests.DB.Members.UserVO
    {
        public static FS.Sql.Tests.DB.Members.UserVO ConvertDataRow(DataRow dr)
        {
            var entity = new FS.Sql.Tests.DB.Members.UserVO();
            if (dr.Table.Columns.Contains("ID") && dr["ID"] != null)
            {
                System.Int32 ID_Out; if (System.Int32.TryParse(dr["ID"].ToString(), out ID_Out)) { entity.ID = ID_Out; }
            }
            if (dr.Table.Columns.Contains("UserName") && dr["UserName"] != null)
            {
                entity.UserName = dr["UserName"].ToString();
            }
            if (dr.Table.Columns.Contains("PassWord") && dr["PassWord"] != null)
            {
                entity.PassWord = dr["PassWord"].ToString();
            }
            if (dr.Table.Columns.Contains("GenderType") && dr["GenderType"] != null)
            {
                entity.GenderType = (eumGenderType)dr["GenderType"].ConvertType(typeof(DB.eumGenderType));
            }
            if (dr.Table.Columns.Contains("LoginCount") && dr["LoginCount"] != null)
            {
                System.Int32 LoginCount_Out; if (System.Int32.TryParse(dr["LoginCount"].ToString(), out LoginCount_Out)) { entity.LogCount = LoginCount_Out; }
            }
            if (dr.Table.Columns.Contains("SiteIDs") && dr["SiteIDs"] != null)
            {
                entity.SiteIDs = ConvertHelper.ConvertType(dr["SiteIDs"], typeof(List<System.Int32>)) as List<System.Int32>;
            }
            if (dr.Table.Columns.Contains("LoginIP") && dr["LoginIP"] != null)
            {
                entity.LoginIP = dr["LoginIP"].ToString();
            }
            if (dr.Table.Columns.Contains("GetDate") && dr["GetDate"] != null)
            {
                System.DateTime GetDate_Out; if (System.DateTime.TryParse(dr["GetDate"].ToString(), out GetDate_Out)) { entity.GetDate = GetDate_Out; }
            }
            if (dr.Table.Columns.Contains("CreateAt") && dr["CreateAt"] != null)
            {
                System.DateTime CreateAt_Out; if (System.DateTime.TryParse(dr["CreateAt"].ToString(), out CreateAt_Out)) { entity.CreateAt = CreateAt_Out; }
            }
            if (dr.Table.Columns.Contains("UserRole") && dr["UserRole"] != null)
            {
            }
            if (dr.Table.Columns.Contains("OrderID") && dr["OrderID"] != null)
            {
                entity.Orders = ConvertHelper.ConvertType(dr["OrderID"], typeof(List<FS.Sql.Tests.DB.Members.OrdersVO>)) as List<FS.Sql.Tests.DB.Members.OrdersVO>;
            }
            return entity;
        }

        public static List<FS.Sql.Tests.DB.Members.UserVO> ConvertDataTable(DataTable dt)
        {
            var lst = new List<FS.Sql.Tests.DB.Members.UserVO>(dt.Rows.Count);
            foreach (DataRow dr in dt.Rows)
            {
                lst.Add(ConvertDataRow(dr));
            }
            return lst;
        }

        public static FS.Sql.Tests.DB.Members.UserVO ConvertDataReader(IDataReader reader)
        {
            var entity = new FS.Sql.Tests.DB.Members.UserVO();
            var isHaveValue = false;
            if (reader.Read())
            {
                isHaveValue = true;
                if (reader.HaveName("ID"))
                {
                    System.Int32 ID_Out; if (System.Int32.TryParse(reader["ID"].ToString(), out ID_Out)) { entity.ID = ID_Out; }
                }
                if (reader.HaveName("UserName"))
                {
                    entity.UserName = reader["UserName"].ToString();
                }
                if (reader.HaveName("PassWord"))
                {
                    entity.PassWord = reader["PassWord"].ToString();
                }
                if (reader.HaveName("GenderType"))
                {
                    entity.GenderType = (FS.Sql.Tests.DB.eumGenderType)reader["GenderType"].ConvertType(typeof(FS.Sql.Tests.DB.eumGenderType));
                }
                if (reader.HaveName("LoginCount"))
                {
                    System.Int32 LoginCount_Out; if (System.Int32.TryParse(reader["LoginCount"].ToString(), out LoginCount_Out)) { entity.LogCount = LoginCount_Out; }
                }
                if (reader.HaveName("SiteIDs"))
                {
                    entity.SiteIDs = ConvertHelper.ConvertType(reader["SiteIDs"], typeof(List<System.Int32>)) as List<System.Int32>;
                }
                if (reader.HaveName("LoginIP"))
                {
                    entity.LoginIP = reader["LoginIP"].ToString();
                }
                if (reader.HaveName("GetDate"))
                {
                    System.DateTime GetDate_Out; if (System.DateTime.TryParse(reader["GetDate"].ToString(), out GetDate_Out)) { entity.GetDate = GetDate_Out; }
                }
                if (reader.HaveName("CreateAt"))
                {
                    System.DateTime CreateAt_Out; if (System.DateTime.TryParse(reader["CreateAt"].ToString(), out CreateAt_Out)) { entity.CreateAt = CreateAt_Out; }
                }
                if (reader.HaveName("UserRole"))
                {

                }
                if (reader.HaveName("OrderID"))
                {
                    entity.Orders = ConvertHelper.ConvertType(reader["OrderID"], typeof(List<FS.Sql.Tests.DB.Members.OrdersVO>)) as List<FS.Sql.Tests.DB.Members.OrdersVO>;
                }
            }
            return isHaveValue ? entity : null;
        }

    }
}
