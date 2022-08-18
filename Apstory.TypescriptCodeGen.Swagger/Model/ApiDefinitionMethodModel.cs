namespace Apstory.TypescriptCodeGen.Swagger.Model
{
    public class ApiDefinitionMethodModel
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public Enums.HttpMethod HttpMethod { get; set; }
        public bool Authenticated { get; set; }
        public List<Parameter> Parameters { get; set; }
        public Parameter ResponseParameter { get; set; }
    }
}
