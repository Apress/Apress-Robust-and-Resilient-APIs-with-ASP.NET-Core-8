using Domain.DTOs;
using Domain.Services;
using System.Reflection;

namespace BLL.Services;

public class CountryService : ICountryService
{
    private static List<CountryDto> _countries = new List<CountryDto>
    {
        new CountryDto
        {
            Id = 1,
            Name = "Canada",
            Description = "Maple Leaf country",
            FlagUri = "https://anthonygiretti.blob.core.windows.net/countryflags/ca.png"
        },
        new CountryDto
        {
            Id = 2,
            Name = "USA",
            Description = "Federal republic of 50 states",
            FlagUri = "https://anthonygiretti.blob.core.windows.net/countryflags/us.png"
        }
    };

    bool ICountryService.Delete(int id)
    {
        return true;
    }

    List<CountryDto> ICountryService.GetAll()
    {
        return new List<CountryDto>
        {
            new CountryDto
            {
                Id = 1,
                Name = "Canada",
                Description = "Maple Leaf country",
                FlagUri = "https://anthonygiretti.blob.core.windows.net/countryflags/ca.png"
            },
            new CountryDto
            {
                Id = 2,
                Name = "USA",
                Description = "Federal republic of 50 states",
                FlagUri = "https://anthonygiretti.blob.core.windows.net/countryflags/us.png"
            }
        };
    }

    CountryDto ICountryService.Retrieve(int id)
    {
        return _countries.FirstOrDefault(x => x.Id == id);
    }

    int ICountryService.CreateOrUpdate(CountryDto country)
    {
        if (country?.Id is null)
            return 1;

        var countryFound = _countries.FirstOrDefault(x => x.Id == country.Id);

        if (countryFound is null)
            return 1;

        return countryFound.Id.Value;
    }

    bool ICountryService.UpdateDescription(int id, string description)
    {
        var country = _countries.FirstOrDefault(x => x.Id == id);
        country.Description = description;

        return true;
    }

    (byte[], string, string) ICountryService.GetFile()
    {
        string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"countries.csv");
        return (File.ReadAllBytes(path), "text/csv", "countries.csv");
    }
}
