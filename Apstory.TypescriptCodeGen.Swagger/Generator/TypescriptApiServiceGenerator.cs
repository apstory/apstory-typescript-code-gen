using Apstory.TypescriptCodeGen.Swagger.Model;
using Apstory.TypescriptCodeGen.Swagger.Util;
using System;

namespace Apstory.TypescriptCodeGen.Swagger.Generator
{
    public class TypescriptApiServiceGenerator
    {
        private string _directoryPath;
        private string _version;
        private string _group;
        private string _exportFile;

        public TypescriptApiServiceGenerator(string directoryPath, string group, string version, string exportFile)
        {
            _directoryPath = directoryPath;
            _group = group;
            _version = version;
            _exportFile = exportFile;
        }

        public async Task Generate(List<ApiDefinitionModel> apiModels, List<CachingInstructionBase> cachingInstructions)
        {
            foreach (var model in apiModels)
            {
                var fileName = model.ControllerName.ToKebabCase();
                var filePath = Path.Join($"{_directoryPath}", $"{fileName}-{_group}.service.ts");

                var allCachesToApply = cachingInstructions.Where(s => s.ServiceName == model.ControllerName && s.Version == $"v{_version}").ToList();
                var cacheToApply = (CachingInstruction?)allCachesToApply.FirstOrDefault(s => s.GetType() == typeof(CachingInstruction));

                model.ControllerName += $"{_group.ToPascalCase()}Service";
                var typescriptModel = "Template/TypescriptApiService.txt".ToLocalPath().ReadEntireFile();
                typescriptModel = typescriptModel.Replace("#CLASSNAME#", model.ControllerName);

                string importStr = string.Empty;
                string methodStr = string.Empty;
                foreach (var method in model.Methods)
                {
                    if (method.ResponseParameter.Type.Contains("BinaryData"))
                        continue;

                    var url = method.Path;
                    var timeoutCacheToApply = allCachesToApply.Where(s => s.GetType() == typeof(TimeoutCachingInstruction))
                                                              .Cast<TimeoutCachingInstruction>()
                                                              .FirstOrDefault(s => s.MethodName == method.Name);

                    //Setup the functions return parameters
                    var responseParam = GetResponseParameter(method.ResponseParameter);
                    var responseImport = GetParameterImport(method.ResponseParameter);
                    if (!importStr.Contains(responseImport) && !method.ResponseParameter.Type.Equals("File"))
                        importStr += responseImport;

                    //Setup the functions incoming parameters
                    var methodParameters = string.Empty;
                    foreach (var param in method.Parameters.Where(s => s.In == Model.Enums.ParameterIn.Path ||
                                                                       s.In == Model.Enums.ParameterIn.Body ||
                                                                       s.In == Model.Enums.ParameterIn.Query))
                    {
                        if (param.Name == "version")
                            continue;

                        if (method.HttpMethod == Model.Enums.HttpMethod.Post && param.In == Model.Enums.ParameterIn.Query)
                            continue;

                        if (!string.IsNullOrWhiteSpace(methodParameters))
                            methodParameters += ", ";

                        methodParameters += $"{param.Name.ToCamelCase()}: {param.Type}";

                        var newImportStr = GetParameterImport(param);
                        if (!importStr.Contains(newImportStr))
                            importStr += newImportStr;

                        if (param.Type == "Date")
                        {
                            url = url.Replace($"{{{param.Name.ToCamelCase()}}}", $"{{await this.baseService.getDateString({param.Name.ToCamelCase()})}}");
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(methodParameters))
                        methodParameters += ", ";
                    methodParameters += $"timeout?: number";

                    var queryParameters = string.Empty;
                    if (method.HttpMethod != Model.Enums.HttpMethod.Post)
                        queryParameters = GenerateQueryParameters(method.Parameters);

                    // Setup http call: 'await baseService.http___(URL,DATA);
                    var httpMethod = method.HttpMethod.ToString();
                    var httpUnAuthed = method.Authenticated ? string.Empty : "UnAuthenticated";
                    var postParams = string.Empty;
                    var postData = method.Parameters.FirstOrDefault(s => s.In == Model.Enums.ParameterIn.Body);
                    if (postData is not null)
                        postParams = $", {postData.Name.ToCamelCase()}";
                    else if (method.HttpMethod == Model.Enums.HttpMethod.Post || method.HttpMethod == Model.Enums.HttpMethod.Put)
                        //All post methods require a body, Supply an empty one
                        postParams = $", {{ }}";

                    url = url.Replace("{version}", "{this.version}").Replace("{", "${encodeURIComponent(").Replace("}", ")}");
                    if (timeoutCacheToApply is not null)
                        methodStr += $"\t@TimeoutCache(CacheArguments.{timeoutCacheToApply.CachingArguments}){Environment.NewLine}";

                    if (cacheToApply is not null)
                    {
                        if (method.HttpMethod != Model.Enums.HttpMethod.Delete)
                        {
                            if (method.HttpMethod == Model.Enums.HttpMethod.Post ||
                                method.HttpMethod == Model.Enums.HttpMethod.Put ||
                                method.HttpMethod == Model.Enums.HttpMethod.Delete)
                            {
                                foreach (var expireCache in cacheToApply.ExpireCategories)
                                    methodStr += $"\t@CacheExpireCategory(CacheCategory.{expireCache}){Environment.NewLine}";
                            }
                            else if (string.IsNullOrEmpty(cacheToApply.CachingDuration))
                                methodStr += $"\t@Cacheable(CacheCategory.{cacheToApply.CachingCategory}){Environment.NewLine}";
                            else
                                methodStr += $"\t@Cacheable(CacheCategory.{cacheToApply.CachingCategory}, {cacheToApply.CachingDuration}){Environment.NewLine}";
                        }
                    }

                    methodStr += $"\tpublic async {method.Name}({methodParameters}): Promise{responseParam} {{{Environment.NewLine}";
                    methodStr += $"\t\tconst url = `${{this.baseService.apiUrl}}{url}{queryParameters}`;{Environment.NewLine}";

                    if (method.HttpMethod == Model.Enums.HttpMethod.Get && responseParam == "<File>")
                    {
                        methodStr += $"\t\tconst response = await this.baseService.httpGetBlob(url, timeout);{Environment.NewLine}";
                        methodStr += $"\t\treturn new File([response], `{method.Name}`, {{ type: response.type }});{Environment.NewLine}";
                    }
                    else
                        methodStr += $"\t\treturn await this.baseService.http{httpMethod}{httpUnAuthed}{responseParam}(url{postParams}, timeout);{Environment.NewLine}";


                    methodStr += $"\t}}{Environment.NewLine}";
                    methodStr += $"{Environment.NewLine}";
                }

                var hasTimeoutCaches = allCachesToApply.Any(s => s.GetType() == typeof(TimeoutCachingInstruction));
                if (hasTimeoutCaches)
                    importStr += $"import {{ CacheArguments, TimeoutCache }} from '../../../caching/timeout-cache-decorator';{Environment.NewLine}";

                if (cacheToApply is not null)
                    importStr += $"import {{ Cacheable, CacheCategory, CacheExpireCategory }} from './../../../caching/cachable-decorator';{Environment.NewLine}";

                typescriptModel = typescriptModel.Replace("#VERSION#", $"'{_version}'");
                typescriptModel = typescriptModel.Replace("#METHODS#", methodStr);
                typescriptModel = typescriptModel.Replace("#IMPORTS#", importStr);

                typescriptModel.WriteToFile(filePath);

                if (!string.IsNullOrWhiteSpace(_exportFile))
                    $"export * from './lib/services/gen/api/{_group}/{fileName}-{_group}.service';{Environment.NewLine}".AppendToFile(_exportFile);
            }
        }

        private bool isKnownType(string type)
        {
            var newType = type.Replace("[]", "");
            if (newType == "number" || newType == "Date" || newType == "string" || newType == "boolean")
                return true;

            return false;
        }

        private string GetResponseParameter(Parameter parameter)
        {
            var retResponseParam = parameter?.Type;
            if (!string.IsNullOrWhiteSpace(retResponseParam))
                retResponseParam = $"<{retResponseParam}>";
            else
                retResponseParam = "<void>";

            return retResponseParam;
        }

        private string GetParameterImport(Parameter parameter)
        {
            if (parameter?.Type is not null)
            {
                var nonArrayType = parameter?.Type.Replace("[]", "");
                if (!isKnownType(nonArrayType))
                {
                    var namespaceStr = string.IsNullOrWhiteSpace(parameter.Namespace) ? string.Empty : $"/{parameter.Namespace.ToLower()}";

                    //All complex objects that end with Id are Enums
                    if (nonArrayType.EndsWith("Id"))
                        return $"import {{ {nonArrayType} }} from '../../../../models{namespaceStr}/enums/{nonArrayType.ToKebabCase()}';{Environment.NewLine}";
                    else
                        return $"import {{ {nonArrayType} }} from '../../../../models/gen{namespaceStr}/{nonArrayType.ToKebabCase()}';{Environment.NewLine}";
                }
            }

            return string.Empty;
        }

        private string GenerateQueryParameters(List<Parameter> parameters)
        {
            var retQueryParameters = string.Empty;
            foreach (var param in parameters.Where(s => s.In == Model.Enums.ParameterIn.Query))
            {
                if (string.IsNullOrWhiteSpace(retQueryParameters))
                    retQueryParameters += "?";
                else
                    retQueryParameters += "&";

                if (param.Type.EndsWith("[]"))
                    retQueryParameters += $"${{this.baseService.createQueryParams({param.Name}, '{param.Name}')}}";
                else
                {
                    var dateFriendlyParam = param.Name;
                    if (param.Type == "Date")
                        dateFriendlyParam = $"await this.baseService.getDateString({param.Name})";

                    retQueryParameters += $"{param.Name}=${{(!{param.Name} ? '' : encodeURIComponent({dateFriendlyParam}))}}";
                }
            }

            return retQueryParameters;
        }
    }
}
