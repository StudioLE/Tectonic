using System.IO;
using StudioLE.Core.Results;
using Lineweights.Workflows.Verification;

namespace Lineweights.Workflows.Tests.Verification;

internal sealed class MockVerifyContext : IVerifyContext
{
    /// <inheritdoc />
    public string FileNamePrefix { get; }

    /// <inheritdoc />
    public DirectoryInfo Directory { get; }

    /// <inheritdoc />
    public void OnResult(IResult result, FileInfo receivedFile, FileInfo verifiedFile)
    {
        // if (AssemblyHelpers.IsDebugBuild())
        //     DiffRunner.LaunchAsync(receivedFile.FullName, verifiedFile.FullName);
    }

    public MockVerifyContext(string fileNamePrefix)
    {
        FileNamePrefix = fileNamePrefix;
        Directory = VerifyTests.Directory;
    }

    internal FileInfo GetSourceFile(string fileExtension)
    {
        return new(Path.Combine(Directory.FullName, $"{FileNamePrefix}.source{fileExtension}"));
    }

    internal string ReadSourceFile(string fileExtension)
    {
        FileInfo sourceFile = GetSourceFile(fileExtension);
        if (!sourceFile.Exists)
            throw new FileNotFoundException("Source file was not found: " + sourceFile.FullName);
        return File.ReadAllText(sourceFile.FullName, Verify.Encoding);
    }
}
