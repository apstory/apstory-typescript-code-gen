using Microsoft.Extensions.CommandLineUtils;
using Apstory.TypescriptCodeGen.Swagger.Extractors;
using Apstory.TypescriptCodeGen.Swagger.Generator;
using Apstory.TypescriptCodeGen.Swagger.Model;
using Apstory.TypescriptCodeGen.Swagger.Util;
using System.Reflection;

namespace Apstory.TypescriptCodeGen.Swagger
{
    class Program
    {
        static void Main(string[] args)
        {
            var versionString = Assembly.GetEntryAssembly()?
                                        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                                        .InformationalVersion
                                        .ToString();

            Console.WriteLine($"Apstory TypeScript-Codegen v{versionString}");

            CommandLineApplication commandLineApplication = new CommandLineApplication(throwOnUnexpectedArg: false);

            CommandOption url = commandLineApplication.Option(
              "-u |--Url <URL>", "The base url of the swagger api endpoint", CommandOptionType.SingleValue);
            CommandOption version = commandLineApplication.Option(
              "-v |--Version <Version>", "The version of the swagger api endpoint", CommandOptionType.SingleValue);
            CommandOption group = commandLineApplication.Option(
              "-g |--Group <Group>", "The group name of the swagger api endpoint (automatically generated from version if left blank)", CommandOptionType.SingleValue);
            CommandOption outputDirectory = commandLineApplication.Option(
              "-o |--OutputDirectory <OutputDirectory>", "The path to output to", CommandOptionType.SingleValue);
            CommandOption exportFile = commandLineApplication.Option(
              "-e |--ExportFile <ExportFile>", "A .ts file that lists all generated files", CommandOptionType.SingleValue);
            CommandOption cachingFile = commandLineApplication.Option(
              "-c |--CachingFile <CachingFile>", "A .txt file that lists all caching instructions in the format: '[Service Name]:[Version]:[Caching Category]:[Duration In Minutes]?:[Caches To Expire on Post/Put/Delete]'", CommandOptionType.SingleValue);
            CommandOption appendModel = commandLineApplication.Option(
              "-am |--AppendModel <TextToAppend>", "Appends generated typescript models with supplied value (Useful for offline fields)", CommandOptionType.SingleValue);

            commandLineApplication.HelpOption("-? | -h | --help");

            commandLineApplication.OnExecute(async () =>
            {
                Console.WriteLine($"Url: {url.Value()}");
                Console.WriteLine($"Version: {version.Value()}");
                Console.WriteLine($"Group: {group.Value()}");
                Console.WriteLine($"OutputDirectory: {outputDirectory.Value()}");
                Console.WriteLine($"ExportFile: {exportFile.Value()}");

                await RunCodeGen(url.Value(), group.Value(), version.Value(), outputDirectory.Value(), exportFile.Value(), cachingFile.Value(), appendModel.Value());

                return 0;
            });

            commandLineApplication.Execute(args);
        }

        public static async Task RunCodeGen(string url, string group, string version, string outputDirectory, string exportFile, string cachingFile, string appendModel)
        {
            if (string.IsNullOrEmpty(group))
                group = $"v{version}";

            if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(version) && !string.IsNullOrEmpty(outputDirectory))
            {
                try
                {
                    List<CachingInstructionBase> cachingInstructions = new List<CachingInstructionBase>();
                    if (!string.IsNullOrWhiteSpace(cachingFile))
                        cachingInstructions = ExtractCachingFromFile(cachingFile);

                    var se = new SwaggerExtractor();
                    await se.Extract($"{url}/swagger/{group}/swagger.json");

                    if (!string.IsNullOrWhiteSpace(exportFile))
                        File.Delete(exportFile);


                    var tmg = new TypescriptModelGenerator(Path.Join(outputDirectory, "models", "gen"), exportFile);
                    await tmg.Generate(se.GetClassModels(), appendModel);

                    var tasg = new TypescriptApiServiceGenerator(Path.Join(outputDirectory, "services", "gen", "api", group), group, version, exportFile);
                    await tasg.Generate(se.GetApiModels(), cachingInstructions);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception! {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Error! Url, Version and OutputDirectory needs to be supplied");
            }
        }

        private static List<CachingInstructionBase> ExtractCachingFromFile(string cachingFile)
        {
            List<CachingInstructionBase> retCachingInstructions = new List<CachingInstructionBase>();

            var cachingContents = cachingFile.ReadEntireFile();
            foreach (var cachingLine in cachingContents.Split("\n"))
            {
                var parts = cachingLine.Trim().Split(":");
                if (parts[0] == "TCACHE")
                    retCachingInstructions.Add(new TimeoutCachingInstruction(parts[1], parts[2], parts[3], parts[4]));
                else
                {
                    if (parts.Length >= 5)
                        retCachingInstructions.Add(new CachingInstruction(parts[0], parts[1], parts[2], parts[3], parts[4].Split(",").ToList()));
                    else if (parts.Length >= 3)
                        retCachingInstructions.Add(new CachingInstruction(parts[0], parts[1], parts[2], parts.Length > 3 ? parts[3] : String.Empty));
                }
            }

            return retCachingInstructions;
        }
    }
}