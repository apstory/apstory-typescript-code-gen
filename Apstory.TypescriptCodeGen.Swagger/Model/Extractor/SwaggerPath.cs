namespace Apstory.TypescriptCodeGen.Swagger.Model.Extractor
{
    public class SwaggerPath
    {
        public SwaggerPathEntry Delete { get; set; }
        public SwaggerPathEntry Post { get; set; }
        public SwaggerPathEntry Get { get; set; }
        public SwaggerPathEntry Put { get; set; }

        public SwaggerPathEntry Patch { get; set; }

        public IEnumerable<SwaggerPathEntry> GetAll()
        {
            if (Delete is not null)
                yield return Delete;
            if (Post is not null)
                yield return Post;
            if (Get is not null)
                yield return Get;
            if (Put is not null)
                yield return Put;
            if (Patch is not null)
                yield return Patch;
        }
    }
}
