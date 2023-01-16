using System.IO;
using System.Reflection;
using Microsoft.Extensions.FileProviders;
using StudioLE.Core.System;

namespace Lineweights.Diagnostics.Samples;

/// <summary>
/// Sample scenes.
/// </summary>
public static class Scenes
{
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
        GeometricElements
    }

    /// <inheritdoc cref="Name.Brickwork"/>
    public static ElementInstance[] Brickwork()
    {
        return FromJson(Name.Brickwork)
            .AllElementsOfType<ElementInstance>()
            .ToArray();
    }

    /// <inheritdoc cref="Name.GeometricElements"/>
    public static GeometricElement[] GeometricElements()
    {
        return FromJson(Name.GeometricElements)
            .AllElementsOfType<GeometricElement>()
            .ToArray();
    }

    /// <summary>
    /// Load a sample model from JSON.
    /// </summary>
    public static Model FromJson(Name name)
    {
        Assembly assembly = typeof(Scenes).Assembly;
        EmbeddedFileProvider provider = new(assembly);
        IFileInfo file = provider.GetFileInfo("Resources/" + name + ".json");
        if (!file.Exists)
            throw new("The file does not exist.");
        using Stream stream = file.CreateReadStream();
        using StreamReader reader = new(stream);
        string json = reader.ReadToEnd();
        Model model = Model.FromJson(json, out List<string> errors, true);
        if (errors.Any())
            Console.WriteLine(errors.Join());
        return model;
    }

    /// <summary>
    /// Load a sample model from JSON.
    /// </summary>
    internal static FileInfo ToJson(Name name, IEnumerable<Element> elements)
    {
        throw new NotImplementedException();
#if false
        FileInfo file = new(name + ".json");
        Model model = new();
        model.AddElements(elements);
        string json = model.ToJson(true);
        System.IO.File.WriteAllText(file.FullName, json);
        return file;
#endif
    }
}
