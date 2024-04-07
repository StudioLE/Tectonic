using System.Reflection;

namespace Tectonic.StaticMethodActivities;

/// <summary>
/// An <see cref="IActivity"/> based on a static <see cref="MethodInfo"/> in an <see cref="Assembly"/>.
/// </summary>
public sealed class StaticMethodActivity<TInput, TOutput> : IActivity<TInput, TOutput>, IActivityMetadata
{
    private readonly MethodInfo _method;

    /// <inheritdoc />
    public string Key { get; }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public string Description { get; }

    /// <summary>
    /// Construct a <see cref="StaticMethodActivity{TInput,TOutput}"/> from <paramref name="method"/>.
    /// </summary>
    /// <param name="method">The method to be executed by the activity.</param>
    /// <param name="key">The unique key identifying the activity.</param>
    public StaticMethodActivity(MethodInfo method, string key)
    {
        _method = method;
        Key = key;
        Name = Key;
        Description = Key;
    }

    /// <inheritdoc />
    public Task<TOutput> Execute(TInput input)
    {
        object[] parameters = { input! };
        object output = _method.Invoke(null, parameters);
        return output switch
        {
            Task<TOutput> task => task,
            TOutput tOutput => Task.FromResult(tOutput),
            _ => throw new($"Expected a {nameof(TOutput)}.")
        };
    }

    public Type GetInputType()
    {
        ParameterInfo parameter = _method.GetParameters().First();
        return parameter.ParameterType;
    }

    public Type GetOutputType()
    {
        return _method.ReturnType;
    }
}
