using System.Net;

namespace Tectonic.Extensions.CommandLine.Tests.Resources;

public class ExampleErrorActivity : ActivityBase<ExampleClass, ExampleClass>
{

    public override Task<ExampleClass?> Execute(ExampleClass example)
    {
        example.Status = new(HttpStatusCode.BadRequest);
        return Task.FromResult<ExampleClass?>(example);
    }
}
