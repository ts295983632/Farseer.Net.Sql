using FS.Sql.Infrastructure;

namespace FS.Sql.Client.Oracle
{
    public class OracleFunctionProvider : AbsFunctionProvider
    {
        public override string CharIndex(string fieldName, string paramName, bool isNot) => $"INSTR({fieldName},{paramName}) {(isNot ? "<=" : ">")} 0";
        public override string StartsWith(string fieldName, string paramName, bool isNot) => $"INSTR({fieldName},{paramName}) {(isNot ? ">" : "=")} 1";
    }
}