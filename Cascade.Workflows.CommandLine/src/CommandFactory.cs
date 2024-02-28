using System.CommandLine;
using System.CommandLine.Invocation;
using Cascade.Workflows.CommandLine.Composition;
using StudioLE.Patterns;

namespace Cascade.Workflows.CommandLine;

public class CommandFactory : IFactory<IActivity, Command>
{
    private readonly ICommandArgumentsStrategy _argumentsStrategy;
    private readonly ICommandOptionsStrategy _optionsStrategy;
    private readonly ICommandHandlerStrategy _handlerStrategy;

    public CommandContext Context { get; }

    public IActivity Activity { get; private set; } = null!;

    public ObjectTree? InputTree { get; private set; }

    public IReadOnlyDictionary<string, Argument> Arguments { get; private set; } = new Dictionary<string, Argument>();

    public IReadOnlyDictionary<string, Option> Options { get; private set; } = new Dictionary<string, Option>();

    public CommandFactory(
        ICommandArgumentsStrategy argumentsStrategy,
        ICommandOptionsStrategy optionsStrategy,
        ICommandHandlerStrategy handlerStrategy,
        CommandContext context)
    {
        _argumentsStrategy = argumentsStrategy;
        _optionsStrategy = optionsStrategy;
        _handlerStrategy = handlerStrategy;
        Context = context;
    }

    public Command Create(IActivity activity)
    {
        Activity = activity;
        InputTree = CreateObjectTree();
        Arguments = _argumentsStrategy.Execute(this);
        Options = _optionsStrategy.Execute(this);
        string commandName = GetCommandName();
        string description = GetDescription();
        Command command = new(commandName, description);
        foreach (KeyValuePair<string, Argument> pair in Arguments)
            command.AddArgument(pair.Value);
        foreach (KeyValuePair<string, Option> pair in Options)
            command.AddOption(pair.Value);
        Func<InvocationContext, Task> handler = _handlerStrategy.Execute(this);
        command.SetHandler(handler);
        return command;
    }

    private ObjectTree CreateObjectTree()
    {
        Type inputType = Activity.GetInputType();
        object? inputs = Activator.CreateInstance(inputType);
        if (inputs is null)
            throw new($"Failed to create inputs for activity: {Activity.GetName()}."
                      + "The input type {inputType} does not have a parameterless constructor.");
        return new(inputs);
    }

    private string GetCommandName()
    {
        return Activity.GetName().ToLower();
    }

    private string GetDescription()
    {
        return Activity.GetDescription();
    }
}
