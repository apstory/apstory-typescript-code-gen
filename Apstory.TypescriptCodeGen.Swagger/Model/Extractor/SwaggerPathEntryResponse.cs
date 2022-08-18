using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apstory.TypescriptCodeGen.Swagger.Model.Extractor
{
    public class SwaggerPathEntryResponse
    {
        public string Description { get; set; }
        public Dictionary<string, SwaggerPathEntryResponseContent> Content { get; set; }
    }
}
