using NUnit.Framework;
using StudioLE.Diagnostics;
using StudioLE.Diagnostics.NUnit;
using StudioLE.Verify;

namespace Cascade.Workflows.NUnit.Samples;

internal sealed class NUnitTestSamples
{
    private readonly IContext _context = new NUnitContext();

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
        await _context.Verify("Hello, world!");
    }

    [TestCase("actual", "expected")]
    public void NUnitTestSamples_TestCase(string actual, string expected)
    {
        Assert.That(actual, Is.Not.EqualTo(expected));
    }
}
