namespace Apstory.TypescriptCodeGen.Swagger.Model
{
    public class ApiDefinitionModel
    {
        public string ControllerName { get; set; }
        public List<ApiDefinitionMethodModel> Methods { get; set; }

        public ApiDefinitionModel(string name)
        {
            this.ControllerName = name;
            this.Methods = new List<ApiDefinitionMethodModel>();
        }
    }
}
