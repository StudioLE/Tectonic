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
        _verifiedFile = new(Path.Combine(_context.Directory.FullName, $"{_context.FileNamePrefix}.received{fileExtension}"));
    }

    /// <summary>
    /// Verify <paramref name="actual"/>.
    /// </summary>
    public Result<bool> Execute(T actual)
    {
        WriteActual(actual);
        if (AssemblyHelpers.IsDebugBuild())
            LaunchDiffEngine(_receivedFile, _verifiedFile);
        Result<bool> result = CompareEquality();
        if (!result.IsSuccess)
        {
            var diffResult = Diff();
            result = Result<bool>.Error(result.Errors.Concat(diffResult.Errors).ToArray());
        }
        _context.OnResult(result);
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

        // TODO: Compare via streams
        // https://stackoverflow.com/a/1359947/247218
        string actual = File.ReadAllText(_receivedFile.FullName);
        string verified = File.ReadAllText(_verifiedFile.FullName);

        // TODO: File.ReadLines removes the necessity to replace line endings!
        return actual
            .ReplaceWindowsLineEndings()
            .Equals(verified.ReplaceWindowsLineEndings());
    }

    protected abstract Result<bool> Diff();

    // TODO: DiffEngineTray SendMove
    // https://github.com/VerifyTests/DiffEngine/blob/3f2e942e73369a1d6c40d96b931d19651944b35f/src/DiffEngine/Tray/PiperClient.cs#L26-L33
    private static async void LaunchDiffEngine(FileInfo receivedFile, FileInfo verifiedFile)
    {
        await DiffRunner.LaunchAsync(receivedFile.FullName, verifiedFile.FullName);
    }
}
