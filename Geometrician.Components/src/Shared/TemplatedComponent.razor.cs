using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Components;
using StudioLE.Core.System;

namespace Geometrician.Components.Shared;

/// <summary>
/// An abstract base for <see cref="IComponent"/> to inherit from.
/// Includes basic implementations for <see cref="ChildContent"/>, <see cref="AdditionalAttributes"/>,
/// <see cref="Class"/> derived from a <see cref="ClassBuilder"/>, and
/// <see cref="Style"/> derived from a <see cref="StyleBuilder"/>.
/// </summary>
public abstract class TemplatedComponentBase : ComponentBase
{
    /// <summary>
    /// The child content passed to the component.
    /// </summary>
    [Parameter]
    public RenderFragment ChildContent { get; set; } = default!;

    /// <summary>
    /// Any additional attributes which were not matched to <see cref="ParameterAttribute"/>.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <inheritdoc cref="ClassBuilder"/>
    internal ClassBuilder Classes { get; } = new();

    /// <inheritdoc cref="StyleBuilder"/>
    internal StyleBuilder Styles { get; } = new();

    /// <summary>
    /// The class to apply to the rendered component.
    /// </summary>
    protected string Class { get; private set; } = string.Empty;

    /// <summary>
    /// The class to apply to the rendered component.
    /// </summary>
    protected string Style { get; private set; } = string.Empty;

    /// <inheritdoc/>
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

    /// <summary>
    /// Build a string of css classes using a <see href="https://refactoring.guru/design-patterns/builder">builder pattern</see>.
    /// </summary>
    internal class ClassBuilder : Collection<string>
    {
        private string? _classAttr;

        /// <summary>
        /// Add the classes defined in <paramref name="additionalAttributes"/>.
        /// </summary>
        /// <param name="additionalAttributes">The dictionary of additional attributes.</param>
        public void AddFromAdditionalAttributes(IReadOnlyDictionary<string, object> additionalAttributes)
        {
            _classAttr = additionalAttributes.TryGetValue("class", out object? value)
                         && value is string str
                ? str
                : null;
        }

        /// <summary>
        /// <inheritdoc cref="ClassBuilder"/>
        /// </summary>
        /// <returns>A string of css classes.</returns>
        public string Build()
        {
            return Enumerable.Empty<string>()
                .Concat(this)
                .Append(_classAttr ?? string.Empty)
                .Join(" ");
        }
    }

    /// <summary>
    /// Build css attributes using a <see href="https://refactoring.guru/design-patterns/builder">builder pattern</see>.
    /// </summary>
    internal class StyleBuilder : Dictionary<string, string>
    {
        private string? _styleAttr;

        /// <summary>
        /// Add the styles defined in <paramref name="additionalAttributes"/>.
        /// </summary>
        /// <param name="additionalAttributes">The dictionary of additional attributes.</param>
        public void AddFromAdditionalAttributes(IReadOnlyDictionary<string, object> additionalAttributes)
        {
            _styleAttr = additionalAttributes.TryGetValue("style", out object? value)
                         && value is string styleAttr
                ? styleAttr
                : null;
        }

        /// <summary>
        /// <inheritdoc cref="StyleBuilder"/>
        /// </summary>
        /// <returns>A string of css styles.</returns>
        public string Build()
        {
            return Enumerable.Empty<string>()
                .Concat(this.Select(x => $"{x.Key}: {x.Value}"))
                .Append(_styleAttr ?? string.Empty)
                .Join("; ");
        }
    }
}
