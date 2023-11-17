using AspNetCore8MinimalApis.Models;
using FluentValidation;
using FluentValidation.Results;
using System.Text.RegularExpressions;

namespace AspNetCore8MinimalApis.Validators;

public class CountryPatchValidator : AbstractValidator<CountryPatch>
{
    public CountryPatchValidator()
    {
        RuleFor(x => x.Description).NotEmpty().WithMessage("{ParameterName} cannot be empty")
                            .Custom((name, context) =>
                            {
                                Regex rg = new Regex("<.*?>"); // try to match HTML tags
                                if (rg.Matches(name).Count > 0)
                                {
                                    // Raises an error
                                    context.AddFailure(new ValidationFailure("Description", "The description has invalid content"));
                                }
                            });

    }
}
