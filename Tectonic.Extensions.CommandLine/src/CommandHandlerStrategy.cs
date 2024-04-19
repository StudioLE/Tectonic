using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Reflection;
using Microsoft.Extensions.Logging;
using StudioLE.Patterns;
using StudioLE.Serialization.Composition;
using StudioLE.Serialization.Parsing;

namespace Tectonic.Extensions.CommandLine;

public interface ICommandHandlerStrategy : IStrategy<CommandFactory, Func<InvocationContext, Task>>
{
}

public class CommandHandlerStrategy : ICommandHandlerStrategy
{
    private readonly ILogger<CommandHandlerStrategy> _logger;
    private readonly IParser _parser;

    public CommandHandlerStrategy(ILogger<CommandHandlerStrategy> logger, IParser parser)
    {
        _logger = logger;
        _parser = parser;
    }

    public Func<InvocationContext, Task> Execute(CommandFactory commandFactory)
    {
        if (commandFactory.InputTree is null)
            throw new("Expected input tree to be set.");
        return async context =>
        {
            SetInputTreeValue(context.BindingContext, commandFactory);
            object? input = commandFactory.InputTree.GetValue();
            if (input is null)
                throw new("Expected input to be set.");
            object? output;
            try
            {
                output = await commandFactory.Activity.ExecuteNonGeneric(input);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "An unhandled exception was thrown by the activity.");
                _logger.LogError(e.GetType() + ": " + e.Message);
                context.ExitCode = 1;
                return;
            }
            try
            {
                if (output is null)
                    return;
                Type outputType = output.GetType();
                foreach (PropertyInfo property in outputType.GetProperties())
                {
                    if (property.PropertyType != typeof(Status))
                        continue;
                    object? value = property.GetValue(output);
                    if (value is not Status status)
                        throw new($"Expected a {nameof(Status)} but received {value?.GetType()}.");
                    context.ExitCode = status.ExitCode;
                    break;
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "An unhandled exception was thrown when processing the the activity outputs.");
                _logger.LogError($"{e.GetType()}: {e.Message}");
                context.ExitCode = 1;
            }
        };
    }

    private void SetInputTreeValue(BindingContext context, CommandFactory commandFactory)
    {
        ObjectProperty[] objectProperties = commandFactory
            .InputTree!
            .FlattenProperties()
            .Where(x => x.CanSet())
            .ToArray();
        foreach (ObjectProperty property in objectProperties)
        {
            if (!_parser.CanParse(property.Type))
                continue;
            if (property.HasArgumentAttribute())
                SetInputTreeValueForArgument(context, commandFactory, property);
            else
                SetInputTreeValueForOption(context, commandFactory, property);
        }
    }

    private void SetInputTreeValueForArgument(BindingContext context, CommandFactory commandFactory, ObjectProperty property)
    {
        if (!commandFactory.Arguments.TryGetValue(property.Key.ToArgument(), out Argument? option))
            return;
        object? value = context.ParseResult.GetValueForArgument(option);
        SetInputTreeValue(property, value);
    }

    private void SetInputTreeValueForOption(BindingContext context, CommandFactory commandFactory, ObjectProperty property)
    {
        if (!commandFactory.Options.TryGetValue(property.FullKey.ToLongOption(), out Option? option))
            return;
        object? value = context.ParseResult.GetValueForOption(option);
        SetInputTreeValue(property, value);
    }

    private void SetInputTreeValue(ObjectProperty property, object? value)
    {
        if (value is null)
            return;
        if (property.Type.IsInstanceOfType(value))
        {
            property.SetValue(value);
            return;
        }
        if (value is not Token token)
            return;
        if (token.Value is not string stringValue)
            return;
        value = _parser.Parse(stringValue, property.Type);
        property.SetValue(value);
    }
}
