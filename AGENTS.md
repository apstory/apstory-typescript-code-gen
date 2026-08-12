# AGENTS.md

## What this is

A .NET (net10.0) console app / dotnet tool that fetches a `swagger.json` endpoint and generates Angular TypeScript models and API services. Single project, no tests, no CI.

## Build / run

```sh
dotnet build Apstory.TypescriptCodeGen.Swagger.sln
dotnet run --project Apstory.TypescriptCodeGen.Swagger -- -u http://localhost -v 1 -o /path/to/out
dotnet pack Apstory.TypescriptCodeGen.Swagger     # PackAsTool, output to ./nupkg (gitignored)
```

- There is no test suite and no linter/formatter to run.
- `bin/` and `obj/` are gitignored (stale net6/net8 artifacts still exist on disk from prior TFMs; ignore them — target is net10.0).

## How the generator works

- `Program.cs` parses CLI args and calls `RunCodeGen`, which fetches `{url}/swagger/{group}/swagger.json` (group defaults to `v{version}`).
- `Extractor/SwaggerExtractor.cs` deserializes the swagger JSON (`$ref` handled as plain values, not resolved).
- `Generator/TypescriptModelGenerator.cs` + `Generator/TypescriptApiServiceGenerator.cs` generate output from string templates in `Template/*.txt` using `#PLACEHOLDER#` replacement. Templates are copied to the output dir at build time and loaded via `FileExtensions.ToLocalPath()` (relative to the assembly location).

Output layout:
- models → `{output}/models/gen/`
- services → `{output}/services/gen/api/{group}/`
- optional `-e` export file gets `export * ...` lines appended (note: appended, not overwritten if `-e` omitted).

## CLI options

`-u/--Url`, `-v/--Version`, `-g/--Group`, `-o/--OutputDirectory`, `-e/--ExportFile`, `-c/--CachingFile`, `-am/--AppendModel`. See `Program.cs` for exact semantics.

## Non-obvious conventions (agent gotchas)

- The generated TypeScript is **Angular-specific** (assumes `BaseHttpService`, `@Injectable`, caching decorators). It is not generic TS.
- Swagger `operationId` is required as the method name; methods missing it are skipped with a warning. `tags[0]` becomes the controller/service name.
- Types whose name ends in `Id` are emitted as **enum** imports (`.../enums/{type}.ts`), not model imports — see `GetParameterImport`.
- `Dictionary<string, T>` swagger objects are emitted as `Map<string, T>`.
- Swagger methods tagged `AllowAnonymous` generate `http*UnAuthenticated` calls.
- Operation IDs starting with `Download` with no JSON 200 response emit a `File`/blob response.
- Response type `StringByteArrayValueTuple` is skipped (hardcoded hack in `SwaggerExtractor.GetApiModels`).
- Casing helpers (`Util/CasingExtensions.cs`) are hand-rolled; `ToKebabCase`/`ToCamelCase` drive filenames and URL param casing. Watch these when touching naming — they have acronym edge cases.

## Prerequisites for the source API

The swagger endpoint must populate `operationId` (see `README.md` for the C# `IOperationFilter` setup). Without it, endpoints are silently dropped.
