using System.IO;
using Lineweights.Diagnostics.Verification;
using StudioLE.Core.Results;

namespace Lineweights.Diagnostics.Tests.Verification;

internal sealed class MockVerifyContext : IVerifyContext
{
    /// <inheritdoc/>
    public string FileNamePrefix { get; }

    /// <inheritdoc/>
    public DirectoryInfo Directory { get; }

    public MockVerifyContext(string fileNamePrefix)
    {
        FileNamePrefix = fileNamePrefix;
        Directory = VerifyTests.Directory;
    }

    /// <inheritdoc/>
    public void OnResult(IResult result, FileInfo receivedFile, FileInfo verifiedFile)
    {
        // if (AssemblyHelpers.IsDebugBuild())
        //     DiffRunner.LaunchAsync(receivedFile.FullName, verifiedFile.FullName);
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
