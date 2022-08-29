using Lineweights.Workflows.Results;

namespace Lineweights.Workflows.NUnit;

/// <summary>
/// Send the model to the server to be sampled.
/// </summary>
public sealed class SendToServerAfterTest : ResultAfterTestAttribute
{
    /// <inheritdoc />
    public override IResultStrategy? Strategy { get; }

    /// <inheritdoc cref="SendToServerAfterTest"/>
    public SendToServerAfterTest()
    {
        Strategy = new SendToServer();
    }
}
