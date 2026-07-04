using SharedKernel;

namespace Web.Api.Infrastructure;

public sealed class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public ApiError? Error { get; set; }

    public static ApiResponse<T> CreateSuccess(T data) => new() { Success = true, Data = data, Error = null };
    public static ApiResponse<T> CreateFailure(string code, string message) => new() { Success = false, Data = default, Error = new ApiError(code, message) };
}

public record ApiError(string Code, string Message);

public static class ApiResponseExtensions
{
    public static IResult ToApiResponse<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            var response = ApiResponse<T>.CreateSuccess(result.Value);
            return Results.Ok(response);
        }

        int statusCode = GetStatusCode(result.Error.Type);
        var errorResponse = ApiResponse<T>.CreateFailure(result.Error.Code, result.Error.Description);
        return Results.Json(errorResponse, statusCode: statusCode);
    }

    public static IResult ToApiResponse(this Result result)
    {
        if (result.IsSuccess)
        {
            var response = ApiResponse<object>.CreateSuccess(new { });
            return Results.Ok(response);
        }

        int statusCode = GetStatusCode(result.Error.Type);
        var errorResponse = ApiResponse<object>.CreateFailure(result.Error.Code, result.Error.Description);
        return Results.Json(errorResponse, statusCode: statusCode);
    }

    private static int GetStatusCode(ErrorType errorType) =>
        errorType switch
        {
            ErrorType.Validation or ErrorType.Problem => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };
}
