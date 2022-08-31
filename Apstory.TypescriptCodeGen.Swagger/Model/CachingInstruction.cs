namespace Apstory.TypescriptCodeGen.Swagger.Model
{
    public class CachingInstruction
    {
        public string ServiceName { get; set; }
        public string Version { get; set; }
        public string CachingCategory { get; set; }
        public string CachingDuration { get; set; }

        public CachingInstruction(string serviceName, string version, string cachingCategory, string cachingDuration)
        {
            ServiceName = serviceName;
            Version = version;
            CachingCategory = cachingCategory;
            CachingDuration = cachingDuration;
        }
    }
}
