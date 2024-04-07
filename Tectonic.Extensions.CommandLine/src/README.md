## About

An implementation of [Tectonic.Core](../../Tectonic.Core/src) for executing Tectonic activities and workflows on the command line using [System.CommandLine](https://learn.microsoft.com/en-us/dotnet/standard/commandline/).

## How to Use


Refer to [Tectonic.Abstractions](../../Tectonic.Abstractions/src) for how to create an activity, then follow these steps to execute it on the command line:

Add a reference to the library from your CLI project:

```xml
<PackageReference Include="Tectonic.Extensions.CommandLine" Version="0.8.0" />
```

Add the following code to your CLI project:

```csharp
IHost host = Host
    .CreateDefaultBuilder()
    .RegisterCustomLoggingProviders()
    .ConfigureServices((context, services) => services
        .AddCommandBuilderServices())
    .Build();
CommandBuilder builder = host
    .Services
    .GetRequiredService<CommandBuilder>();
RootCommand command = builder
    .Register<CreateSomethingAsAnActivity>("your-command-name", "your-subcommand-name")
    .Build();
await command.InvokeAsync(args);
```

Then

```bash
dotnet run your-command-name your-subcommand-name --help
```

```bash
./path/to/YOUR_EXECUTABLE your-command-name your-subcommand-name --help
```

### Examples

- The [unit tests](../tests) provide clear examples of how to use the library.
- [Orbit](https://github.com/StudioLE/Orbit) uses Tectonic activities for its command line interface.
