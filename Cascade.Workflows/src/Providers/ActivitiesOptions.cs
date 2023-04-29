namespace Cascade.Workflows.Providers;

/// <summary>
/// Configuration options for activities.
/// </summary>
/// <remarks>
/// Follows the <see href="https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-7.0">options pattern</see>.
/// </remarks>
public class ActivitiesOptions
{
    /// <summary>
    /// The options section identifier.
    /// <see href="https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-7.0#bind-hierarchical-configuration-data-using-the-options-pattern"/>
    /// </summary>
    public const string Section = "Activities";

    /// <summary>
    /// A collection of assemblies to load
    /// </summary>
    public string[] Assemblies { get; set; } = Array.Empty<string>();
}
