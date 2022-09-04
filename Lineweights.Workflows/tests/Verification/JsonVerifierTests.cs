using Ardalis.Result;
using Lineweights.Workflows.Verification;
using Newtonsoft.Json;

namespace Lineweights.Workflows.Tests.Verification;

internal sealed class JsonVerifierTests
{
    [Test]
    public void JsonVerifier_IsValid()
    {
        // Arrange
        MockVerifyContext context = new("JsonVerifier_Pass");
        JsonVerifier verifier = new(context);
        string actualJson = context.ReadSourceFile(".json");

        // Act
        BBox3[]? actual = JsonConvert.DeserializeObject<BBox3[]>(actualJson, JsonVerifier.Converters);
        if (actual is null)
            throw new("Failed to de-serialize.");
        Result<bool> result = verifier.Execute(actual);
        foreach (string error in result.Errors)
            Console.WriteLine(error);

        // Assert
        Assert.That(result.IsSuccess, Is.True, "IsSuccess");
        Assert.That(result.Errors, Is.Empty, "Errors");
    }

    [Test]
    public void JsonVerifier_IsInvalid()
    {
        // Arrange
        MockVerifyContext context = new("JsonVerifier_Fail");
        JsonVerifier verifier = new(context);
        string actualJson = context.ReadSourceFile(".json");

        // Act
        BBox3[]? actual = JsonConvert.DeserializeObject<BBox3[]>(actualJson, JsonVerifier.Converters);
        if (actual is null)
            throw new("Failed to de-serialize.");
        Result<bool> result = verifier.Execute(actual);
        foreach (string error in result.Errors)
            Console.WriteLine(error);

        // Assert
        Assert.That(result.IsSuccess, Is.False, "IsSuccess");
        Assert.That(result.Errors, Is.Not.Empty, "Errors");
    }
}
