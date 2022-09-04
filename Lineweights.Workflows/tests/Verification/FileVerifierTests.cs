using System.IO;
using Ardalis.Result;
using Lineweights.Workflows.Verification;

namespace Lineweights.Workflows.Tests.Verification;

internal sealed class FileVerifierTests
{
    [TestCase(".txt")]
    [TestCase(".pdf")]
    [TestCase(".bin")]
    public void FileVerifier_IsValid(string fileExtension)
    {
        // Arrange
        MockVerifyContext context = new("FileVerifier_Pass");
        FileInfo file = context.GetSourceFile(fileExtension);
        FileVerifier verifier = new(context, fileExtension);

        // Act
        Result<bool> result = verifier.Execute(file);
        foreach (string error in result.Errors)
            Console.WriteLine(error);

        // Assert
        Assert.That(result.IsSuccess, Is.True, "IsSuccess");
        Assert.That(result.Errors, Is.Empty, "Errors");
    }

    [TestCase(".txt")]
    [TestCase(".pdf")]
    [TestCase(".bin")]
    public void FileVerifier_IsInvalid(string fileExtension)
    {
        // Arrange
        MockVerifyContext context = new("FileVerifier_Fail");
        FileInfo file = context.GetSourceFile(fileExtension);
        FileVerifier verifier = new(context, fileExtension);

        // Act
        Result<bool> result = verifier.Execute(file);
        foreach (string error in result.Errors)
            Console.WriteLine(error);

        // Assert
        Assert.That(result.IsSuccess, Is.False, "IsSuccess");
        Assert.That(result.Errors, Is.Not.Empty, "Errors");
    }
}
