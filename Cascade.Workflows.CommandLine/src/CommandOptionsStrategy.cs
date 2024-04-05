using System.CommandLine;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using StudioLE.Extensions.System;
using StudioLE.Patterns;
using StudioLE.Serialization.Composition;
using StudioLE.Serialization.Parsing;

namespace Cascade.Workflows.CommandLine;

public interface ICommandOptionsStrategy : IStrategy<CommandFactory, IReadOnlyDictionary<string, Option>>
{
}

public class CommandOptionsStrategy : ICommandOptionsStrategy
{
    private readonly HashSet<string> _optionAliases = new();
    private readonly IParser _parser;

    public CommandOptionsStrategy(IParser parser)
    {
        _parser = parser;
    }

    public IReadOnlyDictionary<string, Option> Execute(CommandFactory commandFactory)
    {
        if (commandFactory.InputTree is null)
            throw new("Expected tree to be set.");
        return commandFactory
            .InputTree
            .FlattenProperties()
            .Where(x => x.CanSet())
            .Where(x => !x.HasArgumentAttribute())
            .Where(x => _parser.CanParse(x.Type))
            .Select(CreateOptionForProperty)
            .ToDictionary(option => option.Aliases.First(), option => option);
    }

    private Option CreateOptionForProperty(ObjectProperty property)
    {
        HashSet<string> aliases = GetAliases(property);
        Option option = CreateInstanceOfOption(property, aliases);
        SetOptionValidator(property, option);
        foreach (string alias in aliases)
            _optionAliases.Add(alias);
        return option;
    }

    private HashSet<string> GetAliases(ObjectProperty property)
    {
        HashSet<string> aliases = new()
        {
            property.FullKey.ToLongOption()
        };
        if (!_optionAliases.Contains(property.Key.ToLongOption()))
            aliases.Add(property.Key.ToLongOption());
        if (property.Parent is ObjectProperty parent)
        {
            if (!_optionAliases.Contains(parent.FullKey.ToLongOption()))
                aliases.Add(parent.FullKey.ToLongOption());
            if (!_optionAliases.Contains(parent.Key.ToLongOption()))
                aliases.Add(parent.Key.ToLongOption());
        }
        return aliases;
    }

    private static Option CreateInstanceOfOption(ObjectProperty property, IReadOnlyCollection<string> aliases)
    {
        string[] aliasesArray = aliases.ToArray();
        string description = property.HelperText;
        Type optionType = typeof(Option<>).MakeGenericType(property.Type);
        object? instance = Activator.CreateInstance(optionType, aliasesArray, description);
        if (instance is not Option option)
            throw new($"Failed to construct {nameof(Option)}. Activator returned a {instance!.GetType()}.");
        return option;
    }

    private static void SetOptionValidator(ObjectProperty property, Option option)
    {
        ValidationAttribute[] validationAttributes = property
            .Property
            .GetCustomAttributes<ValidationAttribute>()
            .ToArray();
        if (!validationAttributes.Any())
            return;
        option.AddValidator(result =>
        {
            object? value = result.GetValueForOption(option);
            List<ValidationResult> results = new();
            ValidationContext context = new(value!)
            {
                DisplayName = result.Token?.Value ?? throw new("Failed to get the value of the token.")
                // DisplayName = option.Description ?? throw new("Failed to get the value of the token.")
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
