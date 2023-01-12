namespace Lineweights.Core.Abstractions;

public interface IFactory<in TSource, out TResult>
{
    public TResult Create(TSource source);
}
