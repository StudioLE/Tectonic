using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Invocation;
using Cascade.Workflows.CommandLine.Composition;
using Microsoft.Extensions.Logging;
using StudioLE.Patterns;

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
            SetInputTreeValue(context.BindingContext, commandFactory);
            object input = commandFactory.InputTree.Instance;
            try
            {
                await commandFactory.Activity.Execute(input);
                context.ExitCode = commandFactory.Context.ExitCode;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "An unhandled exception was thrown by the activity.");
                context.ExitCode = 1;
            }
        };
    }

    private void SetInputTreeValue(BindingContext context, CommandFactory commandFactory)
    {
        ObjectTreeProperty[] objectTreeProperties = commandFactory
            .InputTree!
            .FlattenProperties()
            .ToArray();
        foreach (ObjectTreeProperty tree in objectTreeProperties)
        {
            if (!_isParsableStrategy.Execute(tree.Type))
                continue;
            if(tree.HasArgumentAttribute())
                SetInputTreeValueForArgument(context, commandFactory, tree);
            else
                SetInputTreeValueForOption(context, commandFactory, tree);
        }
    }

    private static void SetInputTreeValueForArgument(BindingContext context, CommandFactory commandFactory, ObjectTreeProperty tree)
    {
        if (!commandFactory.Arguments.TryGetValue(tree.Key.ToArgument(), out Argument? option))
            return;
        object? value = context.ParseResult.GetValueForArgument(option);
        if (value is null)
            return;
        if (!tree.Type.IsInstanceOfType(value))
            return;
        tree.SetValue(value);
    }

    private static void SetInputTreeValueForOption(BindingContext context, CommandFactory commandFactory, ObjectTreeProperty tree)
    {
        if (!commandFactory.Options.TryGetValue(tree.FullKey.ToLongOption(), out Option? option))
            return;
        object? value = context.ParseResult.GetValueForOption(option);
        if (value is null)
            return;
        if (!tree.Type.IsInstanceOfType(value))
            return;
        tree.SetValue(value);
    }
}
