using System.Data.Common;
using System.Reflection;
using FS.Cache;
using FS.Sql.Infrastructure;
using FS.Sql.Internal;

namespace FS.Sql.Client.PostgreSql
{
    public class PostgreSqlFunctionProvider : AbsFunctionProvider
    {
        public override string CharIndex(string fieldName, string paramName, bool isNot) => $"POSITION({paramName} IN {fieldName}) {(isNot ? "<=" : ">")} 0";
        public override string StartsWith(string fieldName, string paramName, bool isNot) => $"POSITION({paramName} IN {fieldName}) {(isNot ? ">" : "=")} 1";
        public override string Len(string fieldName) => $"bit_length({fieldName})";
    }
}