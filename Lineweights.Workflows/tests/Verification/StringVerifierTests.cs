using Ardalis.Result;
using Lineweights.Workflows.Verification;

namespace Lineweights.Workflows.Tests.Verification;

internal sealed class StringVerifierTests
{
    [Test]
    public async Task StringVerifier_IsValid()
    {
        // Arrange
        MockVerifyContext context = new("StringVerifier_Pass");
        StringVerifier verifier = new(context, ".txt");
        string actual = context.ReadSourceFile(".txt");

        // Act
        Result<bool> result = await verifier.Execute(actual);
        foreach (string error in result.Errors)
            Console.WriteLine(error);

        // Assert
        Assert.That(result.IsSuccess, Is.True, "IsSuccess");
        Assert.That(result.Errors, Is.Empty, "Errors");
    }

    [Test]
    public async Task StringVerifier_IsInvalid()
    {
        // Arrange
        MockVerifyContext context = new("StringVerifier_Fail");
        StringVerifier verifier = new(context, ".txt");
        string actual = context.ReadSourceFile(".txt");

        // Act
        Result<bool> result = await verifier.Execute(actual);
        foreach (string error in result.Errors)
            Console.WriteLine(error);

        // Assert
        Assert.That(result.IsSuccess, Is.False, "IsSuccess");
        Assert.That(result.Errors, Is.Not.Empty, "Errors");
    }
}
