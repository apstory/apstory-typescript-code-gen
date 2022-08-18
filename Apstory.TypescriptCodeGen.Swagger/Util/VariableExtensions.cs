namespace Apstory.TypescriptCodeGen.Swagger.Util
{
    public static class VariableExtensions
    {
        public static string ToTypeScriptVariable(string type, string format)
        {
            if (type == "int" || type == "integer" || type == "double" || type == "short" || type == "decimal")
                return "number";
            else if (type == "DateTime" || format == "date-time")
                return "Date";
            else if (type == "long" || type == "string" || type == "Guid")
                return "string";
            else if (type == "bool" || type == "boolean")
                return "boolean";

            return type;
        }
    }
}
