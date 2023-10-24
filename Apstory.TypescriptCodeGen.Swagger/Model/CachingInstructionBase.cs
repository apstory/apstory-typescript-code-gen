namespace Apstory.TypescriptCodeGen.Swagger.Model
{
    public class CachingInstructionBase
    {
        public string ServiceName { get; set; }
        public string Version { get; set; }

        public CachingInstructionBase(string serviceName, string version)
        {
            ServiceName = serviceName;
            Version = version;
        }
    }
}
