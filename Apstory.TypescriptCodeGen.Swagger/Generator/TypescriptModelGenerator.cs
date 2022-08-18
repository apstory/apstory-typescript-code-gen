using Apstory.TypescriptCodeGen.Swagger.Model;
using Apstory.TypescriptCodeGen.Swagger.Util;

namespace Apstory.TypescriptCodeGen.Swagger.Generator
{
    public class TypescriptModelGenerator
    {
        private string _directoryPath;

        public TypescriptModelGenerator(string directoryPath)
        {
            _directoryPath = directoryPath;
        }

        public async Task Generate(List<ClassDefinitionModel> classModels)
        {
            foreach (var model in classModels)
            {
                var fileName = model.Name.ToKebabCase();
                var filePath = $"{_directoryPath}\\{fileName}.ts";

                var typescriptModel = "Template/TypescriptModel.txt".ToLocalPath().ReadEntireFile();
                typescriptModel = typescriptModel.Replace("#CLASSNAME#", model.Name);

                string importStr = string.Empty;
                string varStr = string.Empty;
                foreach (var variable in model.Variables)
                {

                    varStr += $"    {variable.Name}: {variable.Type}{(variable.IsArray ? "[]" : "")};{Environment.NewLine}";
                    if (!isKnownType(variable.Type))
                    {
                        var newImportStr = $"import {{ {variable.Type} }} from './{variable.Type.ToKebabCase()}';";
                        if (!importStr.Contains(newImportStr))
                            importStr += newImportStr + Environment.NewLine;
                    }
                }

                if (!string.IsNullOrWhiteSpace(importStr))
                    importStr += Environment.NewLine;

                typescriptModel = typescriptModel.Replace("#VARIABLES#", varStr);
                typescriptModel = typescriptModel.Replace("#IMPORTS#", importStr);


                typescriptModel.WriteToFile(filePath);
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
