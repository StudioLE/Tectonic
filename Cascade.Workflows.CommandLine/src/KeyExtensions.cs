namespace Cascade.Workflows.CommandLine;

public static class KeyExtensions
{
    public static string ToLongOption(this string str)
    {
        return "--" + str.ToLower();
    }
}
