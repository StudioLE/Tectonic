using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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
    public static readonly JsonConverter[] Converters =
    {
        new StringEnumConverter(),
        new DoubleConverter(DecimalPlaces)
    };

    /// <inheritdoc />
    public JsonVerifier(IVerifyContext context) : base(context, ".json")
    {
    }

    /// <inheritdoc />
    protected override void WriteActual(object actual)
    {
        // TODO: directly write the JSON Stream
        string receivedJson = JsonConvert.SerializeObject(actual, Formatting.Indented, Converters);
        File.WriteAllText(_receivedFile.FullName, receivedJson, Verify.Encoding);
    }
}
