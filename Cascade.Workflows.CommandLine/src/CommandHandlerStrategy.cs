using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Invocation;
using Cascade.Workflows.CommandLine.Composition;
using StudioLE.Core.Patterns;

namespace Cascade.Workflows.CommandLine;

public interface ICommandHandlerStrategy : IStrategy<CommandFactory, Func<InvocationContext, Task>>
{
}

public class CommandHandlerStrategy : ICommandHandlerStrategy
{
    private readonly IIsParseableStrategy _isParsableStrategy;

    public CommandHandlerStrategy(IIsParseableStrategy isParsableStrategy)
    {
        _isParsableStrategy = isParsableStrategy;
    }

    public Func<InvocationContext, Task> Execute(CommandFactory commandFactory)
    {
        if (commandFactory.InputTree is null)
            throw new("Expected input tree to be set.");
        return async context =>
        {
            SetInputTreeValueFromOptions(context.BindingContext, commandFactory);
            object input = commandFactory.InputTree.Instance;
            await commandFactory.Activity.Execute(input);
        };
    }

    private void SetInputTreeValueFromOptions(BindingContext context, CommandFactory commandFactory)
    {
        ObjectTreeProperty[] propertyFactories = commandFactory
            .InputTree!
            .FlattenProperties()
            .ToArray();
        foreach (ObjectTreeProperty factory in propertyFactories)
        {
            if (!_isParsableStrategy.Execute(factory.Type))
                continue;
            if (!commandFactory.Options.TryGetValue(factory.FullKey.ToLongOption(), out Option? option))
                continue;
            object? value = context.ParseResult.GetValueForOption(option);
            if (value is null)
                continue;
            if (!factory.Type.IsInstanceOfType(value))
                continue;
            factory.SetValue(value);
        }
    }
}
