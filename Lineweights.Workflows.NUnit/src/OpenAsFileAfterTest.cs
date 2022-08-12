using Lineweights.Workflows.Results;

namespace Lineweights.Workflows.NUnit;

/// <summary>
/// Open a <see cref="ResultModel"/> as a glb.
/// </summary>
public sealed class OpenAsFileAfterTest : ResultAfterTestAttribute
{
    /// <inheritdoc />
    public override IResultStrategy? Strategy { get; }

    /// <inheritdoc cref="OpenAsFileAfterTest"/>
    public OpenAsFileAfterTest(params string[] fileExtensions)
    {
        Strategy = new OpenAsFile(fileExtensions);
    }
}
