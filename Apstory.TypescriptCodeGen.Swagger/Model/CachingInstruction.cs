namespace Apstory.TypescriptCodeGen.Swagger.Model
{
    public class CachingInstruction : CachingInstructionBase
    {
        public string CachingCategory { get; set; }
        public string CachingDuration { get; set; }
        public List<String> ExpireCategories { get; set; }

        public CachingInstruction(string serviceName, string version, string cachingCategory, string cachingDuration)
            : base(serviceName, version)
        {
            CachingCategory = cachingCategory;
            CachingDuration = cachingDuration;
            ExpireCategories = new List<string>() { CachingCategory };
        }

        public CachingInstruction(string serviceName, string version, string cachingCategory, string cachingDuration, List<string> expireCategories)
            : base(serviceName, version)
        {
            CachingCategory = cachingCategory;
            CachingDuration = cachingDuration;
            ExpireCategories = expireCategories;
        }
    }
}
