using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StudioLE.Core.System;

namespace Geometrician.Core.Visualization;

public class VisualizeRequestConverter : JsonConverter<VisualizeRequest>
{
    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, VisualizeRequest request, JsonSerializer serializer)
    {
        JObject jObject = new();

        // Assets
        // JToken? asset = JRaw.FromObject(request.Asset);
        // jObject.Add("Asset", asset);

        // Model
        JToken modelJRaw = JRaw.Parse(request.Model.ToJson(true));
        jObject.Add("Model", modelJRaw);

        // Assemblies
        string[] assemblies = request
            .Model
            .GetElementTypeAssemblies()
            .Select(x => x.Location)
            .ToArray();
        jObject.Add("Assemblies", new JArray(assemblies));

        jObject.WriteTo(writer);
    }

    /// <inheritdoc />
    public override VisualizeRequest ReadJson(
        JsonReader reader,
        Type objectType,
        VisualizeRequest existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        JObject jObject = JObject.Load(reader);

        // Assemblies
        string[] assemblyPaths = jObject["Assemblies"].Values<string>().ToArray();
        // TODO: BEWARE THIS IS INSECURE
        foreach (string path in assemblyPaths)
            // TODO: Use assembly hash and only load if not already loaded
            Assembly.LoadFrom(path);
        bool forceTypeReload = true;

        // Model
        string modelJson = jObject["Model"].ToString();
        Model model = Model.FromJson(modelJson, out List<string> errors, forceTypeReload);
        if (errors.Any())
            Console.WriteLine(errors.Join());

        // Assets
        // string assetJson = jObject["Asset"].ToString();
        // IAsset asset = JsonConvert.DeserializeObject<IAsset>(assetJson);

        return new()
        {
            // Asset = asset,
            Model = model,
            Assemblies = assemblyPaths,
        };
    }
}
