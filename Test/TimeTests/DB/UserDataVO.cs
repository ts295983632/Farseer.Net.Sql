using System;
using System.Collections.Generic;
using System.Data;
using FS.Sql.Tests.DB;
using FS.Sql.Tests.DB.Members;
using FS.Utils.Common;

namespace TimeTests.DB
{
    public class UserDataVO : UserVO
    {
        public UserDataVO() { }
        public UserDataVO(DataRow dr)
        {
            if (dr.Table.Columns.Contains("SiteIDs") && dr["SiteIDs"] != null) { base.SiteIDs = ConvertHelper.ConvertType(dr["SiteIDs"], typeof(List<System.Int32>)) as List<System.Int32>; }

            DateTime createAtOut;
            if (dr[nameof(CreateAt)] != null && DateTime.TryParse(dr[nameof(CreateAt)].ToString(), out createAtOut)) { base.CreateAt = createAtOut; }

            eumGenderType genderTypeOut;
            if (dr[nameof(GenderType)] != null && Enum.TryParse(dr[nameof(GenderType)].ToString(), out genderTypeOut)) { base.GenderType = genderTypeOut; }

            Int32 idOut;
            if (dr[nameof(ID)] != null && Int32.TryParse(dr[nameof(ID)].ToString(), out idOut)) { base.ID = idOut; }

            Int32 logCountOut;
            if (dr[nameof(LogCount)] != null && Int32.TryParse(dr[nameof(LogCount)].ToString(), out logCountOut)) { base.LogCount = logCountOut; }

            base.LoginIP = dr["LoginIP"] as string;
            base.PassWord = dr["PassWord"] as string;
            base.UserName = dr[nameof(UserName)] as string;
        }
    }
}