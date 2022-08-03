namespace Lineweights.Core.Geometry;

/// <summary>
/// Methods to create <see cref="Circle"/>.
/// </summary>
public static class CreateCircle
{
    /// <summary>
    /// Create a circle from three points.
    /// </summary>
    /// <see href="https://math.stackexchange.com/a/4000949/1044137">Source</see>
    /// <see href="https://web.archive.org/web/20211211014611/http://www.ambrsoft.com/TrigoCalc/Circle3D.htm">Source</see>
    public static Circle ByThreePoints(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        double a = DeterminantA(p1, p2, p3);
        double b = DeterminantB(p1, p2, p3);
        double c = DeterminantC(p1, p2, p3);
        double d = DeterminantD(p1, p2, p3);

        if (a == 0)
            throw new("Failed to create an arc. The points are co-linear.");

        Vector3 center = CalculateCenter(a, b, c);
        double radius = CalculateRadius(a, b, c, d);
        return new(center, radius);
    }

    private static double DeterminantA(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        return p1.X * (p2.Y - p3.Y)
               - p1.Y * (p2.X - p3.X)
               + p2.X * p3.Y
               - p3.X * p2.Y;
    }

    private static double DeterminantB(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        return (Math.Pow(p1.X, 2) + Math.Pow(p1.Y, 2)) * (p3.Y - p2.Y)
               + (Math.Pow(p2.X, 2) + Math.Pow(p2.Y, 2)) * (p1.Y - p3.Y)
               + (Math.Pow(p3.X, 2) + Math.Pow(p3.Y, 2)) * (p2.Y - p1.Y);
    }

    private static double DeterminantC(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        return (Math.Pow(p1.X, 2) + Math.Pow(p1.Y, 2)) * (p2.X - p3.X)
               + (Math.Pow(p2.X, 2) + Math.Pow(p2.Y, 2)) * (p3.X - p1.X)
               + (Math.Pow(p3.X, 2) + Math.Pow(p3.Y, 2)) * (p1.X - p2.X);
    }

    private static double DeterminantD(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        return (Math.Pow(p1.X, 2) + Math.Pow(p1.Y, 2)) * (p3.X * p2.Y - p2.X * p3.Y)
               + (Math.Pow(p2.X, 2) + Math.Pow(p2.Y, 2)) * (p1.X * p3.Y - p3.X * p1.Y)
               + (Math.Pow(p3.X, 2) + Math.Pow(p3.Y, 2)) * (p2.X * p1.Y - p1.X * p2.Y);
    }

    private static Vector3 CalculateCenter(double a, double b, double c)
    {
        double x = (b / (2 * a)) * -1;
        double y = (c / (2 * a)) * -1;
        return new(x, y);
    }

    private static double CalculateRadius(double a, double b, double c, double d)
    {
        double n = (Math.Pow(b, 2) + Math.Pow(c, 2) - 4 * a * d)
                   / (4 * Math.Pow(a, 2));
        return Math.Sqrt(n);
    }
}
