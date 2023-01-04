using System.IO;

namespace Lineweights.Workflows.Verification;


/// <inheritdoc cref="VerifierBase{T}"/>
public sealed class StringVerifier : VerifierBase<string>
{
    /// <inheritdoc />
    public StringVerifier(IVerifyContext context, string fileExtension) : base(context, fileExtension)
    {
    }

    /// <inheritdoc />
    protected override async Task Write(FileInfo file, string actual)
    {
        using StreamWriter writer = new(file.FullName, false, Verify.Encoding);
        await writer.WriteAsync(actual);
    }
}
