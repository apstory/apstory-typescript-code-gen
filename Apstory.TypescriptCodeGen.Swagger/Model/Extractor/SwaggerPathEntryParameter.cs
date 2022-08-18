using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apstory.TypescriptCodeGen.Swagger.Model.Extractor
{
    public class SwaggerPathEntryParameter
    {
        public string Name { get; set; }
        public string In { get; set; }
        public SwaggerPathEntryParameterSchema Schema { get; set; }
    }
}
