namespace StudioLE.Core.Results;

public interface IResult
{
    public string[] Warnings { get; set; }

    public string[] Errors { get; }
}

// ReSharper disable once UnusedTypeParameter
public interface IResult<out T>
{
    public string[] Warnings { get; set; }

    public string[] Errors { get; }
}
