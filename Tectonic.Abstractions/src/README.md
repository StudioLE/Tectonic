## About

Abstractions of `Tectonic.Core`, a standardized approach for resolving and executing activity based workflows.

## How to Use

Add a reference to the abstractions from your library project:

```xml
<PackageReference Include="Tectonic.Abstractions" Version="0.8.0" />
```

Create a class that implements `IActivity<TInput, TOutput>`.

The constructor should assign any services you require via [dependency injection](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection).

The `TInput` and `TOutput` can be any class, the execution environment will handle assigning inputs and displaying outputs.

```csharp
public class CreateSomethingAsAnActivity : IActivity<MyInputClass, MyOutputClass>
{
    private readonly ILogger<CreateSomethingAsAnActivity> _logger;

    public CreateSomethingAsAnActivity(ILogger<CreateSomethingAsAnActivity> logger)
    {
        _logger = logger;
    }

    public Task<MyOutputClass> Execute(MyInputClass inputs)
    {
      // Do something with the inputs
      MyOutputClass outputs = new();
      // Do something with the outputs
      return Task.FromResult(outputs);
    }
}
```

Refer to [Tectonic.Extensions.CommandLine](../../Tectonic.Extensions.CommandLine/src) for how to execute activities and workflows on the command line.

### Examples

- The [unit tests](../../Tectonic.Core/tests) provide clear examples of how to use the library.
- [Tectonic.Extensions.CommandLine](../../Tectonic.Extensions.CommandLine/src) provides an implementation for executing Tectonic Workflows on the command line.
- [Tectonic.Server and Tectonic.WebAssembly](https://github.com/StudioLE/TectonicApp) provide an implementation for executing Tectonic Workflows as a Blazor UI application.
- [Orbit](https://github.com/StudioLE/Orbit) uses Tectonic activities for its command line interface.
