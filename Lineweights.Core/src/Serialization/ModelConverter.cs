using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StudioLE.Core.System;

namespace Lineweights.Core.Serialization;

public class ModelConverter : JsonConverter<Model>
{
    /// <inheritdoc/>
    public override void WriteJson(JsonWriter writer, Model model, JsonSerializer serializer)
    {
        string json = model.ToJson(true);
        writer.WriteRawValue(json);
    }

    /// <inheritdoc/>
    public override Model ReadJson(
        JsonReader reader,
        Type objectType,
        Model existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        // TODO: Ensure assemblies are loaded.
        JRaw? jRaw = JRaw.Create(reader);
        string json = jRaw.ToString();
        Model model = Model.FromJson(json, out List<string> errors);
        if (errors.Any())
            Console.WriteLine(errors.Join());
        return model;
    }
}
