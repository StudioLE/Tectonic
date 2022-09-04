using System.IO;
using System.Security.Cryptography;
using Ardalis.Result;

namespace Lineweights.Workflows.Verification;


/// <inheritdoc cref="VerifierBase{T}"/>
public sealed class FileVerifier : VerifierBase<FileInfo>
{
    /// <inheritdoc />
    public FileVerifier(IVerifyContext context, string fileExtension) : base(context, fileExtension)
    {
    }

    /// <inheritdoc />
    protected override void WriteActual(FileInfo actual)
    {
        _receivedFile = actual;
    }

    protected override Result<bool> CompareEquality()
    {
        if (IsTextFile(_receivedFile))
            return base.CompareEquality();
        if(!_receivedFile.Exists)
            return Result<bool>.Error("The received file does not exist.");
        if(!_verifiedFile.Exists)
            return Result<bool>.Error("The verified file does not exist.");
        if(_receivedFile.Length != _verifiedFile.Length)
            return Result<bool>.Error("File size is different.", $"Actual  : {_receivedFile.Length}", $"Verified: {_verifiedFile.Length}");
        string actual = GetFileHash(_receivedFile);
        string verified = GetFileHash(_verifiedFile);
        if(!actual.Equals(verified))
            return Result<bool>.Error("File hashes are different.", $"Actual  : {actual}", $"Verified: {verified}");
        return true;
    }

    private static bool IsTextFile(FileInfo file)
    {
        string[] textFileExtensions =
        {
            ".txt",
            ".json",
            ".svg"
        };
        return textFileExtensions.Contains(file.Extension);
    }

    private static string GetFileHash(FileInfo file)
    {
        using MD5 md5 = MD5.Create();
        using FileStream stream = File.OpenRead(file.FullName);
        byte[] bytes = md5.ComputeHash(stream);
        return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
    }
}
