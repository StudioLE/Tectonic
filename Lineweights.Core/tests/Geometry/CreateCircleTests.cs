﻿using Lineweights.Workflows.Results;

namespace Lineweights.Core.Tests.Geometry;

[SendToDashboardAfterTest]
internal sealed class CreateCircleTests : ResultModel
{
    [TestCase(1, 0, 0, 1, 0, 0)]
    [TestCase(1, 0, 0, 1, 2, 2)]
    [TestCase(-3, 4, 4, 5, 1, -4)]
    public void Create_Circle(double x1, double y1, double x2, double y2, double x3, double y3)
    {
        // Arrange
        var start = new Vector3(x1, y1);
        var end = new Vector3(x2, y2);
        var pointOnCircle = new Vector3(x3, y3);
        var ab = new ModelCurve(new Line(start, pointOnCircle), MaterialByName("Red"));
        var bc = new ModelCurve(new Line(pointOnCircle, end), MaterialByName("Blue"));

        // Act
        Circle circle = CreateCircle.ByThreePoints(start, end, pointOnCircle);

        // Preview
        Model.AddElements(ab, bc, new ModelCurve(circle, MaterialByName("Orange")));

        // Assert
        Verify.Geometry(circle);
    }
}
