using System.ComponentModel.DataAnnotations;

namespace Cascade.Workflows.CommandLine.Tests.Resources;

public record struct ExampleRecordStruct()
{
    [Required]
    public string RecordStructStringValue { get; set; } = string.Empty;

    [Required]
    [Argument]
    public string RecordStructArgValue { get; set; } = string.Empty;
}
