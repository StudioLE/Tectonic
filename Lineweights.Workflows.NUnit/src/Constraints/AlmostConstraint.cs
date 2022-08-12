using NUnit.Framework.Constraints;

namespace Lineweights.Workflows.NUnit.Constraints;

/// <summary>
/// Compare an actual value with an expected value within a given a tolerance.
/// </summary>
/// <remarks>
/// <see href="https://docs.nunit.org/articles/nunit/extending-nunit/Custom-Constraints.html">Reference</see>
/// </remarks>
public sealed class AlmostConstraint : EqualConstraint
{
    /// <summary>
    /// The default tolerance value.
    /// </summary>
    public const double Epsilon = 1e-5;

    /// <inheritdoc cref="AlmostConstraint" />
    public AlmostConstraint(double expected, double tolerance = Epsilon) : base(expected)
    {
        Within(tolerance);
    }


    /// <inheritdoc cref="AlmostConstraint" />
    public static AlmostConstraint Almost(double expected, double tolerance = Epsilon)
    {
        return new(expected, tolerance);
    }
}

/// <summary>
/// Extend Is to include <see cref="AlmostConstraint"/>.
/// </summary>
/// <remarks>
/// <see href="https://docs.nunit.org/articles/nunit/extending-nunit/Custom-Constraints.html">Reference</see>
/// </remarks>
public sealed class Is : global::NUnit.Framework.Is
{
    /// <inheritdoc cref="AlmostConstraint" />
    public static AlmostConstraint Almost(double expected, double tolerance = AlmostConstraint.Epsilon)
    {
        return new(expected, tolerance);
    }
}

/// <summary>
/// Methods to extend <see cref="AlmostConstraint"/>.
/// </summary>
/// <remarks>
/// <see href="https://docs.nunit.org/articles/nunit/extending-nunit/Custom-Constraints.html">Reference</see>
/// </remarks>
public static class AlmostConstraintExtensions
{
    /// <inheritdoc cref="AlmostConstraint" />
    public static AlmostConstraint Almost(this ConstraintExpression expression, double expected, double tolerance = AlmostConstraint.Epsilon)
    {
        var constraint = new AlmostConstraint(expected, tolerance);
        expression.Append(constraint);
        return constraint;
    }
}
