using StudioLE.Conversion;
using StudioLE.Extensions.System;
using StudioLE.Serialization.Composition;
using Tectonic.Extensions.CommandLine.Tests.Resources;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Tectonic.Extensions.CommandLine.Sample;

internal static class Program
{
    public static void Main(string[] args)
    {
        // Arrange
        const int maxAttemptCount = 3;
        bool exceededMaxAttempts = false;
        ConverterResolver resolver = ConverterResolver.Default();
        ExampleClass inputs = new();
        ObjectTree objectTree = new(inputs);

        // Act
        ObjectProperty[] properties = objectTree
            .FlattenProperties()
            .Where(x => x.CanSet())
            .ToArray();
        foreach (ObjectProperty property in properties)
        {
            int attemptCount = 0;
            while (true)
            {
                if (attemptCount >= maxAttemptCount)
                {
                    exceededMaxAttempts = true;
                    break;
                }
                IReadOnlyCollection<string> errors = property.ValidateValue();
                if (!errors.Any())
                    break;
                if (attemptCount > 0)
                    Console.WriteLine("Please try again.");

                Console.WriteLine(errors.Join());
                string? line = Console.ReadLine();
                line ??= string.Empty;
                object? parsed = resolver.TryConvert(line, property.Type);
                property.SetValue(parsed!);
                attemptCount++;
            }
            if (exceededMaxAttempts)
                break;
        }

        if (exceededMaxAttempts)
        {
            Console.WriteLine($"Failed to set value more than {maxAttemptCount} times.");
            return;
        }

        ISerializer serializer = new SerializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();
        object? value = objectTree.GetValue();
        string yaml = serializer.Serialize(value);
        Console.WriteLine(yaml);
    }
}
