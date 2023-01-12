using System.IO;
using StudioLE.Core.Results;

namespace Lineweights.Diagnostics.Verification;

/// <summary>
/// The engine specific verification context.
/// </summary>
public interface IVerifyContext
{
    /// <summary>
    /// The unique file name prefix for the executed test.
    /// </summary>
    public string FileNamePrefix { get; }

    /// <summary>
    /// The directory to store the verification files.
    /// </summary>
    public DirectoryInfo Directory { get; }

    /// <summary>
    /// Process the result after verification.
    /// </summary>
    public void OnResult(IResult result, FileInfo receivedFile, FileInfo verifiedFile);
}
