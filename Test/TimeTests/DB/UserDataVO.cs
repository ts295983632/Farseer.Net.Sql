using System;
using System.Collections.Generic;
using FS.Infrastructure;
using FS.Sql.Map.Attribute;
using System.Data;
using FS.Extends;

namespace FS.Sql.Tests.DB.Members
{
    public class UserDataVO : UserVO
    {
        public UserDataVO() { }
        public UserDataVO(DataRow dr)
        {
            //base.CreateAt = dr[nameof(CreateAt)].ConvertType(DateTime.Now);
            //base.GenderType = dr[nameof(GenderType)].ConvertType(eumGenderType.Man);
            //base.ID = dr[nameof(ID)].ConvertType(0);
            //base.LogCount = dr[nameof(LogCount)].ConvertType(0);
            //base.LoginIP = dr[nameof(LoginIP)].ConvertType("");
            //base.PassWord = dr[nameof(PassWord)].ConvertType("");
            //base.UserName = dr[nameof(UserName)].ConvertType("");


            base.CreateAt = dr[nameof(CreateAt)] as DateTime?;
            base.GenderType = dr[nameof(GenderType)] as eumGenderType?;
            base.ID = dr[nameof(ID)] as int?;
            base.LogCount = dr[nameof(LogCount)] as int?;
            base.LoginIP = dr[nameof(LoginIP)] as string;
            base.PassWord = dr[nameof(PassWord)] as string;
            base.UserName = dr[nameof(UserName)] as string;
        }
    }
}