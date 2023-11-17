using AspNetCore8MinimalApis.Mapping.Interfaces;
using Domain.DTOs;
using Domain.Services;

namespace AspNetCore8MinimalApis.Endpoints;

public static class CountryEndpoints
{
    public static async Task<IResult> GetCountries(int? pageIndex, int? pageSize, ICountryMapper mapper, ICountryService countryService) 
    {
        var paging = new PagingDto
        {
            PageIndex = pageIndex.HasValue ? pageIndex.Value : 1,
            PageSize = pageSize.HasValue ? pageSize.Value : 10
        };
        var countries = await countryService.GetAllAsync(paging);

        return Results.Ok(mapper.Map(countries));
    }
}
