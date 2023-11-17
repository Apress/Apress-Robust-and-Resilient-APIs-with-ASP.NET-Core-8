using System.Reflection;
using System.Text.Json;

namespace AspNetCore8MinimalApis.Models;

public class Country
{
    /// <summary>
    /// The country Id
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// The country name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The country description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// The country flag URI
    /// </summary>
    public string FlagUri { get; set; }

    public static ValueTask<Country> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        var countryFromValue = context.Request.Form["Country"];
        var result = JsonSerializer.Deserialize<Country>(countryFromValue);

        return ValueTask.FromResult(result);
    }
}