using System.Globalization;
using System.IO;
using CsvHelper;
using Lineweights.Results;
using Newtonsoft.Json;

namespace Lineweights.Dashboard.Shared;

/// <summary>
/// Methods to Help with <see cref="Result"/>
/// </summary>
public static class ResultHelper
{
    public static bool IsFileType(this Result @this, params string[] extensions)
    {
        return @this.Metadata.Location is not null
               && extensions.Any(@this.Metadata.Location.AbsoluteUri.EndsWith);
    }

    public static string GetTitle(this Result @this)
    {
        return string.IsNullOrEmpty(@this.Metadata.Name)
            ? @this.Metadata.Id.ToString()
            : @this.Metadata.Name;
    }

    public static string GetMimeType(this Result @this)
    {
        return "application/pdf";
    }

    public static string GetFileAsString(this Result @this)
    {
        if (@this.Metadata.Location is null)
            return string.Empty;
        return new HttpClient()
            .GetStringAsync(@this.Metadata.Location.AbsoluteUri)
            .Result;
    }

    public static string FormatJson(this string json)
    {
        object? parsed = JsonConvert.DeserializeObject(json);
        return JsonConvert.SerializeObject(parsed, Formatting.Indented);
    }

    public static IReadOnlyCollection<IReadOnlyCollection<string>> ToTable(this string @this)
    {
        using StringReader reader = new(@this);
        using CsvReader csv = new(reader, CultureInfo.InvariantCulture);
        List<string[]> table = new();
        while (csv.Read())
            table.Add(csv.Parser.Record ?? Array.Empty<string>());
        return table;
    }
}
