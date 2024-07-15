namespace BeesSnazzySnippets.Results;

public static class ApiResult
{
    /// <summary>
    /// Create a successful ApiResult with a value.
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns>An ApiResult with a status code of Ok and the specified value.</returns>
    public static ApiResult<T> Success<T>(T? value = default)
    {
        return new ApiResult<T>(
            StatusCode: ResultStatusCode.Ok, 
            Value: value, 
            ErrorMessage: null
        );
    }
    
    /// <summary>
    /// Create a failure ApiResult, with a status code, error message and value. 
    /// </summary>
    /// <param name="statusCode"></param>
    /// <param name="errorMessage"></param>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
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
    
    /// <summary>
    /// Create an ApiResult using an existing ServiceResult.
    /// Optionally, the error message can be overriden by specifying an error message.
    ///
    /// The returned ApiResult may not be of the required type; refer to <see cref="ApiResult{T}.ConvertValue{TAlt}(System.Func{T?,TAlt?})" />.
    /// </summary>
    /// <param name="serviceResult"></param>
    /// <param name="errorMessage"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
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
    
    /// <summary>
    /// Converts the current value using the specified mapping function if the value is not null,
    /// and returns a new ApiResult of the specified type. If the value is null, the result will
    /// be the default value for the specified type.
    /// </summary>
    /// <typeparam name="TAlt">The type of the result after conversion.</typeparam>
    /// <param name="mapFunc">The function to map the current value to the new type if it is not null.</param>
    /// <returns>An ApiResult of the new type after applying the mapping function, or the default value if the current value is null.</returns>
    public ApiResult<TAlt> ConvertValueNotNull<TAlt>(Func<T, TAlt?> mapFunc)
    {
        return ConvertValue(mapFunc, _ => default);
    }
    
    /// <summary>
    /// Converts the current value using the specified mapping function, 
    /// regardless of whether the value is null or not, and returns a new ApiResult of the specified type.
    /// </summary>
    /// <typeparam name="TAlt">The type of the result after conversion.</typeparam>
    /// <param name="mapFunc">The function to map the current value to the new type.</param>
    /// <returns>An ApiResult of the new type after applying the mapping function.</returns>
    public ApiResult<TAlt> ConvertValue<TAlt>(Func<T?, TAlt?> mapFunc)
    {
        return ConvertValue(mapFunc, mapFunc);
    }

    /// <summary>
    /// Converts the current value using the specified functions based on whether the value is null or not,
    /// and returns a new ApiResult of the specified type.
    /// </summary>
    /// <typeparam name="TAlt">The type of the result after conversion.</typeparam>
    /// <param name="whenNotNullFunc">The function to map the current value to the new type if it is not null.</param>
    /// <param name="whenNullFunc">The function to map the current value to the new type if it is null.</param>
    /// <returns>An ApiResult of the new type after applying the mapping function.</returns>
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