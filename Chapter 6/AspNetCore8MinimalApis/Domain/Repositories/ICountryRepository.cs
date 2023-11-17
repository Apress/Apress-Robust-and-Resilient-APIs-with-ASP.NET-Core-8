using Domain.DTOs;

namespace Domain.Repositories;

public interface ICountryRepository
{
    Task<CountryDto> RetrieveAsync(int id);
    Task<List<CountryDto>> GetAllAsync();
    Task<int> CreateAsync(CountryDto country);
    Task<int> UpdateAsync(CountryDto country);
    Task<int> UpdateDescriptionAsync(int id, string description);
    Task<int> DeleteAsync(int id);
}
