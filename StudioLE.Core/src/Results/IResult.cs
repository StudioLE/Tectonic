namespace StudioLE.Core.Results;

public interface IResult
{
    public string[] Warnings { get; }

    public string[] Errors { get; }
}

public interface IResult<out T>
{
    public string[] Warnings { get; }

    public string[] Errors { get; }
}
