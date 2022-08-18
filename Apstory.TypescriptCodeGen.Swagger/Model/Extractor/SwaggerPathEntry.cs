using Apstory.TypescriptCodeGen.Swagger.Model.Extractor;

namespace Apstory.TypescriptCodeGen.Swagger.Model
{
    public class SwaggerPathEntry
    {
        public string[] Tags { get; set; }
        public string OperationId { get; set; }
        public SwaggerPathEntryParameter[] Parameters { get; set; }
        public SwaggerPathEntryRequest RequestBody { get; set; }
        public Dictionary<string, SwaggerPathEntryResponse> Responses { get; set; }
    }
}
