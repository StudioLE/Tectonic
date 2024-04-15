using System.Net;

namespace Tectonic.Extensions.CommandLine.Tests.Resources;

public class ExampleErrorActivity : IActivity<ExampleClass, ExampleClass>
{

    public Task<ExampleClass> Execute(ExampleClass example)
    {
        example.Status = new(HttpStatusCode.BadRequest);
        return Task.FromResult(example);
    }
}
