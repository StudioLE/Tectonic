using Lineweights.Diagnostics.Verification;
using Newtonsoft.Json;
using StudioLE.Core.Results;
using StudioLE.Core.System;

namespace Lineweights.Diagnostics.Tests.Verification;

internal sealed class JsonVerifierTests
{
    [Test]
    public async Task JsonVerifier_IsValid()
    {
        // Arrange
        MockVerifyContext context = new("JsonVerifier_Pass");
        JsonVerifier verifier = new(context);
        string actualJson = context.ReadSourceFile(".json");

        // Act
        BBox3[]? actual = JsonConvert.DeserializeObject<BBox3[]>(actualJson, JsonVerifier.Converters);
        if (actual is null)
            throw new("Failed to de-serialize.");
        IResult result = await verifier.Execute(actual);
        if (result.Errors.Any())
            Console.WriteLine(result.Errors.Join());

        // Assert
        Assert.That(result is Success, "Is Success");
        Assert.That(result.Errors, Is.Empty, "Errors");
    }

    [Test]
    public async Task JsonVerifier_IsInvalid()
    {
        // Arrange
        MockVerifyContext context = new("JsonVerifier_Fail");
        JsonVerifier verifier = new(context);
        string actualJson = context.ReadSourceFile(".json");

        // Act
        BBox3[]? actual = JsonConvert.DeserializeObject<BBox3[]>(actualJson, JsonVerifier.Converters);
        if (actual is null)
            throw new("Failed to de-serialize.");
        IResult result = await verifier.Execute(actual);
        if (result.Errors.Any())
            Console.WriteLine(result.Errors.Join());

        // Assert
        Assert.That(result is Success, Is.False, "Is Success");
        Assert.That(result.Errors, Is.Not.Empty, "Errors");
    }
}
