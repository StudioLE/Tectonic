using System.Reflection;
using Ardalis.Result;

namespace Lineweights.Workflows.Execution;

/// <summary>
/// Build an <see cref="ActivityCommand"/> using a <see href="https://refactoring.guru/design-patterns/builder">builder pattern</see>.
/// </summary>
public class ActivityBuilder
{
    #region States

    /// <summary>
    /// The internal <see href="https://refactoring.guru/design-patterns/state">state</see> of the <see cref="ActivityBuilder"/>.
    /// </summary>
    public interface IState
    {
    }

    /// <inheritdoc cref="IState"/>
    public IState State { get; protected set; } = new InitialState();

    /// <summary>
    /// The initial state of the <see cref="ActivityBuilder"/>.
    /// </summary>
    public class InitialState : IState
    {
    }

    /// <summary>
    /// The state of the <see cref="ActivityBuilder"/> when the assembly has been set.
    /// </summary>
    public class AssemblySetState : InitialState
    {
        internal Assembly Assembly { get; init; } = null!;

        internal IReadOnlyDictionary<string, MethodInfo> Activities { get; init; } = null!;

        /// <summary>
        /// The name of the currently loaded assembly.
        /// </summary>
        public string AssemblyName { get; internal init; } = null!;

        /// <summary>
        /// The names of the activities loaded from the assembly.
        /// </summary>
        public IReadOnlyCollection<string> ActivityNames { get; internal init; } = null!;
    }

    /// <summary>
    /// The state of the <see cref="ActivityBuilder"/> when the activity has been set.
    /// </summary>
    public class ActivitySetState : AssemblySetState
    {
        /// <summary>
        /// The name of the selected activity
        /// </summary>
        internal MethodInfo Activity { get; init; } = null!;

        /// <summary>
        /// The name of the selected activity
        /// </summary>
        public string ActivityName { get; internal init; } = null!;

        /// <summary>
        /// The inputs for the activity.
        /// </summary>
        public IReadOnlyCollection<object> Inputs { get; internal init; } = null!;
    }

    /// <summary>
    /// The state of the <see cref="ActivityBuilder"/> when the activity has been built.
    /// </summary>
    public class BuiltState : ActivitySetState
    {
        /// <summary>
        /// The inputs for the activity.
        /// </summary>
        public ActivityCommand Command { get; internal init; } = null!;
    }

    #endregion


    public virtual Result<object> SetAssembly(Assembly assembly)
    {
        if (State is not InitialState state)
            return Result<object>.Error($"{nameof(SetAssembly)} can only be called when {nameof(State)} is {nameof(InitialState)}.");

        string assemblyName = assembly.GetName().Name;

        Type[] types = assembly
            .GetTypes()
            .Where(type => type.IsPublic
                           && type.IsClass)
            .ToArray();

        MethodInfo[] methods =  types
            .SelectMany(type => type
                .GetMethods()
                .Where(method => method.DeclaringType == type
                                 && method.IsPublic
                                 && method.IsStatic
                                 && !method.IsAbstract
                                 && !method.IsVirtual))
            .ToArray();

        Dictionary<string, MethodInfo> activities = methods.ToDictionary(method =>
        {
            string @namespace = method.DeclaringType?.FullName ?? string.Empty;
            if (@namespace.StartsWith(assemblyName))
                @namespace = @namespace.Remove(0, assemblyName.Length + 1);
            return $"{@namespace}.{method.Name}";
        });
        string[] activityNames = activities.Keys.ToArray();

        State = new AssemblySetState
        {
            Assembly = assembly,
            AssemblyName = assemblyName,
            Activities = activities,
            ActivityNames = activityNames
        };

        return true;
    }

    public virtual Result<object> SetActivity(string activityName)
    {
        if (State is not AssemblySetState state)
            return Result<object>.Error($"{nameof(SetActivity)} can only be called when {nameof(State)} is {nameof(AssemblySetState)}.");

        if (!state.Activities.TryGetValue(activityName, out MethodInfo activity))
            return Result<object>.Error("The activity does not exist.");

        ParameterInfo[] parameters = activity.GetParameters();

        object[] inputs =  parameters
            .Select(x => Activator.CreateInstance(x.ParameterType))
            .ToArray();

        State = new ActivitySetState
        {
            Assembly = state.Assembly,
            AssemblyName = state.AssemblyName,
            Activities = state.Activities,
            ActivityNames = state.ActivityNames,
            Activity = activity,
            ActivityName = activityName,
            Inputs = inputs
        };

        return true;
    }

    public virtual Result<object> Build()
    {
        if (State is not ActivitySetState state)
            return Result<object>.Error($"{nameof(Build)} can only be called when {nameof(State)} is {nameof(ActivitySetState)}.");

        object[] inputs = state.Inputs.ToArray();

        Func<object[], object> function = x => state.Activity.Invoke(null, x);
        ActivityCommand command = new(state.ActivityName, inputs, function);

        State = new BuiltState
        {
            Assembly = state.Assembly,
            AssemblyName = state.AssemblyName,
            Activities = state.Activities,
            ActivityNames = state.ActivityNames,
            Activity = state.Activity,
            ActivityName = state.ActivityName,
            Inputs = state.Inputs,
            Command = command,
        };

        return true;
    }
}
