namespace PixelSpot.API.DTOs;

public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public object Errors { get; set; }

    public ApiResponse(string message = null, object errors = null)
    {
        Success = errors == null;
        Message = message ?? (Success ? "Operation completed successfully" : "One or more errors occurred");
        Errors = errors;
    }
}

public class ApiResponse<T> : ApiResponse
{
    public T Data { get; set; }

    public ApiResponse(T data, string message = null) : base(message)
    {
        Data = data;
        Success = true;
    }
}
