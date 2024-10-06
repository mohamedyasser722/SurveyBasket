namespace SurveyBasket.Api.Abstraction;

public class Result
{
    public bool IsSuccess { get; set; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; } = default!;

    public Result(bool isSuccess, Error error)
    {
        if ((isSuccess && error != Error.None) || (!isSuccess && error == Error.None))
            throw new InvalidOperationException();

        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);
    public static Result<T> Success<T>(T value) => new(true, value, Error.None);
    public static Result<T> Failure<T>(Error error) => new(false, default!, error);
}

public class Result<T> : Result
{
    private T? _Value { get; }

    public Result(bool isSuccess, T? value, Error error) : base(isSuccess, error)
    {
        _Value = value;
    }

    public T Value => IsSuccess ? _Value! : throw new InvalidOperationException("Failure results can't have value");

}
