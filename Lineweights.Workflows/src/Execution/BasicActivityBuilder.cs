using System.Reflection;
using Ardalis.Result;

namespace Lineweights.Workflows.Execution;

/// <inheritdoc cref="ActivityBuilder"/>
public class BasicActivityBuilder : ActivityBuilder
{
    public override Result<object> SetAssembly(Assembly assembly)
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

    public override Result<object> SetActivity(string activityName)
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

    public override Result<object> Build()
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
