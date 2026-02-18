namespace Domain.Common
{
    public enum ErrorType
    {
        None = 0,
        Validation = 1, // 400
        NotFound = 2,   // 404
        Conflict = 3,   // 409
        ServerFault = 4 // 500
    }

    public record Error(ErrorType Type, string Message);

    public class Result
    {
        public bool IsSuccess { get; }
        public Error Error { get; }
        public bool IsFailure => !IsSuccess;

        protected Result(bool isSuccess, Error error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new(true, new Error(ErrorType.None, string.Empty));
        public static Result Failure(ErrorType type, string message) => new(false, new Error(type, message));
    }

    public class Result<T> : Result
    {
        public T Value { get; }
        protected internal Result(T value, bool isSuccess, Error error) : base(isSuccess, error)
        {
            Value = value;
        }

        public static Result<T> Success(T value) => new(value, true, new Error(ErrorType.None, string.Empty));
        public static new Result<T> Failure(ErrorType type, string message) => new(default!, false, new Error(type, message));
    }
}