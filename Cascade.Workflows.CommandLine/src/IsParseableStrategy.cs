using StudioLE.Core.Patterns;

namespace Cascade.Workflows.CommandLine;

public interface IIsParseableStrategy : IStrategy<Type, bool>
{
}

public class IsParseableStrategy : IIsParseableStrategy
{
    private readonly HashSet<Type> _parseableTypes = new()
    {
        typeof(string),
        typeof(int),
        typeof(double),
        typeof(Enum)
    };

    public IsParseableStrategy()
    {
    }

    public IsParseableStrategy(HashSet<Type> parseableTypes)
    {
        _parseableTypes = parseableTypes;
    }

    /// <inheritdoc />
    public bool Execute(Type type)
    {
        return _parseableTypes.Contains(type)
               || _parseableTypes.Any(x => x.IsAssignableFrom(type));
    }
}
