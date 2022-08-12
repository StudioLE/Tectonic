using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Lineweights.Drawings;
using Lineweights.Workflows.Results;
using StudioLE.Core.System.IO;

namespace Lineweights.Workflows.Tests;

[SendToDashboardAfterTest]
internal sealed class DashboardTests : ResultModel
{
    private readonly IReadOnlyCollection<GeometricElement> _geometry = Scenes.GeometricElements();

    [SetUp]
    public void Setup()
    {
        Model.AddElements(_geometry);
    }

    [Test]
    public void Dashboard_Sample()
    {
        // Arrange
        Model.AddElements(_geometry);
        ViewDirection[] viewDirections = {
            ViewDirection.Top,
            ViewDirection.Left,
            ViewDirection.Front
        };
        foreach (ViewDirection viewDirection in viewDirections)
        {
            ViewBuilder builder = new ViewBuilder()
                .ScopePadding(.25, .25, .25)
                .ViewDirection(viewDirection)
                .ElementsInView(_geometry);
            View view = builder.Build();
            Model.AddElements(view);
        }

        TableRow[] table = Model.AllElements()
            .GroupBy(x => x.GetType())
            .Select(grouping =>
            {
                TableRow row = new()
                {
                    Type = grouping.Key.Name, Count = grouping.Count()
                };
                Element prototype = grouping.First();
                if (prototype is not GeometricElement geometric)
                    return row;
                IEnumerable<string> representationNames = geometric
                                                              .Representation
                                                              ?.SolidOperations
                                                              .Select(x => x.GetType().Name)
                                                          ?? Enumerable.Empty<string>();
                row.IsGeometricElement = true;
                row.Representations = string.Join(", ", representationNames);
                return row;
            })
            .ToArray();

        FileInfo file = PathHelpers.GetTempFile(".csv");
        DocumentInformation metadata = new()
        {
            Name = "Elements",
            Location = new(file.FullName)
        };
        using StreamWriter writer = new(file.FullName);
        using CsvWriter csv = new (writer, CultureInfo.InvariantCulture);
        csv.WriteRecords(table);

        Model.AddElement(metadata);
    }

    private class TableRow
    {
        [Name("Type")]
        public string Type { get; set; } = string.Empty;

        [Name("Count")]
        public int Count { get; set; }

        [Name("Is Geometric?")]
        public bool IsGeometricElement { get; set; } = false;


        [Name("Representation Types")]
        public string Representations { get; set; } = string.Empty;
    }
}
