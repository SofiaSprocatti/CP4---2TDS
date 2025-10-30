using System.Diagnostics;

namespace UserManagement.Api.Models;

public class ApiResponse<T>
{
    public bool Error { get; set; }
    public string? ErrorMessage { get; set; }
    public T? DataResult { get; set; }
    public string? StackTrace { get; set; }
    public long TimeElapsed { get; set; }

    public static ApiResponse<T> Success(T data, long timeElapsed = 0)
    {
        return new ApiResponse<T>
        {
            Error = false,
            ErrorMessage = null,
            DataResult = data,
            StackTrace = null,
            TimeElapsed = timeElapsed
        };
    }

    public static ApiResponse<T> Failure(string errorMessage, string? stackTrace = null, long timeElapsed = 0)
    {
        return new ApiResponse<T>
        {
            Error = true,
            ErrorMessage = errorMessage,
            DataResult = default,
            StackTrace = stackTrace,
            TimeElapsed = timeElapsed
        };
    }
}

public class ApiResponse : ApiResponse<object>
{
    public static ApiResponse Success(long timeElapsed = 0)
    {
        return new ApiResponse
        {
            Error = false,
            ErrorMessage = null,
            DataResult = null,
            StackTrace = null,
            TimeElapsed = timeElapsed
        };
    }

    public new static ApiResponse Failure(string errorMessage, string? stackTrace = null, long timeElapsed = 0)
    {
        return new ApiResponse
        {
            Error = true,
            ErrorMessage = errorMessage,
            DataResult = null,
            StackTrace = stackTrace,
            TimeElapsed = timeElapsed
        };
    }
}