using System.IO;
using Ardalis.Result;
using DiffMatchPatch;

namespace Lineweights.Workflows.Verification;


/// <inheritdoc cref="VerifierBase{T}"/>
public sealed class StringVerifier : VerifierBase<string>
{
    private const int DiffCharacterThreshold = 2000;
    private const int DiffItemsThreshold = 100;

    /// <inheritdoc />
    public StringVerifier(IVerifyContext context, string fileExtension) : base(context, fileExtension)
    {

    }
    /// <inheritdoc />
    protected override void WriteActual(string actual)
    {
        File.WriteAllText(_receivedFile.FullName, actual);
    }

    /// <inheritdoc />
    protected override Result<bool> Diff()
    {
        // TODO: Move this to IDiffer
        string actual = File.ReadAllText(_receivedFile.FullName);
        string verified = File.ReadAllText(_verifiedFile.FullName);

        if (actual.Length > DiffCharacterThreshold || verified.Length > DiffCharacterThreshold)
            return Result<bool>.Error("[String is too large to diff]", $"Actual: {actual.Length}", $"Verified: {verified.Length}");

        diff_match_patch dfm = new();
        List<Diff> diffs = dfm.diff_main(verified, actual);

        return diffs.Count > DiffItemsThreshold
            ? Result<bool>.Error("[Diff of string had too many parts]", $"Actual: {actual.Length}", $"Verified: {verified.Length}")
            : Result<bool>.Error(diffs.Select(x => $"{x.operation}: {x.text}").ToArray());
    }
}
