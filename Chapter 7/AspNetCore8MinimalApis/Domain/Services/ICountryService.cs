using Domain.DTOs;
using System.Reflection;

namespace Domain.Services;

public interface ICountryService
{
    Task<List<CountryDto>> GetAllAsync(PagingDto paging);
    Task LongRunningQueryAsync(CancellationToken cancellationToken);
    Task<bool> IngestFileAsync(Stream countryFileContent);
    Task<(byte[], string, string)> GetFileAsync();
}
