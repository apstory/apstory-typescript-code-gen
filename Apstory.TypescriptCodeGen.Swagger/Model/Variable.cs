using Apstory.TypescriptCodeGen.Swagger.Util;

namespace Apstory.TypescriptCodeGen.Swagger.Model
{
    public class Variable
    {
        public string Name { get; set; }
        public string Namespace { get; set; }
        public string Type { get; set; }
        public bool Nullable { get; set; }
        public bool IsArray { get; set; }

        public Variable(string name, string type, string format, bool nullable, bool isArray)
        {
            if (type.IndexOf(".") > -1)
            {
                var splits = type.Split('.');
                this.Type = VariableExtensions.ToTypeScriptVariable(splits[splits.Length - 1], format);
                this.Namespace = splits[splits.Length - 2];
            }
            else
                this.Type = VariableExtensions.ToTypeScriptVariable(type, format);

            this.Name = name;
            this.Nullable = nullable;
            this.IsArray = isArray;
        }

        public List<Variable> SubVariables { get; set; }
    }
}
