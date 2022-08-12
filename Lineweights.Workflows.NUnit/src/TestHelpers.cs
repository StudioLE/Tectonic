using System.Diagnostics;
using System.IO;
using StudioLE.Core.System;
using StudioLE.Core.System.IO;

namespace Lineweights.Workflows.NUnit;

/// <summary>
/// Methods to help with creating and viewing samples.
/// </summary>
public static class TestHelpers
{
    /// <summary>
    /// Get a temp file with <paramref name="fileExtension"/>
    /// and named by TestContext.CurrentContext.Test.FullName plus the current date and time.
    /// </summary>
    // TODO: Any calls to this should be replaced by SendToDashboard...
    public static FileInfo FileByTestContext(string fileExtension)
    {
        TestContext context = TestContext.CurrentContext;
        string testName = context.Test.FullName;
        string now = DateTime.UtcNow.ToString("yyyy-MM-ddTHH-mm-ss-fff");
        string fileName = $"{testName}-{now}.{fileExtension}";
        fileName = PathHelpers.ReplaceInvalidFileNameChars(fileName);
        return new(Path.GetTempPath() + fileName);
    }

    /// <summary>
    /// Open <paramref name="file"/> when the calling assembly is a DEBUG build.
    /// </summary>
    // TODO: Any calls to this should be replaced by SendToDashboard...
    public static void OpenInDebug(FileInfo file)
    {
        if (!AssemblyHelpers.IsDebugBuild())
            return;

        if (file is null || !file.Exists)
            throw new FileNotFoundException("Failed to open the sample file. It does not exist.");

        Process.Start(new ProcessStartInfo(file.FullName)
        {
            UseShellExecute = true
        });
    }
}
