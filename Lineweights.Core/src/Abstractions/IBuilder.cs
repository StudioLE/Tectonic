﻿namespace Lineweights.Core.Abstractions;

/// <summary>
/// Build a <typeparamref name="T"/> using a <see href="https://refactoring.guru/design-patterns/builder">builder pattern</see>.
/// </summary>
public interface IBuilder<out T>
{
    /// <inheritdoc cref="IBuilder{T}"/>
    public T Build();
}
