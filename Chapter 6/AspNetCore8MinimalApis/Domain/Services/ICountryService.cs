using Domain.DTOs;

namespace Domain.Services;

public interface ICountryService
{
    Task<CountryDto> RetrieveAsync(int id);
    Task<List<CountryDto>> GetAllAsync();
    Task<int> CreateOrUpdateAsync(CountryDto country);
    Task<bool> UpdateDescriptionAsync(int id, string description);
    Task<bool> DeleteAsync(int id);
}
