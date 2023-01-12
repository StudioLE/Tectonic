using System.IO;
using Lineweights.Diagnostics.Verification;
using StudioLE.Core.Results;
using StudioLE.Core.System;

namespace Lineweights.Diagnostics.Tests.Verification;

internal sealed class FileVerifierTests
{
    [TestCase(".txt")]
    [TestCase(".pdf")]
    [TestCase(".bin")]
    public async Task FileVerifier_IsValid(string fileExtension)
    {
        // Arrange
        MockVerifyContext context = new("FileVerifier_Pass");
        FileInfo file = context.GetSourceFile(fileExtension);
        FileVerifier verifier = new(context, fileExtension);

        // Act
        IResult result = await verifier.Execute(file);
        if(result.Errors.Any())
            Console.WriteLine(result.Errors.Join());

        // Assert
        Assert.That(result is Success, "Is Success");
        Assert.That(result.Errors, Is.Empty, "Errors");
    }

    [TestCase(".txt")]
    [TestCase(".pdf")]
    [TestCase(".bin")]
    public async Task FileVerifier_IsInvalid(string fileExtension)
    {
        // Arrange
        MockVerifyContext context = new("FileVerifier_Fail");
        FileInfo file = context.GetSourceFile(fileExtension);
        FileVerifier verifier = new(context, fileExtension);

        // Act
        IResult result = await verifier.Execute(file);
        if(result.Errors.Any())
            Console.WriteLine(result.Errors.Join());

        // Assert
        Assert.That(result is Success, Is.False, "Is Success");
        Assert.That(result.Errors, Is.Not.Empty, "Errors");
    }
}
