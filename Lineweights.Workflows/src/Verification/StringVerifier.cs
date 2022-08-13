using System.IO;
using Ardalis.Result;
using DiffMatchPatch;
using StudioLE.Core.System;

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

    /// <summary>
    /// Verify <paramref name="actual"/> as a string.
    /// </summary>
    protected override Result<bool> Verify(string actual)
    {
        FileInfo receivedFile = ReceivedFile();
        FileInfo verifiedFile = VerifiedFile();

        File.WriteAllText(receivedFile.FullName, actual);
        if (!verifiedFile.Exists)
            File.WriteAllText(verifiedFile.FullName, "");
        string verified = File.ReadAllText(verifiedFile.FullName);

        if (CompareStringsWithNormalizedLineEndings(actual, verified))
            return true;

        if (AssemblyHelpers.IsDebugBuild())
            LaunchDiffEngine(receivedFile, verifiedFile);

        if (actual.Length > DiffCharacterThreshold || verified.Length > DiffCharacterThreshold)
            return Result<bool>.Error("[String is too large to diff]", $"Actual: {actual.Length}", $"Verified: {verified.Length}");

        diff_match_patch dfm = new();
        List<Diff> diffs = dfm.diff_main(verified, actual);

        return diffs.Count > DiffItemsThreshold
            ? Result<bool>.Error("[Diff of string had too many parts]", $"Actual: {actual.Length}", $"Verified: {verified.Length}")
            : Result<bool>.Error(diffs.Select(x => $"{x.operation}: {x.text}").ToArray());
    }
}
