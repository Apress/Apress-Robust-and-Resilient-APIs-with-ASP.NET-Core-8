using AspNetCore8MinimalApis.Mapping.Interfaces;
using AspNetCore8MinimalApis.Models;
using Domain.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore8MinimalApis.Endpoints;

public static class CountryEndpoints
{
    public static IResult PostCountry([FromBody] Country country, IValidator<Country> validator, ICountryMapper mapper, ICountryService countryService)
    {
        var validationResult = validator.Validate(country);

        if (validationResult.IsValid)
        {
            var countryDto = mapper.Map(country);
            return Results.CreatedAtRoute("countryById", new { Id = countryService.CreateOrUpdate(countryDto) });
        }
        return Results.ValidationProblem(validationResult.ToDictionary());
    }
}
