using System.IO;
using Ardalis.Result;

namespace Lineweights.Workflows.Verification;

/// <summary>
/// A lightweight alternative to <see href="https://github.com/VerifyTests/Verify"/> developed specifically with Elements in mind.
/// </summary>
/// <remarks>
/// Verifier is completely engine agnostic. It has no dependency on NUnit.
/// </remarks>
public abstract class VerifierBase<T>
{
    private readonly IVerifyContext _context;
    protected readonly FileInfo _receivedFile;
    protected readonly FileInfo _verifiedFile;

    /// <inheritdoc cref="VerifierBase{T}"/>
    protected VerifierBase(IVerifyContext context, string fileExtension)
    {
        _context = context;
        if (!context.Directory.Exists)
            throw new DirectoryNotFoundException($"Failed to Verify. The verify directory does not exist: {context.Directory.FullName}");
        _receivedFile = new(Path.Combine(_context.Directory.FullName, $"{_context.FileNamePrefix}.received{fileExtension}"));
        _verifiedFile = new(Path.Combine(_context.Directory.FullName, $"{_context.FileNamePrefix}.verified{fileExtension}"));
    }

    /// <summary>
    /// Verify <paramref name="actual"/>.
    /// </summary>
    public Result<bool> Execute(T actual)
    {
        WriteActual(actual);
        Result<bool> result = CompareEquality();
        _context.OnResult(result, _receivedFile, _verifiedFile);
        return result;
    }

    /// <summary>
    /// Write <paramref name="actual"/> to <see cref="_receivedFile"/>.
    /// </summary>
    protected abstract void WriteActual(T actual);

    /// <summary>
    /// Compare the equality of <see cref="_verifiedFile"/> with <see cref="_receivedFile"/>.
    /// </summary>
    private Result<bool> CompareEquality()
    {
        if(!_receivedFile.Exists)
            return Result<bool>.Error("The received file does not exist.");
        if(!_verifiedFile.Exists)
            return Result<bool>.Error("The verified file does not exist.");
        IEnumerable<string> actualLines  = File.ReadLines(_receivedFile.FullName, Verify.Encoding);
        IEnumerable<string> verifiedLines = File.ReadLines(_verifiedFile.FullName, Verify.Encoding);
        IEnumerable<(string Actual, string Verified)> lines = actualLines.Zip(verifiedLines, (actual, verified) => (actual, verified));
        int lineNumber = 1;
        foreach ((string Actual, string Verified) line in lines)
        {
            if (!line.Verified.Equals(line.Actual))
                return Result<bool>.Error($"Difference found on line {lineNumber}.", $"Actual  : {line.Actual}", $"Verified: {line.Verified}");
            lineNumber++;
        }
        return true;
    }
}
