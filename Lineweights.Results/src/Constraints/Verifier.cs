using System.IO;
using Ardalis.Result;
using DiffEngine;
using DiffMatchPatch;
using JsonDiffPatchDotNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using StudioLE.Core.System;

namespace Lineweights.Results.Constraints;

/// <summary>
/// A lightweight alternative to <see href="https://github.com/VerifyTests/Verify"/> developed specifically with Elements in mind.
/// </summary>
/// <remarks>
/// Verifier is completely engine agnostic. It has no dependency on NUnit.
/// </remarks>
public sealed class Verifier
{
    private readonly IVerifyContext _context;
    private const int DecimalPlaces = 5;
    private const int DiffCharacterThreshold = 2000;
    private const int DiffItemsThreshold = 100;

    /// <inheritdoc cref="Verifier"/>
    public Verifier(IVerifyContext context)
    {
        _context = context;
        if (!context.Directory.Exists)
            throw new DirectoryNotFoundException($"Failed to Verify. The verify directory does not exist: {context.Directory.FullName}");
    }

    /// <summary>
    /// Verify <paramref name="actual"/>.
    /// </summary>
    public Result<bool> Execute(object actual)
    {
        Result<bool> result = actual switch
        {
            string str => VerifyAsString(str),
            _ => VerifyAsJson(actual)
        };
        _context.OnResult(result);
        return result;
    }

    private FileInfo ReceivedFile(string fileExtension)
    {
        return new(Path.Combine(_context.Directory.FullName, $"{_context.FileNamePrefix}.received{fileExtension}"));
    }

    private FileInfo VerifiedFile(string fileExtension)
    {
        return new(Path.Combine(_context.Directory.FullName, $"{_context.FileNamePrefix}.verified{fileExtension}"));
    }

    /// <summary>
    /// Verify <paramref name="actual"/> as a string.
    /// </summary>
    private Result<bool> VerifyAsString(string actual)
    {
        FileInfo receivedFile = ReceivedFile(".txt");
        FileInfo verifiedFile = VerifiedFile(".txt");

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

    /// <summary>
    /// Verify <paramref name="actual"/> as JSON.
    /// </summary>
    private Result<bool> VerifyAsJson(object actual)
    {
        FileInfo receivedFile = ReceivedFile(".json");
        FileInfo verifiedFile = VerifiedFile(".json");

        JsonConverter[] converters =
        {
            new StringEnumConverter(),
            new DoubleConverter(DecimalPlaces)
        };
        string receivedJson = JsonConvert.SerializeObject(actual, Formatting.Indented, converters);
        File.WriteAllText(receivedFile.FullName, receivedJson);
        if (!verifiedFile.Exists)
            File.WriteAllText(verifiedFile.FullName, "{}");
        string verifiedJson = File.ReadAllText(verifiedFile.FullName);

        if (string.IsNullOrWhiteSpace(receivedJson))
            return Result<bool>.Error("Received JSON is empty.");
        if (string.IsNullOrWhiteSpace(verifiedJson))
            return Result<bool>.Error("Verified JSON is empty.");

        if (CompareStringsWithNormalizedLineEndings(receivedJson, verifiedJson))
            return true;

        JToken? diff = Diff(verifiedJson, receivedJson);

        if (diff is null)
            return true;

        if (AssemblyHelpers.IsDebugBuild())
            LaunchDiffEngine(receivedFile, verifiedFile);

        string diffString = diff.ToString();
        return Result<bool>.Error(diffString);
    }

    private static JToken? Diff(string verifiedJson, string receivedJson)
    {
        JsonDiffPatch jdp = new();
        JToken receivedJToken;
        JToken verifiedJToken;

        try
        {
            receivedJToken = JToken.Parse(receivedJson);
        }
        catch (Exception e)
        {
            throw new("Failed to parse received JSON with JToken.", e);
        }

        try
        {
            verifiedJToken = JToken.Parse(verifiedJson);
        }
        catch (Exception e)
        {
            throw new("Failed to parse verified JSON with JToken.", e);
        }
        return jdp.Diff(verifiedJToken, receivedJToken);

    }


    // TODO: DiffEngineTray SendMove
    // https://github.com/VerifyTests/DiffEngine/blob/3f2e942e73369a1d6c40d96b931d19651944b35f/src/DiffEngine/Tray/PiperClient.cs#L26-L33
    private static async void LaunchDiffEngine(FileInfo receivedFile, FileInfo verifiedFile)
    {
        await DiffRunner.LaunchAsync(receivedFile.FullName, verifiedFile.FullName);
    }

    private static bool CompareStringsWithNormalizedLineEndings(string first, string second)
    {
        return first.ReplaceWindowsLineEndings().Equals(second.ReplaceWindowsLineEndings());
    }
}
