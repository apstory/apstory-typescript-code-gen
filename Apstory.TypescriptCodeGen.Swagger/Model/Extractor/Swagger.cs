namespace Apstory.TypescriptCodeGen.Swagger.Model.Extractor
{
    public class Swagger
    {
        public string OpenApi { get; set; }
        public SwaggerInfo Info { get; set; }
        public Dictionary<string, SwaggerPath> Paths { get; set; }
        public SwaggerComponent Components { get; set; }
    }
}
