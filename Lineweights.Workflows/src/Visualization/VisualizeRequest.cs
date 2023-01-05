using Lineweights.Core.Documents;
using Newtonsoft.Json;

namespace Lineweights.Workflows.Visualization;

[JsonConverter(typeof(VisualizeRequestConverter))]
public class VisualizeRequest
{
    public Asset Asset { get; set; } = new();

    public Model Model { get; set; } = new();

    public IReadOnlyCollection<string> Assemblies { get; set; } = Array.Empty<string>();
}
