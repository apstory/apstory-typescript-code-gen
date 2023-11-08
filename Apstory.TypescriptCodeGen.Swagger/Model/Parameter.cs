namespace Apstory.TypescriptCodeGen.Swagger.Model
{
    public class Parameter
    {
        public string Name { get; set; }
        public string Namespace { get; set; }
        public Enums.ParameterIn In { get; set; }
        public string Type { get; set; }
        public string SubType { get; set; }

        public Parameter(string name, Enums.ParameterIn paramIn, string type, string subType)
        {
            if (type.IndexOf(".") > -1)
            {
                var splits = type.Split('.');
                this.Type = splits[splits.Length - 1];
                this.Namespace = splits[splits.Length - 2];
            }
            else
                this.Type = type;

            if (name.IndexOf(".") > -1)
            {
                var splits = name.Split('.');
                this.Name = splits[splits.Length - 1];
            }
            else
                this.Name = name;

            this.In = paramIn;
            this.SubType = subType;
        }
    }
}
