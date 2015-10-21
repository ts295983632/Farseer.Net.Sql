using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using FS.Sql.ExpressionVisitor;
using FS.Sql.Infrastructure;
using FS.Sql.Map;
using FS.Utils.Common;

namespace FS.Sql.Client.OleDb
{
    /// <summary>
    ///     提供ExpressionBinary表达式树的解析
    /// </summary>
    public sealed class OleDbWhereVisitor : WhereVisitor
    {
        /// <summary>
        ///     Select筛选字段时表达式树的解析
        /// </summary>
        /// <param name="dbProvider">数据库提供者（不同数据库的特性）</param>
        /// <param name="map">字段映射</param>
        /// <param name="paramList">SQL参数列表</param>
        public OleDbWhereVisitor(AbsDbProvider dbProvider, SetDataMap map, List<DbParameter> paramList) : base(dbProvider, map, paramList)
        {
        }

        protected override void VisitMethodContains(MethodCallExpression m, Type fieldType, string fieldName, Type paramType, string paramName)
        {
            // 非List<>形式
            if (paramType != null && (!paramType.IsGenericType || paramType.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                #region 搜索值串的处理
                var param = ParamList.Find(o => o.ParameterName == paramName);
                if (param != null && Regex.IsMatch(param.Value.ToString(), @"[\d]+") && (Type.GetTypeCode(fieldType) == TypeCode.Int16 || Type.GetTypeCode(fieldType) == TypeCode.Int32 || Type.GetTypeCode(fieldType) == TypeCode.Decimal || Type.GetTypeCode(fieldType) == TypeCode.Double || Type.GetTypeCode(fieldType) == TypeCode.Int64 || Type.GetTypeCode(fieldType) == TypeCode.UInt16 || Type.GetTypeCode(fieldType) == TypeCode.UInt32 || Type.GetTypeCode(fieldType) == TypeCode.UInt64))
                {
                    param.Value = "," + param.Value + ",";
                    param.DbType = DbType.String;
                    if (DbProvider.KeywordAegis("").Length > 0) { fieldName = "','+" + fieldName.Substring(1, fieldName.Length - 2) + "+','"; }
                    else
                    { fieldName = "','+" + fieldName + "+','"; }
                }
                #endregion

                // 判断是不是字段调用Contains
                var isFieldCall = m.Object != null && m.Object.NodeType == ExpressionType.MemberAccess && ((MemberExpression)m.Object).Expression != null && ((MemberExpression)m.Object).Expression.NodeType == ExpressionType.Parameter;
                SqlList.Push(isFieldCall ? $"INSTR({fieldName},{paramName}) {(IsNot ? "<=" : ">")} 0" : $"INSTR({paramName},{fieldName}) {(IsNot ? "<=" : ">")} 0");
            }
            else
            {
                // 不使用参数化形式，同时移除参数
                var paramValue = CurrentDbParameter.Value;
                ParamList.RemoveAt(ParamList.Count - 1);

                // 字段是字符类型的，需要加入''符号
                if (Type.GetTypeCode(fieldType) == TypeCode.String) { paramValue = "'" + paramValue.ToString().Replace(",", "','") + "'"; }

                SqlList.Push($"{fieldName} {(IsNot ? "Not" : "")} IN ({paramValue})");
            }
        }
    }
}