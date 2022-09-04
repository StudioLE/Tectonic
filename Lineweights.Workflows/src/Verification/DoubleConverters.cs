﻿using Newtonsoft.Json;

namespace Lineweights.Workflows.Verification;

/// <see href="https://stackoverflow.com/a/55971248/247218"/>
internal class DoubleConverter : JsonConverter
{
    private readonly int _decimalPoints;

    public DoubleConverter(int decimalPoints)
    {
        _decimalPoints = decimalPoints;
    }

    /// <inheritdoc/>
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(double);
    }

    /// <inheritdoc/>
    public override void WriteJson(JsonWriter writer, object? obj, JsonSerializer serializer)
    {
        if (obj is not double value)
            throw new("Failed to convert double.");
        double rounded = Math.Round(value, _decimalPoints);
        string json = rounded.ApproximatelyEquals(0)
            ? 0.ToString()
            : rounded.ToString("G");
        writer.WriteRawValue(json);
    }

    /// <inheritdoc/>
    public override bool CanRead => false;

    /// <inheritdoc/>
    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}
