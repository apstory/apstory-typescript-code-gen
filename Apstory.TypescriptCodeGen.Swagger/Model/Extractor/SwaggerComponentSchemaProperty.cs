using Newtonsoft.Json;

namespace Apstory.TypescriptCodeGen.Swagger.Model.Extractor
{
    public class SwaggerComponentSchemaProperty
    {
        public string Type { get; set; }
        public string Format { get; set; }
        public bool Nullable { get; set; }

        [JsonProperty("$ref")]
        public string Reference { get; set; }

        public SwaggerComponentSchemaProperty Items { get; set; }
        public SwaggerComponentSchemaProperty AdditionalProperties { get; set; }
    }
}
