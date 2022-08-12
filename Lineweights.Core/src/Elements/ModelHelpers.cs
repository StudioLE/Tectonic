using System.IO;
using Ardalis.Result;

namespace Lineweights.Core.Elements;

/// <summary>
/// Methods to extend <see cref="Model"/>.
/// </summary>
public static class ModelHelpers
{
    /// <summary>
    /// Get all <see cref="Element"/> from the model.
    /// </summary>
    public static Result<Model> TryGetFromJsonFile(FileInfo file)
    {
        if (!file.Exists)
            return Result<Model>.Error("The file does not exist.");
        string json = File.ReadAllText(file.FullName);
        return TryGetFromJson(json);
    }

    /// <summary>
    /// Get all <see cref="Element"/> from the model.
    /// </summary>
    public static Result<Model> TryGetFromJson(string json)
    {
        Model model = Model.FromJson(json, out List<string> errors, true);
        // TODO: This is a temporary workaround for an unknown issue with InterfaceConverter
        if (errors.Any(error => !error.EndsWith("' not found in JSON. Path 'Interpolation'.")
            && !error.EndsWith("' not found in JSON. Path 'RenderStrategy'.")))
            return Result<Model>.Error(string.Join(Environment.NewLine, errors));
        return model;
    }
}
