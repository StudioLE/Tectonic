using Geometrician.Core.Assets;
using Lineweights.Core.Assets;
using Microsoft.AspNetCore.Components;

namespace Geometrician.Components.Visualization;

/// <summary>
/// A <see cref="IComponent"/> to dynamically determine which viewer should be used to render an <see cref="IAsset"/>.
/// </summary>
public class DynamicViewerComponentBase : ViewerComponentBase<IAsset>
{
    /// <inheritdoc cref="ViewerComponentProvider"/>
    [Inject]
    private ViewerComponentProvider Provider { get; set; } = default!;

    /// <summary>
    /// The <see cref="Type"/> of the viewer <see cref="IComponent"/> to use.
    /// </summary>
    protected Type ComponentType { get; private set; } = default!;

    /// <summary>
    /// The parameters to pass to the viewer <see cref="IComponent"/>.
    /// </summary>
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
