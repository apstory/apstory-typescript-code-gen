namespace Apstory.TypescriptCodeGen.Swagger.Model
{
    public class TimeoutCachingInstruction : CachingInstructionBase
    {
        public string MethodName { get; set; }
        public string CachingArguments { get; set; }

        public TimeoutCachingInstruction(string serviceName, string version, string methodName, string cachingArguments)
            : base(serviceName, version)
        {
            MethodName = methodName;
            CachingArguments = cachingArguments;
        }
    }
}
