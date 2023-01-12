using CsvHelper.Configuration.Attributes;
using Geometrician.Core.Converters;
using Lineweights.Core.Assets;
using Lineweights.Core.Storage;
using StudioLE.Core.Conversion;
using StudioLE.Core.Results;
using StudioLE.Core.System;

namespace Geometrician.Core.Assets;

public class CsvElementTypesAssetFactory : ExternalAssetFactoryBase<Model>
{
    /// <inheritdoc />
    protected override IConverter<Model, Task<IResult<Uri>>> Converter { get; }

    /// <inheritdoc cref="CsvElementTypesAssetFactory"/>
    public CsvElementTypesAssetFactory(IStorageStrategy storageStrategy)
    {
        Converter = new ModelToCsvFile<TableRow>(storageStrategy, Asset.Id + ".csv", CreateTableOfElements);
        Asset.Name = "Element Types in Model";
        Asset.ContentType = "text/csv";
    }

    private static IReadOnlyCollection<TableRow> CreateTableOfElements(Model model)
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
                IEnumerable<string> representationNames = geometric.Representation is null
                    ? Enumerable.Empty<string>()
                    : geometric
                        .Representation
                        .SolidOperations
                        .Select(x => x.GetType().Name);
                row.IsGeometricElement = true;
                row.Representations = representationNames.Join(", ");
                return row;
            })
            .ToArray();
    }

    private class TableRow
    {
        [Name("Type")]
        public string Type { get; set; } = string.Empty;

        [Name("Count")]
        public int Count { get; set; }

        [Name("Geometric?")]
        public bool IsGeometricElement { get; set; } = false;

        [Name("Representation")]
        public string Representations { get; set; } = string.Empty;
    }
}
