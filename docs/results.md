# Results
This library/package provides an opinionated implementation of the result pattern. It is opinionated by design, with the 
primary goal being to support rich error logging and accurate user feedback, while keeping the amount of boilerplate to a minimum.

Two types of result are provided:
* `ServiceResult` - used to bubble errors up from the underlying service(s) to the entry-point.
* `ApiResult` - used to inform consumer/client about the state of the requested operation. 

Both of these types contain a reference to the `ResultStatusCode` enum. In my opinion, the use of an enum here instead of 
relying on HTTP status codes is more resilient to change and more clearly sets out the developers intentions.

Use of this enum is technically optional; however, I would recommend trying it out - you may find it easier to inform 
your user on what went wrong.

## ServiceResult
The `ServiceResult` implementation provides a mechanism to capture and handle error states of internal/non-public facing
services without losing the context of the underlying error.

> **Why is this type for use in internal services?**
> 
> The reason this type is intended to be used for internal services is, ultimately, arbitrary. The `ServiceResult` type 
> has a property to store the exception that caused the error state - if you were to serialize this object and send it back to
> the caller, they will be able to see the exception and the associated stack trace.
>
> It is my personal preference to not expose internal workings to the public domain.


### Basic Usage
Below is a simple example of the `ServiceResult` where there are three possible return paths:
the user could not be created because the name is taken, the user could not be created because of an unknown error,
or the user is created successfully.
```csharp
public async Task<ServiceResult<User>> CreateUserAsync(string username, string password)
{
    try
    {
        var isUsernameTaken = await _userService.IsUsernameTakenAsync(username);
        if(isUsernameTaken)
            return ServiceResult.Failure<User>("Username taken.", ResultStatusCode.ResourceAlreadyExists);

        var user = await _userService.CreateUserAsync(username, password);
        
        return user; // Implicit casting to a successful ServiceResult<User>
                     // Equivalent to: return ServiceResult.Success(user);
    }
    catch (Exception ex)
    {
        return ServiceResult.Failure<User>(ex, "Could not create user");
    }
}
```

The caller is then able to use this result to present a useful error to the user/client. For example, in an API controller
it may look as follows:
```csharp
public async Task<IActionResult> CreateUser(string username, string password)
{
    var createUserResult = await CreateUserAsync(username, password);
    if (createUserResult.TryGet(out var user))
        return Ok(user);

    if (createUserResult.StatusCode == ResultStatusCode.ResourceAlreadyExists)
        return Conflict();

    return Problem();
}
```

### Advanced usage
In more complex scenarios it is likely that a result will need to be bubbled up through multiple layers.
Achieving this when the return type of each layer is the same; however, what can we do when this is not the case?

We have a couple of choices, depending on the specific circumstances. 

Firstly, if the result is known to be in an error state, we can call the `PassThroughFail` method. 
This will create a new ServiceResult of a different type. The value must be changed, however, the result code, 
error message and inner exception will be copied across.

```csharp
public ServiceResult<OtherComplexType> BubbleUpErrors()
{
    ServiceResult<ComplexType> createResult = CreateComplexType();
    if (createResult.IsFailure || !createResult.TryGet(out var balanceRecord))
        return createResult.PassThroughFail<OtherComplexType>(); // This will create a new ServiceResult<OtherComplexType>
                                                                 // The value will be set to default value for the type provided.
                                                                 // In this case, null.
                                                                     
    var otherComplexType = balanceRecord.CreateOtherComplexType();
    return otherComplexType;
}
```

Secondly, it is possible to convert/map the ServiceResult<T> into a different type using a lambda/anonymous function.
The lambda is given the entire original ServiceResult to convert as needed. A simple example is provided below where we can 
hide the "invalid username" / "invalid password message".

> TODO: provide a code sample


## ApiResult
> This section is not yet complete.


The ApiResult implementation provides a mechanism to capture and handle the result of public-facing operations,
making it easier to communicate success or failure states to the client without exposing inner workings of your application
to the public domain.

### Basic Usage
Similar to the `ServiceResult`, the `ApiResult` provides some basic static methods for creating results, with key difference
being that `ApiResult` **does not** store the exception.

To provide an example of the ApiResult usage, we will build on the earlier examples used for the ServiceResult class.

Let's assume that we're building an API controller to expose the `CreateUserAsync` function we saw earlier. 
In its simplest form, where the return value of the controller is the same as the service, we can simple convert the
`ServiceResult<T>` into an `ApiResult<T>`
```csharp
public async Task<ApiResult<User>> ControllerCreateUser(string username, string password)
{
    ServiceResult<User> createResult = await CreateUserAsync(username, password);
    return ApiResult.FromServiceResult<User>(createResult);
}
```

Below is a simple example of ApiResult where there are three possible return paths: the user could not be created because the name is taken, the user could not be created because of an unknown error, or the user is created successfully.