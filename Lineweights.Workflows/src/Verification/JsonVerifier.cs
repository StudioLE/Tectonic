using System.IO;
using Ardalis.Result;
using JsonDiffPatchDotNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using StudioLE.Core.System;

namespace Lineweights.Workflows.Verification;

/// <summary>
/// A lightweight alternative to <see href="https://github.com/VerifyTests/Verify"/> developed specifically with Elements in mind.
/// </summary>
/// <remarks>
/// Verifier is completely engine agnostic. It has no dependency on NUnit.
/// </remarks>
public sealed class JsonVerifier : VerifierBase<object>
{
    private const int DecimalPlaces = 5;

    /// <inheritdoc />
    public JsonVerifier(IVerifyContext context) : base(context, ".json")
    {
    }

    /// <summary>
    /// Verify <paramref name="actual"/> as JSON.
    /// </summary>
    protected override Result<bool> Verify(object actual)
    {
        FileInfo receivedFile = ReceivedFile();
        FileInfo verifiedFile = VerifiedFile();

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
}
