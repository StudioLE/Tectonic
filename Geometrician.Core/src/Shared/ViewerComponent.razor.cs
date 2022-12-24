using System.Globalization;
using System.IO;
using CsvHelper;
using Geometrician.Core.Scripts;
using Lineweights.Core.Documents;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StudioLE.Core.System;
using StudioLE.Core.System.IO;


namespace Geometrician.Core.Shared;

public class ViewerComponentBase : ComponentBase
{
    /// <inheritdoc cref="ILogger"/>
    [Inject]
    protected ILogger<ViewerComponent> Logger { get; set; } = default!;

    /// <inheritdoc cref="ModelViewer"/>
    [Inject]
    protected ModelViewer Three { get; set; } = default!;

    /// <inheritdoc cref="IStorageStrategy"/>
    [Inject]
    protected IStorageStrategy StorageStrategy { get; set; } = default!;

    /// <inheritdoc cref="Geometrician.Core.Scripts.ObjectUrlStorage"/>
    [Inject]
    protected ObjectUrlStorage ObjectUrlStorage { get; set; } = default!;

    /// <inheritdoc cref="Display"/>
    [Inject]
    protected DisplayState Display { get; set; } = default!;

    /// <summary>
    /// The id of the card.
    /// </summary>
    [Parameter]
    public Asset Asset { get; set; } = default!;

    protected Guid ComponentId { get; set; } = Guid.NewGuid();

    protected ViewerType Type { get; set; } = ViewerType.Unknown;

    public string Title { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;

    public IReadOnlyCollection<IReadOnlyCollection<string>> Table { get; set; } = Array.Empty<IReadOnlyCollection<string>>();

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        Asset.ContentType ??= Asset.Info.Location?.GetFileName().GetContentTypeByExtension();
        Type = Asset.ContentType switch
        {
            "text/plain" => ViewerType.Text,
            "text/csv" => ViewerType.Table,
            "model/gltf-binary" => ViewerType.Three,
            "application/json" => ViewerType.Json,
            "image/svg+xml" => ViewerType.Image,
            "application/pdf" => ViewerType.Object,
            "txt/pdf" => ViewerType.Object,
            _ => ViewerType.Unknown
        };

        Title = string.IsNullOrEmpty(Asset.Info.Name)
            ? Asset.Info.Id.ToString()
            : Asset.Info.Name;
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool isFirstRender)
    {
        if (!isFirstRender)
            return;

        bool stateHasChanged = false;

        switch (Type)
        {
            case ViewerType.Json:
                Text = await GetAsJson();
                stateHasChanged = true;
                break;
            case ViewerType.Text:
                Text = await GetAsString();
                stateHasChanged = true;
                break;
            case ViewerType.Table:
                Table = await GetAsTable();
                stateHasChanged = true;
                break;
            case ViewerType.Three:
                await LoadGlb();
                break;
        }

        if (stateHasChanged)
            StateHasChanged();
    }

    /// <summary>
    /// Load the GLB with Three.js
    /// </summary>
    private async Task LoadGlb()
    {
        Logger.LogDebug($"{nameof(LoadGlb)}() called on result {Asset.Info.Id}.");
        if (Asset.Info.Location is null)
        {
            Logger.LogWarning("Failed to load GLB. Location was null.");
            return;
        }
        await Three.Create(ComponentId.ToString(), Asset.Info.Location.AbsoluteUri);
    }

    protected enum ViewerType
    {
        Unknown,
        Three,
        Image,
        Object,
        Table,
        Text,
        Json
    }

    private async Task<string> GetAsString()
    {
        if (Asset.Content is not null)
            return Asset.Content;
        if (Asset.Info.Location is null)
        {
            Logger.LogWarning("Failed to get asset as string. Neither Content or Info.Location are set.");
            return string.Empty;
        }
        if (Asset.Info.Location.IsFile)
        {
            Logger.LogWarning("Failed to get asset as string. The asset location is a local file.");
            return string.Empty;
        }

        try
        {
            if (StorageStrategy is ObjectUrlStorageStrategy)
                return await ObjectUrlStorage.GetAsString(Asset.Info.Location.AbsoluteUri);
            return await new HttpClient().GetStringAsync(Asset.Info.Location.AbsoluteUri);
        }
        catch (Exception e)
        {
            Logger.LogWarning("Failed to get asset as string. " + e.Message);
            return string.Empty;
        }
    }

    private async Task<string> GetAsJson()
    {
        string json = await GetAsString();
        object? parsed = JsonConvert.DeserializeObject(json);
        return JsonConvert.SerializeObject(parsed, Formatting.Indented);
    }

    private async Task<IReadOnlyCollection<IReadOnlyCollection<string>>> GetAsTable()
    {
        string data = await GetAsString();
        using StringReader reader = new(data);
        using CsvReader csv = new(reader, CultureInfo.InvariantCulture);
        List<string[]> table = new();
        while (csv.Read())
            table.Add(csv.Parser.Record ?? Array.Empty<string>());
        return table;
    }
}
