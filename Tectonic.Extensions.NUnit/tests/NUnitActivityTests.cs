using System.Reflection;
using System.Xml;
using NUnit.Framework;
using StudioLE.Extensions.System.Reflection;

namespace Tectonic.Extensions.NUnit.Tests;

internal sealed class NUnitActivityTests
{
    private const string AssemblyPath = "Tectonic.Extensions.NUnit.Samples.dll";
    private const string ActivityKey = "Tectonic.Extensions.NUnit.Samples.NUnitTestSamples.NUnitTestSamples_Test_Verify";
    private readonly Assembly _assembly = AssemblyHelpers.LoadFileByRelativePath(AssemblyPath);
    private NUnitActivityProvider _provider;

    [SetUp]
    public void SetUp()
    {
        _provider = new NUnitActivityProviderBuilder()
            .Add(_assembly)
            .Build();
    }


    [Test]
    public void NUnitActivity_InputType()
    {
        // Arrange
        IActivity activity = _provider.Get(ActivityKey) ?? throw new("Failed to get activity.");

        // Act
        Type inputType = activity.InputType;

        // Assert
        Assert.That(inputType.FullName, Is.EqualTo(typeof(object).FullName));
    }


    [Test]
    public void NUnitActivity_OutputType()
    {
        // Arrange
        IActivity activity = _provider.Get(ActivityKey) ?? throw new("Failed to get activity.");

        // Act
        Type inputType = activity.OutputType;

        // Assert
        Assert.That(inputType.FullName, Is.EqualTo(typeof(XmlNode).FullName));
    }

    [Test]
    public async Task NUnitActivity_Execute()
    {
        // Arrange
        IActivity activity = _provider.Get(ActivityKey) ?? throw new("Failed to get activity.");
        Type inputType = activity.InputType;
        object inputs = Activator.CreateInstance(inputType) ?? throw new("Failed to create inputs.");

        // Act
        object? result = await activity.ExecuteNonGeneric(inputs);

        // Assert
        Assert.That(result, Is.Not.Null, "Output");
        Assert.That(result!.GetType().FullName, Is.EqualTo(typeof(XmlElement).FullName), "Output type");
        if (result is XmlNode outputs)
            Assert.That(outputs, Is.Not.Null, "Output value");
    }
}
