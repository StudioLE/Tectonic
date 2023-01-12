using Lineweights.Workflows.Visualization;

namespace Lineweights.Workflows.Documents;

public class ViewerComponentProvider
{
    private readonly Dictionary<string, Type> _contentTypes;

    public ViewerComponentProvider(VisualizationConfiguration configuration)
    {
        _contentTypes = configuration.ContentTypes;
    }

    public bool TryGetComponentType(string contentType, out Type component)
    {
        return _contentTypes.TryGetValue(contentType, out component);
    }
}
