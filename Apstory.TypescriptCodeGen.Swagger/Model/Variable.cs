using Apstory.TypescriptCodeGen.Swagger.Util;

namespace Apstory.TypescriptCodeGen.Swagger.Model
{
    public class Variable
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool Nullable { get; set; }
        public bool IsArray { get; set; }

        public Variable(string name, string type, string format, bool nullable, bool isArray)
        {
            this.Name = name;
            this.Nullable = nullable;
            this.IsArray = isArray;
            this.Type = VariableExtensions.ToTypeScriptVariable(type, format);
        }
    }
}
