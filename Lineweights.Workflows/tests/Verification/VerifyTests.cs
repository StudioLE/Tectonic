using System.IO;
using Lineweights.Workflows.NUnit.Verification;
using Lineweights.Workflows.Verification;

namespace Lineweights.Workflows.Tests.Verification;

internal sealed class VerifyTests
{
    public static readonly DirectoryInfo Directory = new(Path.Combine(TestContext.CurrentContext.TestDirectory, "..", "..", "..", "Verify"));

    [Test]
    public void Verify_GetContext()
    {
        // Arrange
        // Act
        IVerifyContext context = Verify.GetContext();

        // Asset
        Assert.That(context, Is.Not.Null, "IVerifyContext");
        Assert.That(context, Is.TypeOf<NUnitVerifyContext>(), "IVerifyContext");
    }
}
