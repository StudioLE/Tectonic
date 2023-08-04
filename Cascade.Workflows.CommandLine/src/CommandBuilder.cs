using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using StudioLE.Core.Patterns;

namespace Cascade.Workflows.CommandLine;

public class CommandBuilder : IBuilder<RootCommand>, IDisposable
{
    private readonly IServiceScope _scope;
    private readonly IServiceProvider _services;
    private readonly List<KeyValuePair<string[], IActivity>> _activities = new();

    public CommandBuilder(IServiceProvider services)
    {
        _scope = services.CreateScope();
        _services = _scope.ServiceProvider;
    }

    public CommandBuilder Register<TActivity>(params string[] command) where TActivity : IActivity
    {
        TActivity activity = _services.GetRequiredService<TActivity>();
        _activities.Add(new(command, activity));
        return this;
    }

    /// <inheritdoc/>
    public RootCommand Build()
    {
        RootCommand root = new();
        foreach (KeyValuePair<string[], IActivity> pair in _activities)
        {
            Command command = CreateCommand(pair.Value);
            if(pair.Key.Any())
                command.Name = pair.Key.Last();
            Command parentCommand = GetParentCommand(root, pair.Key);
            parentCommand.AddCommand(command);
        }
        return root;
    }

    private static Command GetParentCommand(RootCommand root, string[] keys)
    {
        string[] parentKeys = keys.SkipLast(1).ToArray();
        return parentKeys.Any()
            ? root.GetOrCreateSubCommand(parentKeys)
            : root;
    }

    private Command CreateCommand(IActivity activity)
    {
        CommandFactory factory = _services.GetRequiredService<CommandFactory>();
        return factory.Create(activity);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _scope.Dispose();
    }
}
