using FluentValidation;

namespace AspNetCore8MinimalApis.EndpointFilters;

public class InputValidatorFilter<T> : IEndpointFilter
{
    private readonly IValidator<T> _validator;
    public InputValidatorFilter(IValidator<T> validator)
    {
        _validator = validator;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        T? inputData = context.GetArgument<T>(0);

        if (inputData is not null)
        {
            var validationResult = await _validator.ValidateAsync(inputData);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }
        }

        return await next.Invoke(context);
    }
}
