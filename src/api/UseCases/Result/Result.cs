namespace UseCases.Result;

public class Result<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public ResultReason Reason { get; set; }

    public T Model { get; set; }

    public static Result<T> Fail(string message)
    {
        return new Result<T>
        {
            Success = false,
            Message = message
        };
    }

    public static Result<T> Fail(ResultReason reason)
    {
        return new Result<T>
        {
            Success = false,
            Reason = reason
        };
    }

    public static Result<T> Fail(ResultReason reason, string message)
    {
        return new Result<T>
        {
            Success = false,
            Reason = reason,
            Message = message
        };
    }

    public static Result<T> Succeed(string message = "")
    {
        return new Result<T>
        {
            Success = true,
            Message = message
        };
    }

    public static Result<T> Succeed(T model, string message = "")
    {
        return new Result<T>
        {
            Success = true,
            Model = model,
            Message = message
        };
    }
}

