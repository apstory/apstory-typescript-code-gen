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
                var possibleImportVariables = new List<Variable>(model.Variables);
                foreach (var variable in model.Variables)
                {
                    if (variable.Type.Equals("Dictionary", StringComparison.OrdinalIgnoreCase))
                    {
                        var indexVariable = variable.SubVariables.FirstOrDefault();
                        var valueVariable = variable.SubVariables.LastOrDefault();
                        varStr += $"    {variable.Name}: Map<{indexVariable.Type}, {valueVariable.Type}{(valueVariable.IsArray ? "[]" : "")}>;{Environment.NewLine}";
                        
                        possibleImportVariables.Add(indexVariable);
                        possibleImportVariables.Add(valueVariable);
                    }
                    else
                        varStr += $"    {variable.Name}: {variable.Type}{(variable.IsArray ? "[]" : "")};{Environment.NewLine}";
                }

                foreach(var variable in possibleImportVariables)
                {
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
            var lwrType = type.ToLowerInvariant();
            if (lwrType == "number" || lwrType == "date" || lwrType == "string" || lwrType == "boolean" || lwrType == "dictionary")
                return true;

            return false;
        }
    }
}
