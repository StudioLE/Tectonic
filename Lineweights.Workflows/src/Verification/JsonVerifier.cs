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
    protected override Task WriteActual(object actual)
    {
        JsonSerializerSettings settings = new()
        {
            Formatting = Formatting.Indented,
            Converters = Converters
        };
        using StreamWriter writer = new(_receivedFile.FullName, false, Verify.Encoding);
        JsonSerializer serializer = JsonSerializer.Create(settings);
        serializer.Serialize(writer, actual);
        return Task.CompletedTask;
    }
}
