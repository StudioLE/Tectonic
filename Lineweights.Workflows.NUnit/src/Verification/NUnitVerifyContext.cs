﻿//
// The majority of the logic in VerifyContext is from Verify v16.7.0
// https://github.com/VerifyTests/Verify
//
// This specific VerifyContext class is therefore under their license:
// https://github.com/VerifyTests/Verify/blob/16.7.0/license.txt

using System.IO;
using System.Reflection;
using System.Text;
using StudioLE.Core.Results;
using DiffEngine;
using Lineweights.Diagnostics.Verification;
using NUnit.Framework.Internal;
using StudioLE.Core.System;

namespace Lineweights.Workflows.NUnit.Verification;

/// <summary>
/// The NUnit specific <see cref="IVerifyContext"/>.
/// </summary>
internal class NUnitVerifyContext : IVerifyContext
{
    private readonly TestContext _context;
    private readonly Test _test;

    /// <inheritdoc/>
    public string FileNamePrefix { get; }

    /// <inheritdoc/>
    public DirectoryInfo Directory { get; }

    /// <inheritdoc cref="NUnitVerifyContext"/>
    public NUnitVerifyContext()
    {
        _context = TestContext.CurrentContext;
        _test = GetTest();
        FileNamePrefix = BuildFileNamePrefix();
        Directory = new(Path.Combine(_context.TestDirectory, "..", "..", "..", "Verify"));
    }

    /// <inheritdoc/>
    public void OnResult(IResult result, FileInfo receivedFile, FileInfo verifiedFile)
    {
        if (result is Success)
            return;
        if (AssemblyHelpers.IsDebugBuild())
            DiffRunner.LaunchAsync(receivedFile.FullName, verifiedFile.FullName);
        Assert.Fail(result.Errors.Prepend("Actual results did not match the verified results:").Join());
    }

    /// <summary>
    /// Determine the filename prefix to use for the <see cref="Verify"/> files.
    /// </summary>
    private string BuildFileNamePrefix()
    {
        Type type = _test.TypeInfo!.Type;
        MethodInfo method = _test.Method!.MethodInfo;
        object?[] parameterValues = GetParameterValues();
        string uniqueness = "";

        string typeName = GetTypeName(type);
        string methodName = method.Name;
        string parameterText = GetParameterText(method, parameterValues);
        return $"{typeName}.{methodName}{parameterText}{uniqueness}";
    }

    /// <see href="https://github.com/VerifyTests/Verify/blob/16.7.0/src/Verify.NUnit/Verifier.cs#L22-L38"/>
    private Test GetTest()
    {
        TestContext.TestAdapter adapter = _context.Test;
        FieldInfo field = typeof(TestContext.TestAdapter)
                              .GetField("_test", BindingFlags.Instance | BindingFlags.NonPublic)
                          ?? throw new("Could not find field `_test` on TestContext.TestAdapter.");
        Test test = (Test)field.GetValue(adapter);
        if (test.TypeInfo == null || test.Method is null)
            throw new("Expected Test.TypeInfo and Test.Method to not be null. Raise a Pull Request with a test that replicates this problem.");
        return test;
    }

    /// <see href="https://github.com/VerifyTests/Verify/blob/16.7.0/src/Verify/Naming/ReflectionFileNameBuilder.cs"/>
    private static string GetTypeName(Type type)
    {
        return type.IsNested
            ? $"{type.ReflectedType!.Name}.{type.Name}"
            : type.Name;
    }

    /// <see href="https://github.com/VerifyTests/Verify/blob/16.7.0/src/Verify.NUnit/Verifier.cs#L22-L38"/>
    private static object?[] GetParameterValues()
    {
        TestContext context = TestContext.CurrentContext;
        TestContext.TestAdapter adapter = context.Test;
        return adapter.Arguments;
    }

    /// <see href="https://github.com/VerifyTests/Verify/blob/16.7.0/src/Verify/Naming/ReflectionFileNameBuilder.cs"/>
    private static string GetParameterText(MethodInfo method, object?[] parameterValues)
    {
        ParameterInfo[] methodParameters = method.GetParameters();
        if (!methodParameters.Any())
            return "";

        if (methodParameters.Length != parameterValues.Length)
            throw new($"The number of passed in parameters ({parameterValues.Length}) must match the number of parameters for the method ({methodParameters.Length}).");

        var dictionary = new Dictionary<string, object?>();
        for (int index = 0; index < methodParameters.Length; index++)
        {
            ParameterInfo parameter = methodParameters[index];
            object? value = parameterValues[index];
            dictionary[parameter.Name!] = value;
        }

        string concat = ConcatParameterDictionary(dictionary);
        return $"_{concat}";
    }

    /// <see href="https://github.com/VerifyTests/Verify/blob/16.7.0/src/Verify/Naming/ParameterBuilder.cs"/>
    private static string ConcatParameterDictionary(Dictionary<string, object?> dictionary)
    {
        var builder = new StringBuilder();
        foreach (KeyValuePair<string, object?> pair in dictionary)
            builder.Append($"{pair.Key}={pair.Value}_");
        builder.Length -= 1;
        return builder.ToString();
    }
}
