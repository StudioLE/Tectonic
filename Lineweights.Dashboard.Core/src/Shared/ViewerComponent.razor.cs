using System.Globalization;
using System.IO;
using CsvHelper;
using Lineweights.Dashboard.Core.Scripts;
using Lineweights.Workflows.Containers;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StudioLE.Core.System;
using StudioLE.Core.System.IO;


namespace Lineweights.Dashboard.Core.Shared;

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

    /// <inheritdoc cref="ObjectUrlStorage"/>
    [Inject]
    protected ObjectUrlStorage ObjectUrlStorage { get; set; } = default!;

    /// <summary>
    /// The id of the card.
    /// </summary>
    [Parameter]
    public Container Container { get; set; } = default!;

    protected Guid ComponentId { get; set; } = Guid.NewGuid();

    protected ViewerType Type { get; set; } = ViewerType.Unknown;

    public string Title { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;

    public IReadOnlyCollection<IReadOnlyCollection<string>> Table { get; set; } = Array.Empty<IReadOnlyCollection<string>>();

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        if (Container.Info.Location is null)
            return;
        Container.ContentType ??= Container.Info.Location!.GetFileName().GetContentTypeByExtension();
        Type = Container.ContentType switch
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


        Title = string.IsNullOrEmpty(Container.Info.Name)
            ? Container.Info.Id.ToString()
            : Container.Info.Name;

        switch (Type)
        {
            case ViewerType.Json:
                Text = await GetFileAsJson();
                break;
            case ViewerType.Text:
                Text = await GetFileAsString();
                break;
            case ViewerType.Table:
                Table = await GetFileAsTable();
                break;
        }

    }


    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && Type == ViewerType.Three)
            await LoadGlb();
    }

    /// <summary>
    /// Load the GLB with Three.js
    /// </summary>
    private async Task LoadGlb()
    {
        Logger.LogDebug($"{nameof(LoadGlb)}() called on result {Container.Info.Id}.");
        await Three.Create(ComponentId.ToString(), Container.Info.Location!.AbsoluteUri);
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

    private async Task<string> GetFileAsString()
    {
        if (Container.Info.Location is null)
            return string.Empty;

        if(StorageStrategy is ObjectUrlStorageStrategy)
            return await ObjectUrlStorage.GetAsString(Container.Info.Location.AbsoluteUri);

        return await new HttpClient().GetStringAsync(Container.Info.Location.AbsoluteUri);
    }

    private async Task<string> GetFileAsJson()
    {
        string json = await GetFileAsString();
        object? parsed = JsonConvert.DeserializeObject(json);
        return JsonConvert.SerializeObject(parsed, Formatting.Indented);
    }

    private async Task<IReadOnlyCollection<IReadOnlyCollection<string>>> GetFileAsTable()
    {
        string data = await GetFileAsString();
        using StringReader reader = new(data);
        using CsvReader csv = new(reader, CultureInfo.InvariantCulture);
        List<string[]> table = new();
        while (csv.Read())
            table.Add(csv.Parser.Record ?? Array.Empty<string>());
        return table;
    }
}
