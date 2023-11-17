using AspNetCore8MinimalApis.Models;
using Domain.DTOs;

namespace AspNetCore8MinimalApis.Mapping.Interfaces;

public interface ICountryMapper
{
    public CountryDto Map(Country country);
    Country Map(CountryDto country);
    List<Country> Map(List<CountryDto> countries);
}