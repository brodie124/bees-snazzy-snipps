namespace BeesSnazzySnips.Results;

public static class ApiResult
{
    public static ApiResult<T> Success<T>(T? value = default)
    {
        return new ApiResult<T>(
            StatusCode: ResultStatusCode.Ok, 
            Value: value, 
            ErrorMessage: null
        );
    }
    
    public static ApiResult<T> Failure<T>(
        ResultStatusCode statusCode, 
        string? errorMessage = null, 
        T? value = default)
    {
        return new ApiResult<T>(
            StatusCode: statusCode,
            Value: value,
            ErrorMessage: errorMessage
        );
    }
    
    public static ApiResult<T> FromServiceResult<T>(
        ServiceResult<T> serviceResult, 
        string? errorMessage = null)
    {
        return new ApiResult<T>(
            StatusCode: serviceResult.StatusCode,
            Value: serviceResult.Value,
            ErrorMessage: errorMessage ?? serviceResult.ErrorMessage
        );
    }
}

public record ApiResult<T>(
    ResultStatusCode StatusCode,
    T? Value,
    string? ErrorMessage
)
{
    public static implicit operator ApiResult<T>(T? value) => ApiResult.Success(value);

    public bool IsSuccess => StatusCode == ResultStatusCode.Ok;
    public bool IsFailure => !IsSuccess;
    
    public ApiResult<TAlt> ConvertValueNotNull<TAlt>(Func<T, TAlt?> mapFunc)
    {
        return ConvertValue(mapFunc, _ => default);
    }
    
    public ApiResult<TAlt> ConvertValue<TAlt>(Func<T?, TAlt?> mapFunc)
    {
        return ConvertValue(mapFunc, mapFunc);
    }

    public ApiResult<TAlt> ConvertValue<TAlt>(Func<T, TAlt?> whenNotNullFunc, Func<T?, TAlt?> whenNullFunc)
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