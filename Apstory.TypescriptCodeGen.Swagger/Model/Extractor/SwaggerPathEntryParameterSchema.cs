using Newtonsoft.Json;

namespace Apstory.TypescriptCodeGen.Swagger.Model.Extractor
{
    public class SwaggerPathEntryParameterSchema
    {
        public string Type { get; set; }

        public SwaggerPathEntryParameterSchema Items { get; set; }

        [JsonProperty("$ref")]
        public string Reference { get; set; }
    }
}
