using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Elements.Serialization.IFC;
using Lineweights.Core.Documents;
using Lineweights.Drawings;
using StudioLE.Core.System;
using StudioLE.Core.System.IO;

namespace Lineweights.Workflows.Samples;

internal static class SampleHelpers
{

    internal static IReadOnlyCollection<View> CreateViews(IReadOnlyCollection<GeometricElement> geometry)
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

    internal static IReadOnlyCollection<TableRow> CreateTableOfElements(Model model)
    {
        return model.Elements.Values
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
                row.Representations = representationNames.Join(", ");
                return row;
            })
            .ToArray();
    }

    internal static Asset CreateCsvFileAsAsset(Model model)
    {
        IReadOnlyCollection<TableRow> table = CreateTableOfElements(model);

        FileInfo file = PathHelpers.GetTempFile(".csv");
        using StreamWriter writer = new(file.FullName);
        using CsvWriter csv = new(writer, CultureInfo.InvariantCulture);
        csv.WriteRecords(table);

        return new()
        {
            Info = new()
            {
                Name = "Table of Elements in Model",
                Location = new(file.FullName)
            },
            ContentType = "text/csv"
        };
    }

    internal static Asset CreateIfcFileAsAsset(Model model)
    {
        FileInfo file = PathHelpers.GetTempFile(".ifc");
        model.ToIFC(file.FullName);
        return new()
        {
            Info = new()
            {
                Name = "IFC of Model",
                Location = new(file.FullName)
            },
            ContentType = "application/x-step"
        };
    }

    internal static Asset CreateJsonAsContentAsset(Model model)
    {
        string json = model.ToJson();

        return new()
        {
            Info = new()
            {
                Name = "Json of Model"
            },
            ContentType = "application/json",
            Content = json
        };
    }

    internal class TableRow
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
