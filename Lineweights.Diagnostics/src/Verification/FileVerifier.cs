using System.IO;
using System.Security.Cryptography;
using Lineweights.Core.Elements.Comparers;
using StudioLE.Core.Results;

namespace Lineweights.Diagnostics.Verification;


/// <inheritdoc cref="VerifierBase{T}"/>
public sealed class FileVerifier : VerifierBase<FileInfo>
{
    /// <inheritdoc />
    public FileVerifier(IVerifyContext context, string fileExtension) : base(context, fileExtension)
    {
    }

    /// <summary>
    /// Verify <paramref name="actual"/>.
    /// </summary>
    public override async Task<IResult> Execute(FileInfo actual)
    {
        FileInfo receivedFile = actual;
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

    /// <inheritdoc />
    protected override Task Write(FileInfo file, FileInfo value)
    {
        file = value;
        return Task.CompletedTask;
    }

    protected override async Task<IResult> Compare(params KeyValuePair<string, FileInfo>[] files)
    {
        bool areTextFiles = files.All(x => IsTextFile(x.Value));
        if (areTextFiles)
            return await base.Compare(files);

        string[] errors = files
            .Where(x => !x.Value.Exists)
            .Select(x => $"The {x.Key} file does not exist.")
            .ToArray();
        if (errors.Any())
            return new Failure(errors);

        errors = CheckValuesMatch(files, x => x.Length);
        if (errors.Any())
            return new Failure("File sizes are different", errors);

        errors = CheckValuesMatch(files, GetFileHash);
        if (errors.Any())
            return new Failure("File hashes are different", errors);

        return new Success();
    }

    private static bool IsTextFile(FileInfo file)
    {
        string[] textFileExtensions =
        {
            ".txt",
            ".json",
            ".svg"
        };
        return textFileExtensions.Contains(file.Extension);
    }

    private static string GetFileHash(FileInfo file)
    {
        using MD5 md5 = MD5.Create();
        using FileStream stream = File.OpenRead(file.FullName);
        byte[] bytes = md5.ComputeHash(stream);
        return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
    }

    private static string[] CheckValuesMatch<TKey, TValue, TCompare>(IEnumerable<KeyValuePair<TKey, TValue>> @this, Func<TValue, TCompare> func)
    {
        KeyValuePair<TKey, TCompare>[] hashes = @this
            .Select(x =>
            {
                TCompare result = func.Invoke(x.Value);
                return new KeyValuePair<TKey, TCompare>(x.Key, result);
            })
            .ToArray();
        var comparer = new KeyValuePairValueComparer<TKey, TCompare>();
        bool areDifferent = hashes.Distinct(comparer).Skip(1).Any();
        return areDifferent
            ? hashes.Select(x => $"{x.Key}: {x.Value}").ToArray()
            : Array.Empty<string>();
    }
}
