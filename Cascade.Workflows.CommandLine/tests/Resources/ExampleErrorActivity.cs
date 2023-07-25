namespace Cascade.Workflows.CommandLine.Tests.Resources;

public class ExampleErrorActivity : IActivity<ExampleClass, ExampleClass>
{
    private readonly CommandContext _context;

    public ExampleErrorActivity(CommandContext context)
    {
        _context = context;
    }

    public Task<ExampleClass> Execute(ExampleClass example)
    {
        _context.ExitCode = 99;
        return Task.FromResult(example);
    }
}
