using System.Diagnostics.CodeAnalysis;

namespace BeesSnazzySnippets.Results;

public static class ServiceResult
{
    /// <summary>
    /// Create a successful ServiceResult, with a status code and an optional value.
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns>A ServiceResult with a status code of Ok and the specified value.</returns>
    public static ServiceResult<T> Success<T>(T? value) => new(
        StatusCode: ResultStatusCode.Ok,
        Value: value);
    
    /// <summary>
    /// Create a failure ServiceResult, with an exception, error message, status code and value.
    /// </summary>
    /// <param name="exception"></param>
    /// <param name="errorMessage"></param>
    /// <param name="statusCode"></param>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static ServiceResult<T> Failure<T>(
        Exception? exception = null,
        string? errorMessage = null,
        ResultStatusCode statusCode = ResultStatusCode.GenericFailure,
        T? value = default)
        => new(
            StatusCode: statusCode,
            Value: value,
            InnerException: exception,
            ErrorMessage: errorMessage
        );

    /// <summary>
    /// Create a failure ServiceResult, with an error message, status code and value.
    /// </summary>
    /// <param name="errorMessage"></param>
    /// <param name="statusCode"></param>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static ServiceResult<T> Failure<T>(
        string errorMessage,
        ResultStatusCode statusCode = ResultStatusCode.GenericFailure,
        T? value = default) =>
        Failure(null, errorMessage, statusCode, value);
    
    /// <summary>
    /// Create a failure ServiceResult from an existing ServiceResult but using a new value.
    /// An error message and/or status code can be specified to override the underlying ServiceResult's properties.
    /// </summary>
    /// <param name="serviceResult">ServiceResult to be "copied"</param>
    /// <param name="value">Value of the newly-created ServiceResult</param>
    /// <param name="errorMessage">Override the underlying ServiceResult's message</param>
    /// <param name="statusCode">Override the underlying ServiceResult's status code</param>
    /// <typeparam name="TAlt"></typeparam>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static ServiceResult<T> FailureFromServiceResult<TAlt, T>(
        ServiceResult<TAlt> serviceResult,
        T? value = default,
        string? errorMessage = null,
        ResultStatusCode? statusCode = null) 
        => Failure(serviceResult.InnerException ?? null, errorMessage ?? serviceResult.ErrorMessage, statusCode ?? serviceResult.StatusCode, value);
}

public record ServiceResult<T>(
    ResultStatusCode StatusCode,
    T? Value,
    Exception? InnerException = null,
    string? ErrorMessage = null
)
{
    public static implicit operator ServiceResult<T>(T? value) => ServiceResult.Success(value);

    public bool IsSuccess => StatusCode == ResultStatusCode.Ok;
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Returns a value that is never null.
    /// An exception will be thrown if the value is null.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public T Get()
    {
        return Value ?? throw new ArgumentNullException(nameof(Value));
    }

    /// <summary>
    /// Populates the provided variable with the ServiceResult value
    /// and returns false if the value is null, or true otherwise.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGet([MaybeNullWhen(false)] out T value)
    {
        value = Value ?? default;
        return Value is not null;
    }

    /// <summary>
    /// Creates a new ServiceResult using the specified type and value.
    /// Optionally, the error message and status code can be overriden.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="errorMessage"></param>
    /// <param name="statusCode"></param>
    /// <typeparam name="TAlt"></typeparam>
    /// <returns></returns>
    public ServiceResult<TAlt> PassThroughFail<TAlt>(
        TAlt? value,
        string? errorMessage = null,
        ResultStatusCode? statusCode = null)
    {
        return ServiceResult.FailureFromServiceResult(this, value, errorMessage, statusCode);
    }
}