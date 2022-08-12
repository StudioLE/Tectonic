using Lineweights.Workflows.Results;

namespace Lineweights.Workflows.NUnit;

/// <summary>
/// Send the model to the dashboard to be sampled.
/// </summary>
public sealed class SendToDashboardAfterTest : ResultAfterTestAttribute
{
    /// <inheritdoc />
    public override IResultStrategy? Strategy { get; }

    /// <inheritdoc cref="SendToDashboardAfterTest"/>
    public SendToDashboardAfterTest(string url = SendToDashboard.HubUrl)
    {
        Strategy = new SendToDashboard(url);
    }
}
