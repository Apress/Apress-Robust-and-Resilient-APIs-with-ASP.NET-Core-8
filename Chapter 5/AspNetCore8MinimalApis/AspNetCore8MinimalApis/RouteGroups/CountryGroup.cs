using AspNetCore8MinimalApis.Endpoints;

namespace AspNetCore8MinimalApis.RouteGroups;

public static class CountryGroup
{
    public static void AddCountryEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/countries");

        group.MapPost("/", CountryEndpoints.PostCountry);
        // Other endpoints in the same group
    }
}
