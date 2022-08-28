﻿using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace Lineweights.Dashboard.Core.Scripts;

/// <summary>
/// A <see href="https://refactoring.guru/design-patterns/facade">facade</see>
/// for the TypeScript <c>ModelViewer</c> class defined in ModelViewer.ts.
/// </summary>
public class ModelViewer
{
    private const string MethodIdentifier = "exports.ModelViewer.Create";
    private readonly IJSRuntime _js;
    private readonly ILogger<ModelViewer> _logger;

    /// <inheritdoc cref="ModelViewer"/>
    public ModelViewer(IJSRuntime js, ILogger<ModelViewer> logger)
    {
        _js = js;
        _logger = logger;
    }

    public async Task Create(string id, string uri)
    {
        _logger.LogDebug($"{nameof(Create)}() called.");
        try
        {
            await _js.InvokeVoidAsync(MethodIdentifier, id, uri);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Failed to create the ModelViewer. A {e.GetType()} exception was thrown: {e.Message}");
        }
    }
}
