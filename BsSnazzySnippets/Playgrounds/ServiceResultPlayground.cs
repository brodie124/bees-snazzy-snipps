using BsSnazzySnippets.Results;

namespace BsSnazzySnippets.Playgrounds;

internal sealed record ComplexType(string Name, int Age);

internal sealed record OtherComplexType(string Colour);

internal sealed class ServiceResultPlayground
{
    public ServiceResult<ComplexType> ImplicitTypeInference()
    {
        var response = new ComplexType("Bob", 24);
        return ServiceResult.Success(response);
    }

    public ServiceResult<ComplexType> TypeRemappingOnFailure()
    {
        var createResult = CreateOtherType();
        if (createResult.IsFailure)
            return createResult.PassThroughFail<ComplexType>(null);

        return ServiceResult.Success<ComplexType>(null);
    }


    public ServiceResult<OtherComplexType> CreateOtherType()
    {
        return new OtherComplexType("Red");
    }
}