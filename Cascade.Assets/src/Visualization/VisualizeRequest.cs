using Elements;
using Newtonsoft.Json;

namespace Cascade.Assets.Visualization;

[JsonConverter(typeof(VisualizeRequestConverter))]
public class VisualizeRequest
{
    public Model Model { get; set; } = new();

    public IReadOnlyCollection<string> Assemblies { get; set; } = Array.Empty<string>();
}
