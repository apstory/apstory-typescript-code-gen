using Apstory.TypescriptCodeGen.Swagger.Model;
using Apstory.TypescriptCodeGen.Swagger.Util;

namespace Apstory.TypescriptCodeGen.Swagger.Generator
{
    public class TypescriptModelGenerator
    {
        private string _directoryPath;
        private string _exportFile;
        public TypescriptModelGenerator(string directoryPath, string exportFile)
        {
            _directoryPath = directoryPath;
            _exportFile = exportFile;
        }

        public async Task Generate(List<ClassDefinitionModel> classModels)
        {
            foreach (var model in classModels)
            {
                var fileName = model.Name.ToKebabCase();
                var filePath = string.Empty;

                if (string.IsNullOrWhiteSpace(model.Namespace))
                    filePath += $"{_directoryPath}/{fileName}.ts";
                else
                    filePath += $"{_directoryPath}/{model.Namespace.ToLower()}/{fileName}.ts";

                var typescriptModel = "Template/TypescriptModel.txt".ToLocalPath().ReadEntireFile();
                typescriptModel = typescriptModel.Replace("#CLASSNAME#", model.Name);

                string importStr = string.Empty;
                string varStr = string.Empty;
                foreach (var variable in model.Variables)
                {

                    varStr += $"    {variable.Name}: {variable.Type}{(variable.IsArray ? "[]" : "")};{Environment.NewLine}";
                    if (!isKnownType(variable.Type))
                    {
                        var newImportStr = string.Empty;
                        var namespaceStr = string.IsNullOrWhiteSpace(variable.Namespace) ? string.Empty : $"/{variable.Namespace.ToLower()}";
                        if (variable.Type.EndsWith("Id"))
                            newImportStr = $"import {{ {variable.Type} }} from '..{namespaceStr}/enums/{variable.Type.ToKebabCase()}';";
                        else
                            newImportStr = $"import {{ {variable.Type} }} from '..{namespaceStr}/{variable.Type.ToKebabCase()}';";

                        if (!importStr.Contains(newImportStr))
                            importStr += newImportStr + Environment.NewLine;
                    }
                }

                if (!string.IsNullOrWhiteSpace(importStr))
                    importStr += Environment.NewLine;

                typescriptModel = typescriptModel.Replace("#VARIABLES#", varStr);
                typescriptModel = typescriptModel.Replace("#IMPORTS#", importStr);

                typescriptModel.WriteToFile(filePath);

                if (!string.IsNullOrWhiteSpace(_exportFile))
                    $"export * from './lib/models/gen/{fileName}';{Environment.NewLine}".AppendToFile(_exportFile);
            }
        }

        public bool isKnownType(string type)
        {
            if (type == "number" || type == "Date" || type == "string" || type == "boolean")
                return true;

            return false;
        }
    }
}
