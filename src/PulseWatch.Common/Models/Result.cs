namespace PulseWatch.Common.Models;

public class Result<T> where T : notnull
{
    private readonly T? _value;

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException(
            "Cannot access Value of a failed result.");

    public bool IsSuccess { get; } = true;
    public bool IsFailure => !IsSuccess;
    public string ErrorMessage { get; } = string.Empty;


    private Result(T value)
    {
        ArgumentNullException.ThrowIfNull(value);

        _value = value;
        IsSuccess = true;
    }

    private Result(string errorMessage)
    {
        ErrorMessage = errorMessage;
        IsSuccess = false;
    }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(string errorMessage) => new(errorMessage);
}

public class Result
{
    public bool IsSuccess { get; } = true;
    public bool IsFailure => !IsSuccess;
    public string ErrorMessage { get; } = string.Empty;

    private Result()
    {
        IsSuccess = true;
    }

    private Result(string errorMessage)
    {
        ErrorMessage = errorMessage;
        IsSuccess = false;
    }

    public static Result Success() => new();
    public static Result Failure(string errorMessage) => new(errorMessage);
}
