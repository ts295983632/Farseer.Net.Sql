using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using FS.Cache;
using FS.Sql.Map.Attribute;

namespace FS.Sql.Map
{
    /// <summary>
    ///     字段 映射关系
    /// </summary>
    public class SetPhysicsMap
    {
        /// <summary>
        ///     获取所有Set属性
        /// </summary>
        public readonly Dictionary<PropertyInfo, FieldMapState> MapList;

        /// <summary>
        ///     关系映射
        /// </summary>
        /// <param name="type">实体类Type</param>
        public SetPhysicsMap(Type type)
        {
            Type = type;
            MapList = new Dictionary<PropertyInfo, FieldMapState>();
            PrimaryFields = new Dictionary<PropertyInfo, FieldAttribute>();

            // 循环Set的字段
            foreach (var propertyInfo in Type.GetProperties())
            {
                #region 获取字段已标记的特性

                var modelAtt = new FieldMapState();
                var attrs = propertyInfo.GetCustomAttributes(false);
                foreach (var item in attrs)
                {
                    // 字段
                    if (item is FieldAttribute)
                    {
                        modelAtt.Field = (FieldAttribute) item;
                        continue;
                    }
                    // 字符串长度
                    if (item is StringLengthAttribute)
                    {
                        modelAtt.StringLength = (StringLengthAttribute) item;
                        continue;
                    }
                    // 是否必填
                    if (item is RequiredAttribute)
                    {
                        modelAtt.Required = (RequiredAttribute) item;
                        continue;
                    }
                    // 字段描述
                    if (item is DisplayAttribute)
                    {
                        modelAtt.Display = (DisplayAttribute) item;
                        continue;
                    }
                    // 值的长度
                    if (item is RangeAttribute)
                    {
                        modelAtt.Range = (RangeAttribute) item;
                        continue;
                    }
                    // 正则
                    if (item is RegularExpressionAttribute)
                    {
                        modelAtt.RegularExpression = (RegularExpressionAttribute) item;
                        continue;
                    }
                }

                #endregion

                #region 初始化字段映射

                if (modelAtt.Field == null) { modelAtt.Field = new FieldAttribute {Name = propertyInfo.Name}; }
                if (string.IsNullOrEmpty(modelAtt.Field.Name)) { modelAtt.Field.Name = propertyInfo.Name; }
                if (modelAtt.Field.IsMap)
                {
                    // 主键
                    if (modelAtt.Field.IsPrimaryKey) { PrimaryFields[propertyInfo] = modelAtt.Field; }
                    // 标识
                    if (modelAtt.Field.IsDbGenerated) { DbGeneratedFields = new KeyValuePair<PropertyInfo, FieldAttribute>(propertyInfo, modelAtt.Field); }
                }

                #endregion

                #region 初始化字段描述映射

                if (modelAtt.Display == null) { modelAtt.Display = new DisplayAttribute {Name = propertyInfo.Name}; }
                if (string.IsNullOrEmpty(modelAtt.Display.Name)) { modelAtt.Display.Name = propertyInfo.Name; }

                #endregion

                #region 加入智能错误显示消息

                // 是否必填
                if (modelAtt.Required != null && string.IsNullOrEmpty(modelAtt.Required.ErrorMessage)) { modelAtt.Required.ErrorMessage = string.Format("{0}，不能为空！", modelAtt.Display.Name); }

                // 字符串长度判断
                if (modelAtt.StringLength != null && string.IsNullOrEmpty(modelAtt.StringLength.ErrorMessage))
                {
                    if (modelAtt.StringLength.MinimumLength > 0 && modelAtt.StringLength.MaximumLength > 0) { modelAtt.StringLength.ErrorMessage = string.Format("{0}，长度范围必须为：{1} - {2} 个字符之间！", modelAtt.Display.Name, modelAtt.StringLength.MinimumLength, modelAtt.StringLength.MaximumLength); }
                    else if (modelAtt.StringLength.MaximumLength > 0) { modelAtt.StringLength.ErrorMessage = string.Format("{0}，长度不能大于{1}个字符！", modelAtt.Display.Name, modelAtt.StringLength.MaximumLength); }
                    else
                    { modelAtt.StringLength.ErrorMessage = string.Format("{0}，长度不能小于{1}个字符！", modelAtt.Display.Name, modelAtt.StringLength.MinimumLength); }
                }

                // 值的长度
                if (modelAtt.Range != null && string.IsNullOrEmpty(modelAtt.Range.ErrorMessage))
                {
                    decimal minnum;
                    decimal.TryParse(modelAtt.Range.Minimum.ToString(), out minnum);
                    decimal maximum;
                    decimal.TryParse(modelAtt.Range.Minimum.ToString(), out maximum);

                    if (minnum > 0 && maximum > 0) { modelAtt.Range.ErrorMessage = string.Format("{0}，的值范围必须为：{1} - {2} 之间！", modelAtt.Display.Name, minnum, maximum); }
                    else if (maximum > 0) { modelAtt.Range.ErrorMessage = string.Format("{0}，的值不能大于{1}！", modelAtt.Display.Name, maximum); }
                    else
                    { modelAtt.Range.ErrorMessage = string.Format("{0}，的值不能小于{1}！", modelAtt.Display.Name, minnum); }
                }

                #endregion

                MapList.Add(propertyInfo, modelAtt);
            }
        }

        /// <summary>
        ///     设置了主键的字段列表
        /// </summary>
        public Dictionary<PropertyInfo, FieldAttribute> PrimaryFields { get; }

        /// <summary>
        ///     设置了标识的字段
        /// </summary>
        public KeyValuePair<PropertyInfo, FieldAttribute> DbGeneratedFields { get; private set; }

        /// <summary>
        ///     类型
        /// </summary>
        internal Type Type { get; set; }

        /// <summary>
        ///     通过实体类型，返回Mapping
        /// </summary>
        public static implicit operator SetPhysicsMap(Type type)
        {
            return SetMapCacheManger.Cache(type);
        }

        /// <summary>
        ///     获取当前属性（通过使用的fieldName）
        /// </summary>
        /// <param name="fieldName">属性名称</param>
        public KeyValuePair<PropertyInfo, FieldMapState> GetState(string fieldName)
        {
            return string.IsNullOrEmpty(fieldName) ? MapList.FirstOrDefault(o => o.Value.Field.IsPrimaryKey) : MapList.FirstOrDefault(o => o.Key.Name == fieldName);
        }
    }
}