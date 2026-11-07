namespace GpSharp.Core;

using System;

/// <summary>
/// Represents the result of an operation which may or may not have succeeded.
/// </summary>
public class Result<T>
{
    public T? Value { get; }
    public Exception? Error { get; }

    public bool IsSuccess => Error == null;

    private Result(T? value, Exception? error = null)
    {
        Value = value;
        Error = error;
    }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(Exception error) => new(default, error);

    public TValue Match<TValue>(Func<T, TValue> onSuccess, Action<Exception> onFailure)
    {
        if (IsSuccess) return onSuccess(Value!);
        onFailure(Error!);
        throw new InvalidOperationException("Called result.Match with failure.");
    }
}
