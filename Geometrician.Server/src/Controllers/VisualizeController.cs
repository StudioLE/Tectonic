using System.Text.Json;
using Geometrician.Core.Shared;
using Lineweights.Core.Documents;
using Lineweights.PDF;
using Lineweights.SVG;
using Lineweights.Workflows.Documents;
using Lineweights.Workflows.Visualization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Geometrician.Server.Controllers;

public class VisualizeController : Controller
{
    private readonly ILogger<VisualizeController> _logger;
    private readonly AssetState _state;
    private readonly IStorageStrategy _storageStrategy;

    public VisualizeController(ILogger<VisualizeController> logger, AssetState state, IStorageStrategy storageStrategy)
    {
        _logger = logger;
        _state = state;
        _storageStrategy = storageStrategy;
    }

    [HttpHead(GeometricianService.VisualizeRoute)]
    [HttpGet(GeometricianService.VisualizeRoute)]
    public ActionResult Get()
    {
        _logger.LogDebug($"{nameof(Get)} called.");
        return Ok();
    }

    [HttpPost(GeometricianService.VisualizeRoute)]
    public ActionResult Post([FromBody] JsonElement jsonElement)
    {
        _logger.LogDebug($"{nameof(Post)} called.");
        // Run on a separate thread so we can return an immediate response..
        Task.Run(() => ProcessReceived(jsonElement));
        return StatusCode(StatusCodes.Status202Accepted);
    }

    private async Task ProcessReceived(JsonElement jsonElement)
    {
        string json = jsonElement.GetRawText();
        VisualizeRequest? receivedAsset = JsonConvert.DeserializeObject<VisualizeRequest>(json);
        if (receivedAsset is null)
        {
            _logger.LogWarning("Failed to deserialize asset from JSON.");
            return;
        }
        Asset asset = await BuildAsset(receivedAsset);
        _state.Assets.Add(asset);
    }

    private async Task<Asset> BuildAsset(VisualizeRequest request)
    {
        // TODO: If this is created via DI then the Storage Strategy will be injected automatically.
        AssetBuilder builder = new AssetBuilder()
            .SetStorageStrategy(_storageStrategy)
            .SetDocumentInformation(request.Asset.Info)
            .ConvertModelToGlb()
            .ExtractViewsAndConvertToSvg()
            .ExtractSheetsAndConvertToPdf()
            .AddAssets(request.Asset.Children.ToArray());
        // TODO: We may need to ensure the Assets are stored using the correct StorageStrategy?
        return await builder.Build(request.Model);
    }
}
