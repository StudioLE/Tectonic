using System.Globalization;
using System.IO;
using CsvHelper;
using Geometrician.Core.Scripts;
using Lineweights.Core.Documents;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StudioLE.Core.Exceptions;
using StudioLE.Core.Results;

namespace Geometrician.Core.Visualization;

public class StringAssetComponentBase : ViewerComponentBase<IAsset>
{
    /// <inheritdoc cref="ILogger"/>
    [Inject]
    private ILogger<StringAssetComponentBase> Logger { get; set; } = default!;

    /// <inheritdoc cref="IStorageStrategy"/>
    [Inject]
    protected IStorageStrategy StorageStrategy { get; set; } = default!;

    /// <inheritdoc cref="Geometrician.Core.Scripts.ObjectUrlStorage"/>
    [Inject]
    protected ObjectUrlStorage ObjectUrlStorage { get; set; } = default!;

    protected string Content { get; private set; } = string.Empty;

    protected string Json { get; private set; } = string.Empty;

    protected IReadOnlyCollection<IReadOnlyCollection<string>> Table { get; private set; } = Array.Empty<IReadOnlyCollection<string>>();

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender && Factory.Result is Success)
        {
            await LoadContent();
            await InvokeAsync(StateHasChanged);
        }
    }

    /// <inheritdoc />
    protected override async Task AfterExecution()
    {
        await LoadContent();
    }

    private async Task LoadContent()
    {
        Content = Factory.Asset switch
        {
            ExternalAsset externalAsset => await GetContent(externalAsset),
            InternalAsset internalAsset => await GetContent(internalAsset),
            _ => throw new TypeSwitchException<IAsset>(Factory.Asset)
        };
        switch (Factory.Asset.ContentType)
        {
            case "application/json":
                Json = FormatJson(Content);
                break;
            case "text/csv":
                Table = ToTable(Content);
                break;
        }
    }

    private Task<string> GetContent(InternalAsset asset)
    {
        return Task.FromResult(asset.Content);
    }

    private async Task<string> GetContent(ExternalAsset asset)
    {
        if (asset.Location is null)
        {
            Logger.LogWarning("Failed to get asset as string. The asset location was null.");
            return string.Empty;
        }
        if (asset.Location.IsFile)
        {
            Logger.LogWarning("Failed to get asset as string. The asset location is a local file.");
            return string.Empty;
        }
        return await GetContent(asset.Location);

    }

    private async Task<string> GetContent(Uri uri)
    {
        try
        {
            if (StorageStrategy is ObjectUrlStorageStrategy)
                return await ObjectUrlStorage.GetAsString(uri.AbsoluteUri);
            return await new HttpClient().GetStringAsync(uri.AbsoluteUri);
        }
        catch (Exception e)
        {
            Logger.LogWarning("Failed to get asset as string. " + e.Message);
            return string.Empty;
        }
    }

    private static string FormatJson(string json)
    {
        object? parsed = JsonConvert.DeserializeObject(json);
        return JsonConvert.SerializeObject(parsed, Formatting.Indented);
    }

    private static IReadOnlyCollection<IReadOnlyCollection<string>> ToTable(string content)
    {
        using StringReader reader = new(content);
        using CsvReader csv = new(reader, CultureInfo.InvariantCulture);
        List<string[]> table = new();
        while (csv.Read())
            table.Add(csv.Parser.Record ?? Array.Empty<string>());
        return table;
    }
}
