using Lineweights.Core.Serialisation;
using Newtonsoft.Json;

namespace Lineweights.Workflows.Containers;

/// <summary>
/// <para>
/// An information container is a uniquely identified document that contains information related to built assets.
/// The convention is defined in ISO 19650.
/// </para>
/// <para>
/// <see cref="ContainerBuilder"/> provides logic to create a container of a <see cref="Model"/>.
/// </para>
/// <para>
/// The content of a container is defined by either its <see cref="Content"/> property or obtained from the
/// <see cref="DocumentInformation.Location"/> property of <see cref="Info"/>.
/// </para>
/// <para>
/// A container can be a <see href="https://refactoring.guru/design-patterns/composite">composite</see> of <see cref="Children"/>.
/// </para>
/// </summary>
/// <remarks>
/// Theoretically this is similar to and could be implemented as an
/// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.container?view=net-6.0">IContainer</see>
/// of <see href="https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.icomponent?view=net-6.0">IComponent</see>
/// </remarks>
public class Container
{
    /// <summary>
    /// The document information.
    /// </summary>
    [JsonConverter(typeof(OverrideInheritanceConverter))]
    public DocumentInformation Info { get; set; } = new();

    /// <summary>
    /// The uri of any additional files.
    /// </summary>
    public IReadOnlyCollection<Container> Children { get; set; } = Array.Empty<Container>();

    /// <summary>
    /// The <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types/Common_types">MIME type</see>.
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// The content (if explicitly defined).
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// Errors occured when producing the container.
    /// </summary>
    public IReadOnlyCollection<string> Errors { get; set; } = Array.Empty<string>();
}
