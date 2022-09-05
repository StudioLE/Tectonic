using NUnit.Framework;
using StudioLE.Core.Measurements;

namespace StudioLE.Core.Tests.Measurements;

[TestFixture]
public class NaturalLanguageHelpersTests
{
    [TestCase("a second", 0)]
    [TestCase("a second", 1)]
    [TestCase("2 seconds", 2)]
    [TestCase("a minute", 0, 1)]
    [TestCase("5 minutes", 0, 5)]
    [TestCase("an hour", 0, 0, 1)]
    [TestCase("2 hours", 0, 0, 2)]
    [TestCase("a day", 0, 0, 24)]
    [TestCase("a day", 0, 0, 0, 1)]
    [TestCase("6 days", 0, 0, 0, 6)]
    [TestCase("a week", 0, 0, 0, 7)]
    [TestCase("4 weeks", 0, 0, 0, 29)]
    [TestCase("a month", 0, 0, 0, 30)]
    [TestCase("6 months", 0, 0, 0, 6 * 30)]
    [TestCase("a year", 0, 0, 0, 365)]
    [TestCase("68 years", int.MaxValue)]
    public void NaturalLanguageHelpers_TimeSpan(
        string expected,
        int seconds,
        int minutes = 0,
        int hours = 0,
        int days = 0
    )
    {
        // Arrange
        TimeSpan timeSpan = new(days, hours, minutes, seconds);

        // Act
        string result = timeSpan.ToNaturalLanguage();

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [TestCase("now", 0)]
    [TestCase("10 minutes ago", 0, -10)]
    [TestCase("10 minutes until", 10, 10)]
    [TestCase("68 years until", int.MaxValue)]
    [TestCase("68 years ago", int.MinValue)]
    public void NaturalLanguageHelpers_DateTime(
        string expected,
        int seconds,
        int minutes = 0,
        int hours = 0,
        int days = 0
    )
    {
        // Arrange
        TimeSpan timeSpan = new(days, hours, minutes, seconds);
        DateTime now = DateTime.Now;
        DateTime dateTime = now + timeSpan;

        // Act
        string result = dateTime.ToNaturalLanguage();

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }
}
