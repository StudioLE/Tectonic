using System.IO;
using Ardalis.Result;
using DiffEngine;
using StudioLE.Core.System;

namespace Lineweights.Workflows.Verification;

/// <summary>
/// A lightweight alternative to <see href="https://github.com/VerifyTests/Verify"/> developed specifically with Elements in mind.
/// </summary>
/// <remarks>
/// Verifier is completely engine agnostic. It has no dependency on NUnit.
/// </remarks>
public abstract class VerifierBase<T>
{
    private readonly string _fileExtension;
    private readonly IVerifyContext _context;

    /// <inheritdoc cref="VerifierBase{T}"/>
    protected VerifierBase(IVerifyContext context, string fileExtension)
    {
        _fileExtension = fileExtension;
        _context = context;
        if (!context.Directory.Exists)
            throw new DirectoryNotFoundException($"Failed to Verify. The verify directory does not exist: {context.Directory.FullName}");
    }

    /// <summary>
    /// Verify <paramref name="actual"/>.
    /// </summary>
    public Result<bool> Execute(T actual)
    {
        Result<bool> result = Verify(actual);
        _context.OnResult(result);
        return result;
    }

    /// <summary>
    /// Verify <paramref name="actual"/>.
    /// </summary>
    protected abstract Result<bool> Verify(T actual);

    protected FileInfo ReceivedFile()
    {
        return new(Path.Combine(_context.Directory.FullName, $"{_context.FileNamePrefix}.received{_fileExtension}"));
    }

    protected FileInfo VerifiedFile()
    {
        return new(Path.Combine(_context.Directory.FullName, $"{_context.FileNamePrefix}.verified{_fileExtension}"));
    }

    // TODO: DiffEngineTray SendMove
    // https://github.com/VerifyTests/DiffEngine/blob/3f2e942e73369a1d6c40d96b931d19651944b35f/src/DiffEngine/Tray/PiperClient.cs#L26-L33
    protected static async void LaunchDiffEngine(FileInfo receivedFile, FileInfo verifiedFile)
    {
        await DiffRunner.LaunchAsync(receivedFile.FullName, verifiedFile.FullName);
    }

    protected static bool CompareStringsWithNormalizedLineEndings(string first, string second)
    {
        return first.ReplaceWindowsLineEndings().Equals(second.ReplaceWindowsLineEndings());
    }
}
