using FS.Sql.Map.Attribute;

namespace FS.Sql.Tests.DB.Members
{
    public class ValueUserVO
    {
        /// <summary>
        ///     用户ID
        /// </summary>
        [Field(IsInParam = true)]
        public int? ID { get; set; }
    }
}