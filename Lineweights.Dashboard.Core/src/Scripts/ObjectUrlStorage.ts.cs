using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace Lineweights.Dashboard.Core.Scripts;

/// <summary>
/// A <see href="https://refactoring.guru/design-patterns/facade">facade</see>
/// for the TypeScript <c>ObjectUrlStorage</c> class defined in ObjectUrlStorage.ts.
/// </summary>
public class ObjectUrlStorage
{
    private readonly IJSRuntime _js;
    private readonly ILogger<ObjectUrlStorage> _logger;

    public ObjectUrlStorage(IJSRuntime js, ILogger<ObjectUrlStorage> logger)
    {
        _js = js;
        _logger = logger;
    }

    public async Task<Uri> Create(string fileName, string contentType, byte[] byteArray)
    {
        _logger.LogDebug($"{nameof(Create)}() called.");
        try
        {
            string result = await _js.InvokeAsync<string>("exports.ObjectUrlStorage.Create", fileName, contentType, byteArray);
            Uri uri = new(result);
            return uri;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Failed to create an ObjectUrl. A {e.GetType()} exception was thrown: {e.Message}");
            throw;
        }
    }

    public async Task<string> GetAsString(string url)
    {
        _logger.LogDebug($"{nameof(GetAsString)}() called.");
        try
        {
            string result = await _js.InvokeAsync<string>("exports.ObjectUrlStorage.GetAsString", url);
            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Failed to get ObjectUrl. A {e.GetType()} exception was thrown: {e.Message}");
            throw;
        }
    }
}
