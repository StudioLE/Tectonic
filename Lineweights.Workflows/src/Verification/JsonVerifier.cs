using System.IO;
using Ardalis.Result;
using JsonDiffPatchDotNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

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

    /// <inheritdoc />
    protected override void WriteActual(object actual)
    {
        JsonConverter[] converters =
        {
            new StringEnumConverter(),
            new DoubleConverter(DecimalPlaces)
        };
        // TODO: directly write the JSON Stream
        string receivedJson = JsonConvert.SerializeObject(actual, Formatting.Indented, converters);
        File.WriteAllText(_receivedFile.FullName, receivedJson);
    }

    /// <inheritdoc />
    protected override Result<bool> Diff()
    {
        // TODO: Move this to IDiffer
        string actual = File.ReadAllText(_receivedFile.FullName);
        string verified = File.ReadAllText(_verifiedFile.FullName);

        JsonDiffPatch jdp = new();
        JToken actualJToken;
        JToken verifiedJToken;

        try
        {
            actualJToken = JToken.Parse(actual);
        }
        catch (Exception e)
        {
            throw new("Failed to parse received JSON with JToken.", e);
        }

        try
        {
            verifiedJToken = JToken.Parse(verified);
        }
        catch (Exception e)
        {
            throw new("Failed to parse verified JSON with JToken.", e);
        }

        JToken? diffJToken = jdp.Diff(verifiedJToken, actualJToken);
        if (diffJToken is null)
            return true;
        string diff = diffJToken.ToString();
        return Result<bool>.Error(diff);
    }
}
