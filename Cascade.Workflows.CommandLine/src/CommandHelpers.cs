using System.CommandLine;

namespace Cascade.Workflows.CommandLine;

public static class CommandHelpers
{
    private static Command? GetSubCommand(this Command @this, string name)
    {
        return @this
            .Subcommands
            .FirstOrDefault(x => x.Name == name);
    }

    private static Command GetOrCreateSubCommand(this Command @this, string name)
    {
        Command? subCommand = @this.GetSubCommand(name);
        if (subCommand is null)
        {
            subCommand = new(name);
            @this.AddCommand(subCommand);
        }
        return subCommand;
    }

    public static Command GetOrCreateSubCommand(this Command @this, string[] names)
    {
        if (!names.Any())
            throw new($"{nameof(GetOrCreateSubCommand)} failed. Names can't be empty.");
        Command command = @this;
        foreach (string name in names)
            command = command.GetOrCreateSubCommand(name);
        return command;
    }
}
