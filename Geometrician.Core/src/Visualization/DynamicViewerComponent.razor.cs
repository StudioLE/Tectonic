using Lineweights.Core.Documents;
using Lineweights.Workflows.Documents;
using Microsoft.AspNetCore.Components;

namespace Geometrician.Core.Visualization;

public class DynamicViewerComponentBase : ViewerComponentBase<IAsset>
{
    /// <inheritdoc cref="ViewerComponentProvider"/>
    [Inject]
    private ViewerComponentProvider Provider { get; set; } = default!;

    protected Type ComponentType { get; private set; } = default!;

    protected Dictionary<string, object> ComponentParameters { get; } = new();

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        ComponentParameters.Add("Factory", Factory);
        if (Provider.TryGetComponentType(Factory.Asset.ContentType, out Type component))
            ComponentType = component;
        else if (Factory.Asset.ContentType.StartsWith("text/"))
            ComponentType = typeof(CodeViewerComponent);
        else if (Factory.Asset.ContentType.StartsWith("image/"))
            ComponentType = typeof(ImageViewerComponent);
        else if(Factory.Asset is ExternalAsset)
            ComponentType = typeof(UriViewerComponent);
        else
            ComponentType = typeof(CodeViewerComponent);
    }
}
