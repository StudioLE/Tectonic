using System.ComponentModel.DataAnnotations;

namespace Tectonic.Extensions.CommandLine.Tests.Resources;

public record struct ExampleRecordStruct()
{
    [Required]
    public string RecordStructStringValue { get; set; } = string.Empty;

    [Required]
    [Argument]
    public string RecordStructArgValue { get; set; } = string.Empty;

    [ValidateComplexType]
    public ExampleNestedRecordStruct NestedRecordStruct { get; set; } = new();
}
