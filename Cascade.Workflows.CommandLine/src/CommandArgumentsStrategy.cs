using System.CommandLine;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Cascade.Workflows.CommandLine.Composition;
using StudioLE.Extensions.System;
using StudioLE.Patterns;

namespace Cascade.Workflows.CommandLine;

public interface ICommandArgumentsStrategy : IStrategy<CommandFactory, IReadOnlyDictionary<string, Argument>>
{
}

public class CommandArgumentsStrategy : ICommandArgumentsStrategy
{
    private readonly IIsParseableStrategy _isParsableStrategy;

    public CommandArgumentsStrategy(IIsParseableStrategy isParsableStrategy)
    {
        _isParsableStrategy = isParsableStrategy;
    }

    public IReadOnlyDictionary<string, Argument> Execute(CommandFactory commandFactory)
    {
        if (commandFactory.InputTree is null)
            throw new("Expected tree to be set.");
        return commandFactory
            .InputTree
            .FlattenProperties()
            .Where(x => x.HasArgumentAttribute())
            .Where(x => _isParsableStrategy.Execute(x.Type))
            .Select(CreateArgumentForProperty)
            .ToDictionary(x => x.Name, x => x);
    }

    private static Argument CreateArgumentForProperty(ObjectTreeProperty tree)
    {
        Argument option = CreateInstanceOfArgument(tree);
        SetArgumentValidator(tree, option);
        return option;
    }

    private static Argument CreateInstanceOfArgument(ObjectTreeProperty tree)
    {
        string name = tree.Key.ToArgument();
        string description = tree.HelperText;
        Type optionType = typeof(Argument<>).MakeGenericType(tree.Type);
        object? instance = Activator.CreateInstance(optionType, name, description);
        if (instance is not Argument argument)
            throw new($"Failed to construct {nameof(Argument)}. Activator returned a {instance.GetType()}.");
        return argument;
    }

    private static void SetArgumentValidator(ObjectTreeProperty tree, Argument option)
    {
        ValidationAttribute[] validationAttributes = tree
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
