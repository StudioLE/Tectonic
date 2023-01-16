namespace StudioLE.Core.Measurements;

/// <summary>
/// Methods to help format anything.
/// </summary>
public static class FormattingHelpers
{
    /// <summary>
    /// Convert a <see cref="TimeSpan"/> to a natural language representation.
    /// </summary>
    /// <example>
    /// <code>
    /// TimeSpan.FromSeconds(10).ToNaturalLanguage();
    /// // 10 seconds
    /// </code>
    /// </example>
    public static string ToNaturalLanguage(this TimeSpan @this)
    {
        const int daysInWeek = 7;
        const int daysInMonth = 30;
        const int daysInYear = 365;
        const long threshold = 100 * TimeSpan.TicksPerMillisecond;
        @this = @this.TotalSeconds < 0
            ? TimeSpan.FromSeconds(@this.TotalSeconds * -1)
            : @this;
        return (@this.Ticks + threshold) switch
        {
            < 2 * TimeSpan.TicksPerSecond => "a second",
            < 1 * TimeSpan.TicksPerMinute => @this.Seconds + " seconds",
            < 2 * TimeSpan.TicksPerMinute => "a minute",
            < 1 * TimeSpan.TicksPerHour => @this.Minutes + " minutes",
            < 2 * TimeSpan.TicksPerHour => "an hour",
            < 1 * TimeSpan.TicksPerDay => @this.Hours + " hours",
            < 2 * TimeSpan.TicksPerDay => "a day",
            < 1 * daysInWeek * TimeSpan.TicksPerDay => @this.Days + " days",
            < 2 * daysInWeek * TimeSpan.TicksPerDay => "a week",
            < 1 * daysInMonth * TimeSpan.TicksPerDay => (@this.Days / daysInWeek).ToString("F0") + " weeks",
            < 2 * daysInMonth * TimeSpan.TicksPerDay => "a month",
            < 1 * daysInYear * TimeSpan.TicksPerDay => (@this.Days / daysInMonth).ToString("F0") + " months",
            < 2 * daysInYear * TimeSpan.TicksPerDay => "a year",
            _ => (@this.Days / daysInYear).ToString("F0") + " years"
        };
    }

    /// <summary>
    /// Convert a <see cref="DateTime"/> to a natural language representation.
    /// </summary>
    /// <example>
    /// <code>
    /// (DateTime.Now - TimeSpan.FromSeconds(10)).ToNaturalLanguage()
    /// // 10 seconds ago
    /// </code>
    /// </example>
    public static string ToNaturalLanguage(this DateTime @this)
    {
        TimeSpan timeSpan = @this - DateTime.Now;
        return timeSpan.TotalSeconds switch
        {
            >= 1 => timeSpan.ToNaturalLanguage() + " until",
            <= -1 => timeSpan.ToNaturalLanguage() + " ago",
            _ => "now"
        };
    }
}
