using System.CommandLine;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using StudioLE.Extensions.System;
using StudioLE.Patterns;
using StudioLE.Serialization.Composition;
using StudioLE.Serialization.Parsing;

namespace Tectonic.Extensions.CommandLine;

public interface ICommandArgumentsStrategy : IStrategy<CommandFactory, IReadOnlyDictionary<string, Argument>>
{
}

public class CommandArgumentsStrategy : ICommandArgumentsStrategy
{
    private readonly IParser _parser;

    public CommandArgumentsStrategy(IParser parser)
    {
        _parser = parser;
    }

    public IReadOnlyDictionary<string, Argument> Execute(CommandFactory commandFactory)
    {
        if (commandFactory.InputTree is null)
            throw new("Expected tree to be set.");
        return commandFactory
            .InputTree
            .FlattenProperties()
            .Where(x => x.CanSet())
            .Where(x => x.HasArgumentAttribute())
            .Where(x => _parser.CanParse(x.Type))
            .Select(CreateArgumentForProperty)
            .ToDictionary(x => x.Name, x => x);
    }

    private static Argument CreateArgumentForProperty(ObjectProperty property)
    {
        Argument option = CreateInstanceOfArgument(property);
        SetArgumentValidator(property, option);
        return option;
    }

    private static Argument CreateInstanceOfArgument(ObjectProperty property)
    {
        string name = property.Key.ToArgument();
        string description = property.HelperText;
        Type optionType = typeof(Argument<>).MakeGenericType(property.Type);
        object? instance = Activator.CreateInstance(optionType, name, description);
        if (instance is not Argument argument)
            throw new($"Failed to construct {nameof(Argument)}. Activator returned a {instance!.GetType()}.");
        return argument;
    }

    private static void SetArgumentValidator(ObjectProperty property, Argument option)
    {
        ValidationAttribute[] validationAttributes = property
            .Property
            .GetCustomAttributes<ValidationAttribute>()
            .ToArray();
        if (!validationAttributes.Any())
            return;
        option.AddValidator(result =>
        {
            object? value = result.GetValueForArgument(option);
            List<ValidationResult> results = new();
            ValidationContext context = new(value!)
            {
                DisplayName = result.Argument.Name
            };
            if (Validator.TryValidateValue(value!, context, results, validationAttributes))
                return;
            string message = results
                .Select(x => x.ErrorMessage)
                .OfType<string>()
                .Join();
            result.ErrorMessage = message;
        });
    }
}
