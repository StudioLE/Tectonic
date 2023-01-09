using StudioLE.Core.Results;
using Lineweights.Workflows.Verification;
using StudioLE.Core.System;

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
        IResult result = await verifier.Execute(actual);
        if(result.Errors.Any())
            Console.WriteLine(result.Errors.Join());

        // Assert
        Assert.That(result is Success, "Is Success");
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
        IResult result = await verifier.Execute(actual);
        if(result.Errors.Any())
            Console.WriteLine(result.Errors.Join());

        // Assert
        Assert.That(result is Success, Is.False, "Is Success");
        Assert.That(result.Errors, Is.Not.Empty, "Errors");
    }
}
