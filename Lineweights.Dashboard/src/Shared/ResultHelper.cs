using System.Globalization;
using System.IO;
using CsvHelper;
using Lineweights.Workflows.Results;
using Newtonsoft.Json;

namespace Lineweights.Dashboard.Shared;

/// <summary>
/// Methods to Help with <see cref="Result"/>
/// </summary>
public static class ResultHelper
{
    public static bool IsFileType(this Result @this, params string[] extensions)
    {
        return @this.Info.Location is not null
               && extensions.Any(@this.Info.Location.AbsoluteUri.EndsWith);
    }

    public static string GetTitle(this Result @this)
    {
        return string.IsNullOrEmpty(@this.Info.Name)
            ? @this.Info.Id.ToString()
            : @this.Info.Name;
    }

    public static string GetMimeType(this Result @this)
    {
        return "application/pdf";
    }

    public static string GetFileAsString(this Result @this)
    {
        if (@this.Info.Location is null)
            return string.Empty;
        return new HttpClient()
            .GetStringAsync(@this.Info.Location.AbsoluteUri)
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
