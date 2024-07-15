namespace BeesSnazzySnips.Results;

public record ApiResult<T>(
    ResultStatusCode StatusCode,
    T? Value,
    string? ErrorMessage
)
{
    public static implicit operator ApiResult<T>(T? value) => Success(value);

    public bool IsSuccess => StatusCode == ResultStatusCode.Ok;

    public static ApiResult<T> Success(T? value = default)
    {
        return new ApiResult<T>(
            StatusCode: ResultStatusCode.Ok, 
            Value: value, 
            ErrorMessage: null
        );
    }

    public static ApiResult<T> Failure(ResultStatusCode statusCode, string? errorMessage = null, T? value = default)
    {
        return new ApiResult<T>(
            StatusCode: statusCode,
            Value: value,
            ErrorMessage: errorMessage
        );
    }


    public static ApiResult<T> FromServiceResult(ServiceResult<T> serviceResult)
    {
        return new ApiResult<T>(
            StatusCode: serviceResult.StatusCode,
            Value: serviceResult.Value,
            ErrorMessage: serviceResult.ErrorMessage
        );
    }

    public static ApiResult<T> FromServiceResult(ServiceResult<T> serviceResult, string errorMessage)
    {
        return new ApiResult<T>(
            StatusCode: serviceResult.StatusCode,
            Value: serviceResult.Value,
            ErrorMessage: errorMessage
        );
    }
     
    public static ApiResult<T> FromServiceResult(ServiceResult<T> serviceResult, Func<ServiceResult<T>, string> errorMessageMapFunc)
    {
        var errorMessage = errorMessageMapFunc(serviceResult);
        return new ApiResult<T>(
            StatusCode: serviceResult.StatusCode,
            Value: serviceResult.Value,
            ErrorMessage: errorMessage
        );
    }

    public ApiResult<TAlt> Map<TAlt>(Func<T?, TAlt?> mapFunc)
    {
        return Map(mapFunc, mapFunc);
    }
    
    public ApiResult<TAlt> MapWhenNotNull<TAlt>(Func<T, TAlt?> mapFunc)
    {
        return Map(mapFunc, _ => default);
    }

    public ApiResult<TAlt> Map<TAlt>(Func<T, TAlt?> whenNotNullFunc, Func<T?, TAlt?> whenNullFunc)
    {
        var mappedValue = Value is not null
            ? whenNotNullFunc(Value)
            : whenNullFunc(Value);

        return new ApiResult<TAlt>(
            StatusCode: StatusCode,
            Value: mappedValue,
            ErrorMessage: ErrorMessage
        );
    }
}