using System.Reflection;
using Ardalis.Result;

namespace Lineweights.Workflows.Execution;

public interface IActivityRunner
{
    public Result<IReadOnlyCollection<string>> ExtractActivities(Assembly assembly);

    public Result<object> Execute(string activityId);
}
