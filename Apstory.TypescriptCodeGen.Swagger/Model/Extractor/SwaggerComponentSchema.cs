namespace Apstory.TypescriptCodeGen.Swagger.Model.Extractor
{
    public class SwaggerComponentSchema
    {
        public string Type { get; set; }
        public Dictionary<string, SwaggerComponentSchemaProperty> Properties { get; set; }
        public bool AdditionalProperties { get; set; }
    }
}
