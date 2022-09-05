using Lineweights.Core.Distribution;
using Lineweights.Drawings.Rendering;
using StudioLE.Core.Exceptions;

namespace Lineweights.Drawings;

/// <summary>
/// The direction of a view.
/// </summary>
public enum ViewDirection
{
    /// <summary>
    /// A view looking from the top to the bottom.
    /// </summary>
    Top,

    /// <summary>
    /// A view looking from the bottom to the top.
    /// </summary>
    Bottom,

    /// <summary>
    /// A view looking from the front to the back.
    /// </summary>
    Front,

    /// <summary>
    /// A view looking from the back to the front.
    /// </summary>
    Back,

    /// <summary>
    /// A view looking from the left to the right.
    /// </summary>
    Left,

    /// <summary>
    /// A view looking from the right to the left.
    /// </summary>
    Right
}

/// <summary>
/// Build a <see cref="View"/> using a <see href="https://refactoring.guru/design-patterns/builder">builder pattern</see>.
/// </summary>
public sealed class ViewBuilder
{
    #region Fields

    private string _name = string.Empty;
    private Vector3 _right = Vector3.XAxis;
    private Vector3 _up = Vector3.YAxis.Negate();
    private Vector3 _facing = Vector3.ZAxis.Negate();
    private double _widthPadding;
    private double _heightPadding;
    private double _depthPadding;
    private BBox3? _scopeBounds;
    private IReadOnlyCollection<Element>? _elements;
    private double _scale = 1;
    private IRenderStrategy _renderStrategy = new WireframeRender();

    #endregion

    #region Builder methods

    /// <summary>
    /// Set the <see cref="View.Scope"/> as a <see cref="BBox3"/>.
    /// </summary>
    public ViewBuilder ScopeBounds(BBox3 bounds)
    {
        _scopeBounds = bounds;
        return this;
    }

    /// <summary>
    /// Set the padding used when automatically determining the <see cref="View.Scope"/>
    /// as the bounds around the <see cref="ViewScope.Elements"/>.
    /// </summary>
    public ViewBuilder ScopePadding(double width, double height, double depth)
    {
        _widthPadding = width;
        _heightPadding = height;
        _depthPadding = depth;
        return this;
    }

    /// <summary>
    /// The direction of the <see cref="View"/>.
    /// </summary>
    public ViewBuilder ViewDirection(ViewDirection direction)
    {
        _name = direction.ToString();
        switch (direction)
        {
            case Drawings.ViewDirection.Top:
                _right = Vector3.XAxis;
                _up = Vector3.YAxis;
                _facing = Vector3.ZAxis.Negate();
                break;
            case Drawings.ViewDirection.Bottom:
                _right = Vector3.XAxis.Negate();
                _up = Vector3.YAxis;
                _facing = Vector3.ZAxis;
                break;
            case Drawings.ViewDirection.Front:
                _right = Vector3.XAxis;
                _up = Vector3.ZAxis;
                _facing = Vector3.YAxis;
                break;
            case Drawings.ViewDirection.Back:
                _right = Vector3.XAxis.Negate();
                _up = Vector3.ZAxis;
                _facing = Vector3.YAxis.Negate();
                break;
            case Drawings.ViewDirection.Left:
                _right = Vector3.YAxis.Negate();
                _up = Vector3.ZAxis;
                _facing = Vector3.XAxis;
                break;
            case Drawings.ViewDirection.Right:
                _right = Vector3.YAxis;
                _up = Vector3.ZAxis;
                _facing = Vector3.XAxis.Negate();
                break;
            default:
                throw new EnumSwitchException<ViewDirection>("Failed to set view direction.", direction);
        }

        return this;
    }

    /// <inheritdoc cref="ViewScope.Elements"/>
    public ViewBuilder ElementsInView(IReadOnlyCollection<Element> elements)
    {
        _elements = elements;
        return this;
    }

    /// <inheritdoc cref="View.Scale"/>
    public ViewBuilder Scale(double scale)
    {
        _scale = scale;
        return this;
    }

    /// <inheritdoc cref="View.RenderStrategy"/>
    public ViewBuilder RenderStrategy(IRenderStrategy strategy)
    {
        _renderStrategy = strategy;
        return this;
    }

    #endregion

    #region Execution methods

    /// <summary>
    /// Build the <see cref="View"/>.
    /// </summary>
    public View Build()
    {
        IReadOnlyCollection<Element> elements = _elements ?? throw new("Failed to build view. Elements must be set.");
        if (elements.Count == 0)
            throw new("Failed to build view. There are no elements.");

        // TODO: If we're using a BBox3 then we're limited to only cardinal directions?
        BBox3 scopeBounds = _scopeBounds ?? CreateBBox3.ByElements(elements);
        double scopeWidth = _right.Dimension(scopeBounds) + _widthPadding * 2;
        double scopeHeight = _up.Dimension(scopeBounds) + _heightPadding * 2;
        double scopeDepth = _facing.Dimension(scopeBounds) + _depthPadding * 2;
        Vector3 scopeOrigin = scopeBounds.Center() + _facing * scopeDepth * 0.5 * -1;

        BBox3 viewBounds = CreateBBox3.ByLengths2d(scopeWidth * _scale, scopeHeight * _scale);

        return new()
        {
            Name = _name,
            IsElementDefinition = true,
            Scope = new()
            {
                Origin = scopeOrigin,
                RightDirection = _right,
                UpDirection = _up,
                FacingDirection = _facing,
                Width = scopeWidth,
                Height = scopeHeight,
                Depth = scopeDepth,
                Elements = elements.ToList(),
            },
            Bounds = viewBounds,
            Scale = _scale,
            RenderStrategy = _renderStrategy
        };
    }

    #endregion
}
