using NUnit.Framework;
using StudioLE.Verify;
using StudioLE.Verify.NUnit;

namespace Cascade.Workflows.NUnit.Samples;

internal sealed class NUnitTestSamples
{
    private readonly IVerify _verify = new NUnitVerify();

    [Test]
    public void NUnitTestSamples_Test()
    {
        Assert.Pass();
    }

    [Test]
    public async Task NUnitTestSamples_Test_Async()
    {
        await Task.CompletedTask;
        Assert.Pass();
    }

    [Test]
    public async Task NUnitTestSamples_Test_Verify()
    {
        await _verify.String("Hello, world!");
    }

    [TestCase("actual", "expected")]
    public void NUnitTestSamples_TestCase(string actual, string expected)
    {
        Assert.That(actual, Is.Not.EqualTo(expected));
    }
}
