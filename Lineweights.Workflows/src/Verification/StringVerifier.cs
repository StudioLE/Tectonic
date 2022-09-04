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
    protected override void WriteActual(string actual)
    {
        File.WriteAllText(_receivedFile.FullName, actual, Verify.Encoding);
    }
}
