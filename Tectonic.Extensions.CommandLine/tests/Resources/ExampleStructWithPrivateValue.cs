namespace Tectonic.Extensions.CommandLine.Tests.Resources;

public readonly struct ExampleStructWithPrivateValue
{
    private readonly int _value;

    // ReSharper disable once ConvertToAutoProperty
    public int ReadOnlyValue => _value;

    public ExampleStructWithPrivateValue(string value)
    {
        _value = int.Parse(value);
    }

}
