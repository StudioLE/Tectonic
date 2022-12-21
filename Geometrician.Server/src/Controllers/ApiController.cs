using System.Text.Json;
using Geometrician.Core.Shared;
using Lineweights.Core.Documents;
using Lineweights.Workflows.Visualization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Geometrician.Server.Controllers;

public class ApiController : Controller
{
    private readonly ILogger<ApiController> _logger;
    private readonly AssetState _state;

    public ApiController(ILogger<ApiController> logger, AssetState state)
    {
        _logger = logger;
        _state = state;
    }

    [HttpPost(GeometricianService.AssetRoute)]
    public ActionResult PostAsset([FromBody] JsonElement jsonElement)
    {
        _logger.LogDebug($"{nameof(PostAsset)} called.");
        string json = jsonElement.GetRawText();
        Asset? asset = JsonConvert.DeserializeObject<Asset>(json);
        if (asset is null)
            return BadRequest("Failed to convert JSON.");
        _state.Assets.Add(asset);
        return CreatedAtAction(nameof(PostAsset), asset);
    }
}
