using System.Text.Json;
using Geometrician.Core.Visualization;
using Lineweights.Workflows.Visualization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Geometrician.Server.Controllers;

public class VisualizeController : Controller
{
    private readonly ILogger<VisualizeController> _logger;
    private readonly VisualizationState _state;

    public VisualizeController(ILogger<VisualizeController> logger, VisualizationState state)
    {
        _logger = logger;
        _state = state;
    }

    [HttpHead(VisualizationConfiguration.VisualizeRoute)]
    [HttpGet(VisualizationConfiguration.VisualizeRoute)]
    public ActionResult Get()
    {
        _logger.LogDebug($"{nameof(Get)} called.");
        return Ok();
    }

    [HttpPost(VisualizationConfiguration.VisualizeRoute)]
    public ActionResult Post([FromBody] JsonElement jsonElement)
    {
        _logger.LogDebug($"{nameof(Post)} called.");
        // Run on a separate thread so we can return an immediate response..
        Task.Run(() => ProcessReceived(jsonElement));
        return StatusCode(StatusCodes.Status202Accepted);
    }

    private void ProcessReceived(JsonElement jsonElement)
    {
        string json = jsonElement.GetRawText();
        VisualizeRequest[]? requests = JsonConvert.DeserializeObject<VisualizeRequest[]>(json);
        if (requests is null)
        {
            _logger.LogWarning("Failed to deserialize asset from JSON.");
            return;
        }

        foreach (VisualizeRequest request in requests)
            _state.AddOutcome(new(), request);
    }
}
