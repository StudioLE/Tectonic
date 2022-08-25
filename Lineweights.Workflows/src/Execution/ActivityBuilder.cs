using System.Reflection;
using Ardalis.Result;

namespace Lineweights.Workflows.Execution;

/// <summary>
/// Build an <see cref="ActivityCommand"/> using a <see href="https://refactoring.guru/design-patterns/builder">builder pattern</see>.
/// </summary>
public abstract class ActivityBuilder
{
    /// <summary>
    /// The internal <see href="https://refactoring.guru/design-patterns/state">state</see> of the <see cref="ActivityBuilder"/>.
    /// </summary>
    public interface IState
    {
    }

    /// <inheritdoc cref="IState"/>
    public IState State { get; protected set; } = new InitialState();

    /// <summary>
    /// Set the assembly.
    /// </summary>
    public abstract Result<object> SetAssembly(Assembly assembly);

    /// <summary>
    /// Set the activity.
    /// </summary>
    public abstract Result<object> SetActivity(string activityName);

    /// <summary>
    /// Build the command.
    /// </summary>
    public abstract Result<object> Build();

    /// <summary>
    /// The initial state of the <see cref="BasicActivityBuilder"/>.
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
}
