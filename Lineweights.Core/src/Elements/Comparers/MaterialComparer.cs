namespace Lineweights.Core.Elements.Comparers;

/// <summary>
/// Compare the equality of <see cref="Material"/> by their properties.
/// </summary>
public sealed class MaterialComparer : IEqualityComparer<Material>
{
    /// <inheritdoc cref="MaterialComparer"/>
    public bool Equals(Material first, Material second)
    {
        if (ReferenceEquals(first, second))
            return true;
        if (first.GetType() != second.GetType())
            return false;

        return first.Color.Equals(second.Color)
               && first.SpecularFactor.Equals(second.SpecularFactor)
               && first.GlossinessFactor.Equals(second.GlossinessFactor)
               && first.Unlit == second.Unlit
               && first.Texture == second.Texture
               && first.DoubleSided == second.DoubleSided
               && first.RepeatTexture == second.RepeatTexture
               && first.NormalTexture == second.NormalTexture
               && first.InterpolateTexture == second.InterpolateTexture
               && first.EmissiveTexture == second.EmissiveTexture
               && first.EmissiveFactor.Equals(second.EmissiveFactor);
    }

    /// <inheritdoc cref="MaterialComparer"/>
    public int GetHashCode(Material obj)
    {
        HashCode hashCode = new();
        hashCode.Add(obj.Color);
        hashCode.Add(obj.SpecularFactor);
        hashCode.Add(obj.GlossinessFactor);
        hashCode.Add(obj.Unlit);
        hashCode.Add(obj.Texture);
        hashCode.Add(obj.DoubleSided);
        hashCode.Add(obj.RepeatTexture);
        hashCode.Add(obj.NormalTexture);
        hashCode.Add(obj.InterpolateTexture);
        hashCode.Add(obj.EmissiveTexture);
        hashCode.Add(obj.EmissiveFactor);
        return hashCode.ToHashCode();
    }
}
