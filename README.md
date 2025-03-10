# Apstory Typescript Code Generator

Typescript code generator, generating typescript models and api service calls from a swagger.json endpoint

## Usage
  
dotnet Apstory.TypescriptCodeGen.Swagger.dll -u http://localhost -v 1 -o c:\project\gen

## Command Options
**-u** The base url of the swagger api endpoint
**-v** The version of the swagger api endpoint
**-o** The directory to write the generated models and services to

## Prerequisites

Swaggers ```OperationId``` value must be populated in the JSON as its used as the method name in the typescript api service

Here is an example of how to achieve this in c#
```
public class AddActionNameAsOperationIdOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters == null) operation.Parameters = new List<OpenApiParameter>();

        var descriptor = context.ApiDescription.ActionDescriptor as ControllerActionDescriptor;
        if (descriptor != null)
            operation.OperationId = descriptor.ActionName;
    }
}
```

Then in program.cs, add in the OperationFilter to your swagger documentation generation.
```
builder.Services.AddSwaggerGen(c =>
{
    c.OperationFilter<AddActionNameAsOperationIdOperationFilter>();
}
```