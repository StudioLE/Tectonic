using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Invocation;
using Cascade.Workflows.CommandLine.Composition;
using Microsoft.Extensions.Logging;
using StudioLE.Core.Patterns;

namespace Cascade.Workflows.CommandLine;

public interface ICommandHandlerStrategy : IStrategy<CommandFactory, Func<InvocationContext, Task>>
{
}

public class CommandHandlerStrategy : ICommandHandlerStrategy
{
    private readonly ILogger<CommandHandlerStrategy> _logger;
    private readonly IIsParseableStrategy _isParsableStrategy;

    public CommandHandlerStrategy(ILogger<CommandHandlerStrategy> logger, IIsParseableStrategy isParsableStrategy)
    {
        _logger = logger;
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
            try
            {
                await commandFactory.Activity.Execute(input);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "An unhandled exception was thrown by the activity.");
                context.ExitCode = 1;
            }
            context.ExitCode = commandFactory.Context.ExitCode;
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
