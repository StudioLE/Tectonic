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
    protected FileInfo _receivedFile;
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
    public async Task<Result<bool>> Execute(T actual)
    {
        await WriteActual(actual);
        Result<bool> result = await CompareEquality();
        _context.OnResult(result, _receivedFile, _verifiedFile);
        return result;
    }

    /// <summary>
    /// Write <paramref name="actual"/> to <see cref="_receivedFile"/>.
    /// </summary>
    protected abstract Task WriteActual(T actual);

    /// <summary>
    /// Compare the equality of <see cref="_verifiedFile"/> with <see cref="_receivedFile"/>.
    /// </summary>
    protected virtual async Task<Result<bool>> CompareEquality()
    {
        if(!_receivedFile.Exists)
            return Result<bool>.Error("The received file does not exist.");
        if(!_verifiedFile.Exists)
            return Result<bool>.Error("The verified file does not exist.");
        using StreamReader actualReader = new(_receivedFile.FullName, Verify.Encoding);
        using StreamReader verifiedReader = new(_verifiedFile.FullName, Verify.Encoding);
        int lineNumber = 1;
        int actualPeek = actualReader.Peek();
        int verifiedPeek = verifiedReader.Peek();
        string? actual;
        string? verified;
        while (actualPeek >= 0 && verifiedPeek >= 0)
        {
            actual = await actualReader.ReadLineAsync();
            verified = await verifiedReader.ReadLineAsync();
            if (!actual.Equals(verified))
                return Result<bool>.Error($"Difference found on line {lineNumber}.", $"Actual  : {actual}", $"Verified: {verified}");
            actualPeek = actualReader.Peek();
            verifiedPeek = verifiedReader.Peek();
            lineNumber++;
        }
        if(actualPeek >= 0)
            return Result<bool>.Error("Line counts don't match. Actual still has lines.");
        if(verifiedPeek >= 0)
            return Result<bool>.Error("Line counts don't match. Actual still has lines.");
        return true;
    }
}
