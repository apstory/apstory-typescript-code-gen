using Microsoft.Extensions.CommandLineUtils;
using Apstory.TypescriptCodeGen.Swagger.Extractors;
using Apstory.TypescriptCodeGen.Swagger.Generator;

namespace Apstory.TypescriptCodeGen.Swagger
{

    class Program
    {
        static void Main(string[] args)
        {
            CommandLineApplication commandLineApplication = new CommandLineApplication(throwOnUnexpectedArg: false);

            CommandOption url = commandLineApplication.Option(
              "-u |--Url <URL>", "The base url of the swagger api endpoint", CommandOptionType.SingleValue);
            CommandOption version = commandLineApplication.Option(
              "-v |--Version <Version>", "The version of the swagger api endpoint", CommandOptionType.SingleValue);
            CommandOption outputDirectory = commandLineApplication.Option(
              "-o |--OutputDirectory <OutputDirectory>", "The path to output to", CommandOptionType.SingleValue);
            CommandOption exportFile = commandLineApplication.Option(
              "-e |--ExportFile <ExportFile>", "A .ts file that lists all generated files", CommandOptionType.SingleValue);

            commandLineApplication.HelpOption("-? | -h | --help");

            commandLineApplication.OnExecute(async () =>
            {
                Console.WriteLine($"Url: {url.Value()}");
                Console.WriteLine($"Version: {version.Value()}");
                Console.WriteLine($"OutputDirectory: {outputDirectory.Value()}");
                Console.WriteLine($"ExportFile: {exportFile.Value()}");

                await RunCodeGen(url.Value(), version.Value(), outputDirectory.Value(), exportFile.Value());

                return 0;
            });

            commandLineApplication.Execute(args);
        }

        public static async Task RunCodeGen(string url, string version, string outputDirectory, string exportFile)
        {
            if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(version) && !string.IsNullOrEmpty(outputDirectory))
            {
                try
                {
                    var se = new SwaggerExtractor();
                    await se.Extract($"{url}/swagger/v{version}/swagger.json");

                    var tmg = new TypescriptModelGenerator($"{outputDirectory}\\models\\gen", exportFile);
                    await tmg.Generate(se.GetClassModels());

                    var tasg = new TypescriptApiServiceGenerator($"{outputDirectory}\\services\\gen\\api\\v{version}", version, exportFile);
                    await tasg.Generate(se.GetApiModels());
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
    }
}