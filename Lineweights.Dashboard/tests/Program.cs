using System.IO;
using System.Reflection;
using System.Xml;
using Lineweights.Workflows.NUnit;
using Lineweights.Workflows.Results;
using NUnit.Engine;
using NUnit.Framework;

namespace Lineweights.Dashboard.Mock;

internal sealed class Program
{
    public static void Main(string[] args)
    {
        TestRunner();
    }

    private static void TestRunner()
    {
        // Get an interface to the engine
        ITestEngine engine = TestEngineActivator.CreateInstance();

        FileInfo dll = new(@"E:\Repos\Hypar\Lineweights\Lineweights.Flex\samples\bin\Debug\net6.0\Lineweights.Flex.Samples.dll");

        if (!dll.Exists)
            throw new("File not found..");

        string[] dlls = Directory
            .EnumerateFiles(dll.DirectoryName!)
            .Where(x => x.EndsWith(".dll"))
            .ToArray();

        Assembly assembly = Assembly.LoadFrom(dll.FullName);

        // Create a simple test package - one assembly, no special settings
        TestPackage package = new(dll.FullName);

        // Get a runner for the test package
        ITestRunner runner = engine.GetRunner(package);

        // Run all the tests in the assembly
        XmlNode testResult = runner.Run(listener: null, TestFilter.Empty);
    }

    private static void SendToDashboardAfterTest()
    {
        // Arrange
        SendToDashboardAfterTest attribute = new();
        if (attribute.Strategy is not SendToDashboard strategy)
        {
            Assert.Fail("Strategy type");
            return;
        }
        Model model = Create_Arc_By3Points(-3, 4, 4, 5, 1, -4);

        // Act
        strategy.Execute(model, new());

        // Assert
        Console.WriteLine(strategy.State);
    }

    private static Model Create_Arc_By3Points(double x1, double y1, double x2, double y2, double x3, double y3)
    {
        // Arrange
        var start = new Vector3(x1, y1);
        var end = new Vector3(x2, y2);
        var pointOnArc = new Vector3(x3, y3);
        var ab = new ModelCurve(new Line(start, pointOnArc), MaterialHelpers.MaterialByName("Red"));
        var bc = new ModelCurve(new Line(pointOnArc, end), MaterialHelpers.MaterialByName("Blue"));
        Model model = new();

        // Act
        Arc arc = CreateArc.ByThreePoints(start, end, pointOnArc);

        // Preview
        model.AddElements(ab, bc, new ModelCurve(arc, MaterialHelpers.MaterialByName("Orange")));

        return model;
    }
}
