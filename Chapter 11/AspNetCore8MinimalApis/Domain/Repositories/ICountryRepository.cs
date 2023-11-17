using Domain.DTOs;

namespace Domain.Repositories;

public interface ICountryRepository
{
    Task<List<CountryDto>> GetAllAsync(PagingDto paging);
    Task LongRunningQueryAsync(CancellationToken cancellationToken);
}