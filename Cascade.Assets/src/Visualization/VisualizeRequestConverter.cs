using System.IO;
using System.Reflection;
using Elements;
using Geometrician.Core.Elements;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StudioLE.Core.System;

namespace Cascade.Assets.Visualization;

public class VisualizeRequestConverter : JsonConverter<VisualizeRequest>
{
    /// <inheritdoc/>
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

    /// <inheritdoc/>
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
        // https://learn.microsoft.com/en-us/dotnet/framework/deployment/best-practices-for-assembly-loading
        foreach (string path in assemblyPaths)
        {
            Assembly[] loadedAssemblies = AppDomain.CurrentDomain
                .GetAssemblies()
                .ToArray();
            AssemblyName[] referencedAssemblies = loadedAssemblies
                .SelectMany(x => x.GetReferencedAssemblies())
                .Distinct()
                .ToArray();
            string[] loadedAssemblyNames = AppDomain.CurrentDomain
                .GetAssemblies()
                .Select(x => x.GetName().Name)
                .ToArray();
            string[] referencedAssemblyNames = referencedAssemblies
                .Select(x => x.Name)
                .ToArray();
            string fileName = Path.GetFileName(path);
            if (fileName.EndsWith(".dll"))
                fileName = fileName.Substring(0, fileName.Length - 4);
            if (loadedAssemblyNames.Contains(fileName) || referencedAssemblyNames.Contains(fileName))
                continue;
            // TODO: Use assembly hash
            try
            {
                Assembly.LoadFrom(path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
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
            Assemblies = assemblyPaths
        };
    }
}
