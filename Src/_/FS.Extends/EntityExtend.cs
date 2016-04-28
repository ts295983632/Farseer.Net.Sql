using System;
using System.Collections.Generic;
using FS.Cache;
using FS.Infrastructure;
using FS.Sql;
using FS.Sql.Infrastructure;
using FS.Utils.Common;

// ReSharper disable once CheckNamespace

namespace FS.Extends
{
    public static partial class SqlExtend
    {
        /// <summary>
        ///     检测实体类值状况
        /// </summary>
        public static bool Check<TEntity>(this TEntity info, Action<string, string> tip = null, string url = "") where TEntity : IVerification
        {
            if (info == null) { return false; }
            //if (tip == null) { tip = new Terminator().Alert; }
            //返回错误
            Dictionary<string, List<string>> dicError;
            var result = info.Check(out dicError);

            if (!result)
            {
                var lst = new List<string>();
                foreach (var item in dicError) { lst.AddRange(item.Value); }

                tip(lst.ToString("<br />"), url);
            }
            return result;
        }

        /// <summary>
        ///     检测实体类值状况
        /// </summary>
        public static bool Check<TEntity>(this TEntity info, Action<Dictionary<string, List<string>>> tip) where TEntity : IVerification
        {
            //返回错误
            Dictionary<string, List<string>> dicError;
            var result = info.Check(out dicError);

            if (!result) { tip(dicError); }
            return result;
        }

        /// <summary>
        ///     检测实体类值状况
        /// </summary>
        /// <param name="dicError">返回错误消息,key：属性名称；vakue：错误消息</param>
        /// <param name="entity">要检测的实体</param>
        public static bool Check<TEntity>(this TEntity entity, out Dictionary<string, List<string>> dicError) where TEntity : IVerification
        {
            dicError = new Dictionary<string, List<string>>();
            var map = SetMapCacheManger.Cache(typeof(TEntity));
            foreach (var kic in map.MapList)
            {
                var lstError = new List<string>();
                var value = PropertyGetCacheManger.Cache(kic.Key, entity);
                // 是否必填
                if (kic.Value.Required != null && !kic.Value.Required.IsValid(value)) { lstError.Add(kic.Value.Required.ErrorMessage); }

                //if (value == null) { continue; }

                // 字符串长度判断
                if (kic.Value.StringLength != null && !kic.Value.StringLength.IsValid(value)) { lstError.Add(kic.Value.StringLength.ErrorMessage); }

                // 值的长度
                if (kic.Value.Range != null && !kic.Value.Range.IsValid(value)) { lstError.Add(kic.Value.Range.ErrorMessage); }

                // 正则
                if (kic.Value.RegularExpression != null && !kic.Value.RegularExpression.IsValid(value)) { lstError.Add(kic.Value.RegularExpression.ErrorMessage); }

                if (lstError.Count > 0) { dicError.Add(kic.Key.Name, lstError); }
            }
            return dicError.Count == 0;
        }

        /// <summary>
        ///     检测实体类值状况
        /// </summary>
        /// <param name="result">返返回结果情况</param>
        /// <param name="entity">要检测的实体</param>
        public static bool Check<TEntity>(this TEntity entity, Result result) where TEntity : IVerification
        {
            //返回错误
            Dictionary<string, List<string>> dicError;
            var isError = entity.Check(out dicError);

            if (!isError) { result.Add(dicError); }
            return isError;
        }
    }
}