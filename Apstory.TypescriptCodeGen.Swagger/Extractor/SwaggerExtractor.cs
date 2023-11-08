using Newtonsoft.Json;
using Apstory.TypescriptCodeGen.Swagger.Model;
using Apstory.TypescriptCodeGen.Swagger.Util;
using Apstory.TypescriptCodeGen.Swagger.Model.Extractor;

namespace Apstory.TypescriptCodeGen.Swagger.Extractors
{
    public class SwaggerExtractor
    {
        private Model.Extractor.Swagger? _data;

        public SwaggerExtractor()
        { }

        public async Task Extract(string path)
        {
            using var http = new HttpClient();
            var swaggerJsonString = await http.GetStringAsync(path);

            var settings = new JsonSerializerSettings();
            settings.MetadataPropertyHandling = MetadataPropertyHandling.Ignore; //Parse $ref indicators as normal values
            _data = JsonConvert.DeserializeObject<Model.Extractor.Swagger>(swaggerJsonString, settings)!;
        }

        public List<ClassDefinitionModel> GetClassModels()
        {
            if (_data is null)
                throw new Exception("Run Extract(path) before attempting to fetch class models");

            List<ClassDefinitionModel> retClassDefinitionModels = new List<ClassDefinitionModel>();
            foreach (var swagCompKvp in _data.Components.Schemas.Where(s => s.Value?.Properties != null))
            {
                var cdm = new ClassDefinitionModel(swagCompKvp.Key);
                foreach (var swagPropKvp in swagCompKvp.Value.Properties)
                {
                    var varType = swagPropKvp.Value?.Reference ?? swagPropKvp.Value.Type;
                    if (varType.IndexOf("/") > 0)
                        varType = varType.Substring(varType.LastIndexOf("/") + 1);

                    bool isArray = false;
                    if (varType == "array")
                    {
                        varType = swagPropKvp.Value.Items?.Reference ?? swagPropKvp.Value.Items.Type;
                        if (varType.IndexOf("/") > 0)
                            varType = varType.Substring(varType.LastIndexOf("/") + 1);

                        isArray = true;
                    }

                    cdm.Variables.Add(new Variable(swagPropKvp.Key, varType, swagPropKvp.Value.Format, swagPropKvp.Value.Nullable, isArray));
                }

                retClassDefinitionModels.Add(cdm);
            }

            return retClassDefinitionModels;
        }

        public List<ApiDefinitionModel> GetApiModels()
        {
            if (_data is null)
                throw new Exception("Run Extract(path) before attempting to fetch class models");

            List<ApiDefinitionModel> retApiDefinitionModels = new List<ApiDefinitionModel>();
            foreach (var swagPathKvp in _data.Paths)
            {
                foreach (var swagMethodInfo in swagPathKvp.Value.GetAll())
                {
                    var controllerName = swagMethodInfo.Tags[0];
                    var adm = retApiDefinitionModels.FirstOrDefault(s => s.ControllerName == controllerName);
                    if (adm is null)
                    {
                        adm = new ApiDefinitionModel(controllerName);
                        retApiDefinitionModels.Add(adm);
                    }

                    var method = new ApiDefinitionMethodModel();

                    if (swagMethodInfo == swagPathKvp.Value.Get)
                        method.HttpMethod = Model.Enums.HttpMethod.Get;
                    else if (swagMethodInfo == swagPathKvp.Value.Delete)
                        method.HttpMethod = Model.Enums.HttpMethod.Delete;
                    else if (swagMethodInfo == swagPathKvp.Value.Put)
                        method.HttpMethod = Model.Enums.HttpMethod.Put;
                    else if (swagMethodInfo == swagPathKvp.Value.Post)
                        method.HttpMethod = Model.Enums.HttpMethod.Post;
                    else if (swagMethodInfo == swagPathKvp.Value.Patch)
                        method.HttpMethod = Model.Enums.HttpMethod.Patch;

                    method.Name = swagMethodInfo.OperationId;
                    method.Path = swagPathKvp.Key;
                    method.Authenticated = !swagMethodInfo.Tags.Contains("AllowAnonymous");
                    method.Parameters = new List<Parameter>();

                    if (swagMethodInfo.RequestBody is not null)
                    {
                        if (swagMethodInfo.RequestBody.Content.ContainsKey("application/json"))
                        {
                            var entry = swagMethodInfo.RequestBody.Content["application/json"];

                            string refName = GetVariableType(entry.Schema);
                            method.Parameters.Add(new Parameter(refName.Replace("[]", "s"), Model.Enums.ParameterIn.Body, VariableExtensions.ToTypeScriptVariable(refName, entry.Schema.Format), string.Empty));
                        }

                        //TODO: "multipart/form-data"
                    }

                    if (swagMethodInfo.Responses is not null)
                    {
                        var successResponse = swagMethodInfo.Responses["200"];
                        if (successResponse is not null && successResponse.Content is not null && successResponse.Content.ContainsKey("application/json"))
                        {
                            var entry = successResponse.Content["application/json"];
                            string refName = GetVariableType(entry.Schema);

                            //TODO: Hack for now to get around invalid response types
                            if (refName == "StringByteArrayValueTuple")
                                continue;

                            method.ResponseParameter = new Parameter(refName.Replace("[]", "s"), Model.Enums.ParameterIn.Body, VariableExtensions.ToTypeScriptVariable(refName, entry.Schema.Format), string.Empty);
                        }
                    }

                    if (swagMethodInfo.Parameters is not null)
                        foreach (var param in swagMethodInfo.Parameters)
                        {
                            string refName = string.Empty;
                            if (param.Schema.Type == "array")
                                refName = $"{VariableExtensions.ToTypeScriptVariable(param.Schema.Items.Type, param.Schema.Format)}[]";
                            else
                                refName = param.Schema.Type ?? param.Schema.Reference.Substring(param.Schema.Reference.LastIndexOf("/") + 1);

                            method.Parameters.Add(new Parameter(param.Name, Enum.Parse<Model.Enums.ParameterIn>(param.In, true), VariableExtensions.ToTypeScriptVariable(refName, param.Schema.Format), param.Schema.Items?.Reference));
                        }

                    adm.Methods.Add(method);
                }
            }

            return retApiDefinitionModels;
        }

        private string GetVariableType(SwaggerPathEntryParameterSchema schema)
        {
            if (schema.Type == "array")
                return GetVariableFromSwaggerStruct(schema.Items.Type, schema.Reference ?? schema.Items.Reference) + "[]";
            else
                return GetVariableFromSwaggerStruct(schema.Type, schema.Reference);
        }

        private string GetVariableFromSwaggerStruct(string type, string reference)
        {
            if (!string.IsNullOrWhiteSpace(type))
                return VariableExtensions.ToTypeScriptVariable(type, string.Empty);

            return reference.Substring(reference.LastIndexOf("/") + 1);
        }
    }
}
