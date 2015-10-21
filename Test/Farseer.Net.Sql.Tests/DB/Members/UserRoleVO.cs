using FS.Infrastructure;
using FS.Sql.Map.Attribute;

namespace FS.Sql.Tests.DB.Members
{
    public class UserRoleVO : IEntity
    {
        [Field(IsPrimaryKey = true)]
        public int? ID { get; set; }

        public string Caption { get; set; }
        public string Descr { get; set; }
        public int? UserCount { get; set; }

        [Field(Name = "LevelNum")]
        public int? Level { get; set; }
    }
}