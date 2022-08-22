using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Lineweights.Drawings;
using StudioLE.Core.System.IO;

namespace Lineweights.Workflows.Samples;

internal static class ResultSamples
{
    internal static IReadOnlyCollection<View> Views(IReadOnlyCollection<GeometricElement> geometry)
    {
        ViewDirection[] viewDirections = {
            ViewDirection.Top,
            ViewDirection.Left,
            ViewDirection.Front
        };

        return viewDirections.Select(viewDirection =>
            {
                ViewBuilder builder = new ViewBuilder()
                    .ScopePadding(.25, .25, .25)
                    .ViewDirection(viewDirection)
                    .ElementsInView(geometry);
                return builder.Build();
            })
            .ToArray();
    }

    internal static DocumentInformation CsvDocumentInformation(IEnumerable<Element> elements)
    {
        TableRow[] table = elements
            .GroupBy(x => x.GetType())
            .Select(grouping =>
            {
                TableRow row = new()
                {
                    Type = grouping.Key.Name,
                    Count = grouping.Count()
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
        DocumentInformation doc = new()
        {
            Name = "Elements",
            Location = new(file.FullName)
        };
        using StreamWriter writer = new(file.FullName);
        using CsvWriter csv = new(writer, CultureInfo.InvariantCulture);
        csv.WriteRecords(table);

        return doc;
    }

    internal static IReadOnlyCollection<Element> All()
    {
        IReadOnlyCollection<GeometricElement> geometry = Scenes.GeometricElements();
        Model model = new();

        model.AddElements(geometry);

        model.AddElements(Views(geometry));
        model.AddElements(CsvDocumentInformation(model.Elements.Values));
        return model.Elements.Values.ToArray();
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
