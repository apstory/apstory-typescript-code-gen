namespace Apstory.TypescriptCodeGen.Swagger.Model
{
    public class ClassDefinitionModel
    {
        public string Name { get; set; }
        public string Namespace { get; set; }

        public List<Variable> Variables { get; set; }

        public ClassDefinitionModel(string name)
        {
            if (name.IndexOf(".") > -1)
            {
                var splits = name.Split('.');
                this.Name = splits[splits.Length - 1];
                this.Namespace = splits[splits.Length - 2];
            }
            else
                this.Name = name;

            this.Variables = new List<Variable>();
        }

        public void Add(Variable variable)
        {
            this.Variables.Add(variable);
        }
    }
}
