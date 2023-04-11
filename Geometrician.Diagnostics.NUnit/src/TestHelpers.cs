using System.IO;
using StudioLE.Core.System.IO;

namespace Geometrician.Diagnostics.NUnit;

/// <summary>
/// Methods to help with creating and viewing samples.
/// </summary>
public static class TestHelpers
{
    /// <summary>
    /// Get a temp file with <paramref name="fileExtension"/>
    /// and named by TestContext.CurrentContext.Test.FullName plus the current date and time.
    /// </summary>
    public static FileInfo FileByTestContext(string fileExtension)
    {
        TestContext context = TestContext.CurrentContext;
        string testName = context.Test.FullName;
        string now = DateTime.UtcNow.ToString("yyyy-MM-ddTHH-mm-ss-fff");
        string fileName = $"{testName}-{now}.{fileExtension}";
        fileName = PathHelpers.ReplaceInvalidFileNameChars(fileName);
        return new(Path.GetTempPath() + fileName);
    }
}
