# Apstory Typescript Code Generator

Typescript code generator, generating typescript models and api service calls from a swagger.json file

## Usage
  
dotnet Apstory.TypescriptCodeGen.Swagger.dll -u http://localhost -v 1 -o c:\project\gen

## Command Options
**-u** The base url of the swagger api endpoint
**-v** The version of the swagger api endpoint
**-o** The directory to write the generated models and services to

## Prerequisites

Swaggers OperationId value must be populated in the JSON as its used as the method name in the typescript api service