using System.IO;
using DiffEngine;
using Lineweights.Workflows.Verification;
using Newtonsoft.Json.Linq;
using StudioLE.Core.System;

namespace Lineweights.Workflows.NUnit.Verification;

public static class VerifyHelpers
{
    /// <summary>
    /// Verify two strings match.
    /// </summary>
    public static async Task Diff(string expected, string actual)
    {
        string tempFile = Path.GetTempFileName();

        FileInfo expectedFile = new($"{tempFile}.expected.json");
        FileInfo actualFile = new($"{tempFile}.actual.json");

        using StreamWriter expectedWriter = new(expectedFile.FullName, false, Verify.Encoding);
        await expectedWriter.WriteAsync(expected);

        using StreamWriter actualWriter = new(actualFile.FullName, false, Verify.Encoding);
        await actualWriter.WriteAsync(actual);

        if (AssemblyHelpers.IsDebugBuild())
            await DiffRunner.LaunchAsync(actualFile.FullName, expectedFile.FullName);
    }

    public static void SerialisationAsModel<TElement>(TElement expectedElement) where TElement : Element
    {
        Model expectedModel = new();
        // TODO: Sub elements MUST be added before the element.
        expectedModel.AddSubElements(expectedElement);
        expectedModel.AddElements(expectedElement);

        // Act
        string json = expectedModel.ToJson(true);
        Model actualModel = Model.FromJson(json, out List<string> errors, true);
        string json2 = actualModel.ToJson(true);

        json = CleanJson(json);
        json2 = CleanJson(json2);

        // Preview
        // await new Visualize().Execute(model);
        // await new Visualize().Execute(deserializedModel);

        // Assert
        Assert.Multiple(async () =>
        {
            DiffRunner.MaxInstancesToLaunch(2);

            // Verify element names
            IReadOnlyCollection<string> expectedElements = GetElementNames(expectedModel);
            IReadOnlyCollection<string> actualElements = GetElementNames(actualModel);
            await Verify.String(expectedElements.Join(), actualElements.Join());

            // Verify json
            await Verify.String(json, json2);
            Assert.That(actualModel.Elements.Count, Is.EqualTo(expectedModel.Elements.Count), "Element count.");
            Assert.That(errors, Is.Empty, "Serialisation errors.");
            CollectionAssert.AreEqual(actualElements, expectedElements);
            TElement deserializedElement = expectedModel
                .AllElementsOfType<TElement>()
                .First();
            Assert.That(deserializedElement, Is.EqualTo(expectedElement), "Matches.");
        });
    }

    private static IReadOnlyCollection<string> GetElementNames(Model model)
    {
        return model
            .Elements
            .Values
            .Select(element => $"{element.Id} {element.GetType()}")
            .OrderBy(x => x)
            .ToArray();
    }

    private static string CleanJson(string json)
    {
        JObject unOrderedModel = JObject.Parse(json);

        IEnumerable<JObject> orderedElements = unOrderedModel["Elements"].Select(CleanProperties);

        JProperty transform = new("Transform", unOrderedModel["Transform"]);
        JProperty elements = new("Elements", orderedElements);
        JObject orderedModel = new(transform, elements);
        return orderedModel.ToString();
    }

    private static JObject CleanProperties(JToken jToken)
    {
        if (jToken is not JProperty jProperty)
            throw new("Expected a JProperty");
        if (jProperty.Value is not JObject jObject)
            throw new("Expected a JObject");
        IOrderedEnumerable<JProperty> properties = jObject
            .Properties()
            // Remove null values
            .Where(x => x.Name.ToString() == "Name" || x.Value.Type != JTokenType.Null)
            // Order alphabetically
            .OrderBy(x => x.Name.ToString());
        return new(properties);
    }
}
