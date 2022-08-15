using System.IO;
using Ardalis.Result;

namespace Lineweights.Workflows;

/// <summary>
/// Sample scenes.
/// </summary>
public static class Scenes
{
    private static readonly string _pathFromRootToSamples = "Lineweights.Workflows/samples";

    private static readonly string[] _possiblePathsToRoot =
    {
        "../../../../../", "../../"
    };

    /// <summary>
    /// Scene names.
    /// </summary>
    public enum Name
    {
        /// <summary>
        /// A sample scene of brickwork.
        /// </summary>
        Brickwork,

        /// <summary>
        /// A sample scene of geometric elements.
        /// </summary>
        GeometricElements,

        /// <summary>
        /// A sample scene of geometric elements with views and a sheet.
        /// </summary>
        GeometricElementsOnSheet
    }

    /// <inheritdoc cref="Name.Brickwork"/>
    public static ElementInstance[] Brickwork()
    {
        return FromJson<ElementInstance>(Name.Brickwork);
    }

    /// <inheritdoc cref="Name.GeometricElements"/>
    public static GeometricElement[] GeometricElements()
    {
        return FromJson<GeometricElement>(Name.GeometricElements);
    }

    /// <summary>
    /// Load a sample model from JSON.
    /// </summary>
    private static T[] FromJson<T>(Name name) where T : Element
    {
        FileInfo file = File(name);
        Result<Model> result = ModelHelpers.TryGetFromJsonFile(file);
        return Validate.OrThrow(result, $"Failed to load scene {name} from JSON")
            .AllElementsOfType<T>()
            .ToArray();
    }

    /// <summary>
    /// Load a sample model from JSON.
    /// </summary>
    public static FileInfo ToJson(Name name, IEnumerable<Element> elements)
    {
        FileInfo file = File(name);
        Model model = new();
        model.AddElements(elements);
        string json = model.ToJson(true);
        System.IO.File.WriteAllText(file.FullName, json);
        return file;
    }

    private static FileInfo File(Name name)
    {
        string cwd = Directory.GetCurrentDirectory();
        FileInfo? file = _possiblePathsToRoot
            .Select(pathToRoot => new FileInfo(Path.Combine(cwd, pathToRoot, _pathFromRootToSamples, $"{name}.json")))
            .FirstOrDefault(x => x.Exists);
        return file ?? throw new FileNotFoundException("Could not find the sample file: " + name);
    }
}
