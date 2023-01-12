using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Components;
using StudioLE.Core.System;

namespace Geometrician.Components.Shared;

public class TemplatedComponentBase : ComponentBase
{
    [Parameter]
    public RenderFragment ChildContent { get; set; } = default!;

    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    internal ClassBuilder Classes { get; } = new();

    internal StyleBuilder Styles { get; } = new();

    protected string Class { get; private set; } = string.Empty;

    protected string Style { get; private set; } = string.Empty;

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (AdditionalAttributes is not null)
        {
            Classes.AddFromAdditionalAttributes(AdditionalAttributes);
            Styles.AddFromAdditionalAttributes(AdditionalAttributes);
        }
        Class = Classes.Build();
        Style = Styles.Build();
        base.OnParametersSet();
    }

    internal class ClassBuilder : Collection<string>
    {
        private string? _classAttr;

        public void AddFromAdditionalAttributes(IReadOnlyDictionary<string, object> additionalAttributes)
        {
            _classAttr = additionalAttributes.TryGetValue("class", out object? value)
                         && value is string str
                ? str
                : null;
        }

        public string Build()
        {
            return Enumerable.Empty<string>()
                .Concat(this)
                .Append(_classAttr ?? string.Empty)
                .Join(" ");
        }
    }

    internal class StyleBuilder : Dictionary<string, string>
    {
        private string? _styleAttr;

        public void AddFromAdditionalAttributes(IReadOnlyDictionary<string, object> additionalAttributes)
        {
            _styleAttr = additionalAttributes.TryGetValue("style", out object? value)
                         && value is string styleAttr
                ? styleAttr
                : null;
        }

        public string Build()
        {
            return Enumerable.Empty<string>()
                .Concat(this.Select(x => $"{x.Key}: {x.Value}"))
                .Append(_styleAttr ?? string.Empty)
                .Join("; ");
        }
    }
}
