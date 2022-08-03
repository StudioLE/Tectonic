using NUnit.Framework;
using StudioLE.Core.System;

namespace StudioLE.Core.Tests.System;

internal sealed class StringHelpersTests
{
    [Test]
    public void StringHelpers_ReplaceLineEndings()
    {
        // Arrange
        string crlf = "Hello\r\nworld";
        string lf = "Hello\nworld";

        // Act
        string crlfReplaced = crlf.ReplaceWindowsLineEndings();
        string lfReplaced = lf.ReplaceWindowsLineEndings();

        // Assert
        Assert.True(crlfReplaced.Equals(lfReplaced));
        Assert.False(crlf.Equals(lf));
    }
}
