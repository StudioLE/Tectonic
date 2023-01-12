﻿using Newtonsoft.Json;

namespace Lineweights.Workflows.Visualization;

[JsonConverter(typeof(VisualizeRequestConverter))]
public class VisualizeRequest
{
    public Model Model { get; set; } = new();

    public IReadOnlyCollection<string> Assemblies { get; set; } = Array.Empty<string>();
}
