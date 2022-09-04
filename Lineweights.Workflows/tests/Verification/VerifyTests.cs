using System.IO;
using Ardalis.Result;
using Lineweights.Workflows.NUnit.Verification;
using Lineweights.Workflows.Verification;
using Newtonsoft.Json;

namespace Lineweights.Workflows.Tests.Verification;

internal sealed class VerifyTests
{
    private readonly DirectoryInfo _directory = new(Path.Combine(TestContext.CurrentContext.TestDirectory, "..", "..", "..", "Verify"));

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

    [Test]
    public void StringVerifier_IsValid()
    {
        // Arrange
        IVerifyContext context = new MockVerifyContext(_directory, "StringVerifier_Pass");
        StringVerifier verifier = new(context, ".txt");
        FileInfo sourceFile = new(Path.Combine(_directory.FullName, "StringVerifier_Pass.source.txt"));
        if (!sourceFile.Exists)
            throw new FileNotFoundException("Source file was not found");
        string actual = File.ReadAllText(sourceFile.FullName, Verify.Encoding);

        // Act
        Result<bool> result = verifier.Execute(actual);
        foreach (string error in result.Errors)
            Console.WriteLine(error);

        // Assert
        Assert.That(result.IsSuccess, Is.True, "IsSuccess");
        Assert.That(result.Errors, Is.Empty, "Errors");
    }

    [Test]
    public void StringVerifier_IsInvalid()
    {
        // Arrange
        IVerifyContext context = new MockVerifyContext(_directory, "StringVerifier_Fail");
        StringVerifier verifier = new(context, ".txt");
        FileInfo source = new(Path.Combine(_directory.FullName, "StringVerifier_Fail.source.txt"));
        if (!source.Exists)
            throw new FileNotFoundException("Source file was not found");
        string actual = File.ReadAllText(source.FullName, Verify.Encoding);

        // Act
        Result<bool> result = verifier.Execute(actual);
        foreach (string error in result.Errors)
            Console.WriteLine(error);

        // Assert
        Assert.That(result.IsSuccess, Is.False, "IsSuccess");
        Assert.That(result.Errors, Is.Not.Empty, "Errors");
    }

    [Test]
    public void JsonVerifier_IsValid()
    {
        // Arrange
        IVerifyContext context = new MockVerifyContext(_directory, "JsonVerifier_Pass");
        JsonVerifier verifier = new(context);
        FileInfo sourceFile = new(Path.Combine(_directory.FullName, "JsonVerifier_Pass.source.json"));
        if (!sourceFile.Exists)
            throw new FileNotFoundException("Source file was not found");
        string actualJson = File.ReadAllText(sourceFile.FullName, Verify.Encoding);

        // Act
        BBox3[]? actual = JsonConvert.DeserializeObject<BBox3[]>(actualJson, JsonVerifier.Converters);
        if (actual is null)
            throw new("Failed to de-serialize.");
        Result<bool> result = verifier.Execute(actual);
        foreach (string error in result.Errors)
            Console.WriteLine(error);

        // Assert
        Assert.That(result.IsSuccess, Is.True, "IsSuccess");
        Assert.That(result.Errors, Is.Empty, "Errors");
    }

    [Test]
    public void JsonVerifier_IsInvalid()
    {
        // Arrange
        IVerifyContext context = new MockVerifyContext(_directory, "JsonVerifier_Fail");
        JsonVerifier verifier = new(context);
        FileInfo source = new(Path.Combine(_directory.FullName, "JsonVerifier_Fail.source.json"));
        if (!source.Exists)
            throw new FileNotFoundException("Source file was not found");
        string actualJson = File.ReadAllText(source.FullName, Verify.Encoding);

        // Act
        BBox3[]? actual = JsonConvert.DeserializeObject<BBox3[]>(actualJson, JsonVerifier.Converters);
        if (actual is null)
            throw new("Failed to de-serialize.");
        Result<bool> result = verifier.Execute(actual);
        foreach (string error in result.Errors)
            Console.WriteLine(error);

        // Assert
        Assert.That(result.IsSuccess, Is.False, "IsSuccess");
        Assert.That(result.Errors, Is.Not.Empty, "Errors");
    }

    private class MockVerifyContext : IVerifyContext
    {
        /// <inheritdoc />
        public string FileNamePrefix { get; }

        /// <inheritdoc />
        public DirectoryInfo Directory { get; }

        /// <inheritdoc />
        public void OnResult(Result<bool> result, FileInfo receivedFile, FileInfo verifiedFile)
        {
            // if (AssemblyHelpers.IsDebugBuild())
            //     DiffRunner.LaunchAsync(receivedFile.FullName, verifiedFile.FullName);
        }

        public MockVerifyContext(DirectoryInfo directory, string fileNamePrefix)
        {
            FileNamePrefix = fileNamePrefix;
            Directory = directory;
        }
    }
}
