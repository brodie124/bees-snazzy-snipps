namespace BeesSnazzySnippets.Results;

public enum ResultStatusCode
{
    Ok = 1,
    GenericFailure = 2,
    BadRequest = 3,
    InvalidCredentials = 4,
    ResourceNotFound = 5,
    ResourceAlreadyExists = 6,
    ResourceExpired = 7,
    ResourceDenied = 8,
    FunctionalityDisabled = 9,
}