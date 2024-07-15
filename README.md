A lightweight NuGet library for C# developers designed to simplify error handling while capturing actionable results for 
user interfaces. This library provides two main constructs: ApiResult and ServiceResult, allowing for clear representation 
of success and failure states in your applications.

## Installation
To install the BeesSnazzySnippets.Results library, use the following command in your NuGet Package Manager Console:

```bash
Install-Package BeesSnazzySnippets
```

Alternatively, you can add it to your project file:
```xml
<PackageReference Include="BeesSnazzySnippets.Results" Version="1.0.0" />
```


## Usage

### ServiceResult
The `ServiceResult` class provides a mechanism to encapsulate potential error states, as well as surface the cause via
the pre-defined `ResultStatusCode` enum.

This type is intended to be used internally; for a public-facing type, refer to the [ApiResult](#ApiResult)


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

### ApiResult
This section has not been written yet.


