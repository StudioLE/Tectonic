using System.Net;

namespace Tectonic;

/// <summary>
/// The status of an <see cref="IActivity"/>.
/// </summary>
public record struct Status
{
    /// <summary>
    /// The HTTP status code describing the status.
    /// </summary>
    public HttpStatusCode StatusCode { get; init; } = HttpStatusCode.OK;

    /// <summary>
    /// The command line exit code describing the status.
    /// </summary>
    public byte ExitCode { get; init; } = 0;

    /// <summary>
    /// The message describing the status.
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Create a new instance of <see cref="Status"/>.
    /// </summary>
    public Status()
    {
    }

    /// <summary>
    /// DI constructor for <see cref="Status"/>.
    /// </summary>
    public Status(HttpStatusCode statusCode)
    {
        StatusCode = statusCode;
        ExitCode = GetExitCode(statusCode);
    }

    private static byte GetExitCode(HttpStatusCode statusCode)
    {
        int number = (int)statusCode;
        return number switch
        {
            // Information
            < 200 => 0,
            // Success
            < 300 => 0,
            // Redirection
            < 400 => 0,
            // Client error
            < 500 => (byte)(number - 300),
            // Server error
            < 600 => (byte)(number - 300),
            _ => 1
        };
    }
}
