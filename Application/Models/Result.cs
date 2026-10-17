public sealed record Result<T> where T : notnull
{
    public T? Value { get; }

    public bool IsSuccess { get; } = true;
    public bool IsFailure => !IsSuccess;
    public string ErrorMessage { get; } = "";

    private Result(T value)
    {
        Value = value;
        IsSuccess = true;
    }

    private Result(string errorMessage = "")
    {
        ErrorMessage = errorMessage;
        IsSuccess = false;
    }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(string errorMessage = "") => new(errorMessage);
}

public sealed record Result
{
    public bool IsSuccess { get; } = true;
    public bool IsFailure => !IsSuccess;
    public string ErrorMessage { get; } = "";

    private Result()
    {
        IsSuccess = true;
    }

    private Result(string errorMessage)
    {
        IsSuccess = false;
        ErrorMessage = errorMessage;
    }

    public static Result Success() => new();
    public static Result Failure(string errorMessage = "") => new(errorMessage);
}