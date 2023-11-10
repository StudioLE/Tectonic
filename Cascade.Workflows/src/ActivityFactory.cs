namespace Cascade.Workflows;

public static class ActivityFactory
{
    /// <summary>
    /// Construct a <see cref="IActivity{TInput,TOutput}"/>.
    /// </summary>
    public static IActivity Create(Type type)
    {
        // TODO: Replace this with DI creation. Or implement as separate IFactory<Type, IActivity>
        object? createdInstance = Activator.CreateInstance(type);
        if (createdInstance is not IActivity activity)
            throw new($"Failed to construct activity. Activator didn't return an {nameof(IActivity)}.");
        return activity;
    }
}
