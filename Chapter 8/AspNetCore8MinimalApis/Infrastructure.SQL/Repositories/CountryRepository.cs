using Domain.DTOs;
using Domain.Repositories;
using Infrastructure.SQL.Database;
using Infrastructure.SQL.Database.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.SQL.Repositories;

public class CountryRepository : ICountryRepository
{
    private readonly DemoContext _demoContext;

    public CountryRepository(DemoContext demoContext)
    {
        _demoContext = demoContext;
    }

    public async Task<List<CountryDto>> GetAllAsync(PagingDto paging)
    {
        return (await _demoContext.Countries
                                 .AsNoTracking()
                                 .Select(x => new CountryDto
                                 {
                                     Id = x.Id,
                                     Name = x.Name,
                                     Description = x.Description,
                                     FlagUri = x.FlagUri
                                 })
                                 .Skip((paging.PageIndex - 1) * paging.PageSize)
                                 .Take(paging.PageSize)
                                 .ToListAsync());
    }

    public async Task LongRunningQueryAsync(CancellationToken cancellationToken)
    {
        await _demoContext.Database.ExecuteSqlRawAsync("WAITFOR DELAY '00:00:10'", cancellationToken: cancellationToken);
    }
}
