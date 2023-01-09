using System.IO;
using StudioLE.Core.Results;

namespace Lineweights.Workflows.Verification;

/// <summary>
/// A lightweight alternative to <see href="https://github.com/VerifyTests/Verify"/> developed specifically with Elements in mind.
/// </summary>
/// <remarks>
/// Verifier is completely engine agnostic. It has no dependency on NUnit.
/// </remarks>
public abstract class VerifierBase<T>
{
    protected readonly IVerifyContext _context;
    protected readonly string _fileExtension;

    /// <inheritdoc cref="VerifierBase{T}"/>
    protected VerifierBase(IVerifyContext context, string fileExtension)
    {
        _context = context;
        _fileExtension = fileExtension;
        if (!context.Directory.Exists)
            throw new DirectoryNotFoundException($"Failed to Verify. The verify directory does not exist: {context.Directory.FullName}");
    }

    /// <summary>
    /// Verify <paramref name="actual"/>.
    /// </summary>
    public virtual async Task<IResult> Execute(T actual)
    {
        FileInfo receivedFile = new(Path.Combine(_context.Directory.FullName, $"{_context.FileNamePrefix}.received{_fileExtension}"));
        FileInfo verifiedFile = new(Path.Combine(_context.Directory.FullName, $"{_context.FileNamePrefix}.verified{_fileExtension}"));
        await Write(receivedFile, actual);
        KeyValuePair<string, FileInfo>[] files =
        {
            new("Verified", verifiedFile),
            new("Received", receivedFile)
        };
        IResult result = await Compare(files);
        _context.OnResult(result, receivedFile, verifiedFile);
        return result;
    }

    /// <summary>
    /// Verify <paramref name="actual"/> matches <paramref name="expected"/>.
    /// </summary>
    public async Task<IResult> Execute(T expected, T actual)
    {
        FileInfo expectedFile = new(Path.GetTempFileName() + ".expected.txt");
        FileInfo actualFile = new(Path.GetTempFileName() + ".actual.txt");
        await Write(expectedFile, expected);
        await Write(actualFile, actual);
        KeyValuePair<string, FileInfo>[] files =
        {
            new("Expected", expectedFile),
            new("Actual", actualFile)
        };
        IResult result = await Compare(files);
        _context.OnResult(result, actualFile, expectedFile);
        return result;
    }

    /// <summary>
    /// Write <paramref name="value"/> to <paramref name="file"/>.
    /// </summary>
    protected abstract Task Write(FileInfo file, T value);

    /// <summary>
    /// Compare the equality of the contents of <paramref name="files"/>.
    /// </summary>
    protected virtual async Task<IResult> Compare(params KeyValuePair<string , FileInfo>[] files)
    {
        string[] errors = files
            .Where(x => !x.Value.Exists)
            .Select(x => $"The {x.Key} file does not exist.").ToArray();
        if(errors.Any())
            return new Failure(errors);

        KeyValuePair<string, StreamReader>[] readers = files
            .Select(x => new KeyValuePair<string, StreamReader>(x.Key, new(x.Value.FullName, Verify.Encoding)))
            .ToArray();
        int lineNumber = 1;
        while (!errors.Any() && readers.Any(x => x.Value.Peek() != -1))
        {
            string[] lines = await Task.WhenAll(readers.Select(x => x.Value.ReadLineAsync()));
            if (lines.Distinct().Skip(1).Any())
                errors = Array.Empty<string>()
                    .Append($"Difference found on line {lineNumber}.")
                    .Concat(readers.Select((_, i) => $"{readers[i].Key}: {lines[i]}"))
                    .ToArray();
            lineNumber++;
        }

        if (errors.Any())
        {
            foreach (KeyValuePair<string, StreamReader> pair in readers)
                pair.Value.Dispose();
            return new Failure(errors);
        }

        errors = readers
            .Where(x => x.Value.Peek() != -1)
            .Select(x => $"Line counts don't match. {x.Key} still has lines.")
            .ToArray();

        foreach (KeyValuePair<string, StreamReader> pair in readers)
            pair.Value.Dispose();
        return errors.Any()
            ? new Failure(errors)
            : new Success();
    }
}
