namespace Apstory.TypescriptCodeGen.Swagger.Model
{
    public class ClassDefinitionModel
    {
        public string Name { get; set; }

        public List<Variable> Variables { get; set; }

        public ClassDefinitionModel(string name)
        {
            this.Name = name;
            this.Variables = new List<Variable>();
        }

        public void Add(Variable variable)
        {
            this.Variables.Add(variable);
        }
    }
}
