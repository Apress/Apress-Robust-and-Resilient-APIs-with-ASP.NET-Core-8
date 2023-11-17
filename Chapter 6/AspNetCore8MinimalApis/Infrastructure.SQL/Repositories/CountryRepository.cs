using Domain.DTOs;
using Domain.Repositories;
using Infrastructure.SQL.Database;
using Infrastructure.SQL.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.SQL.Repositories;

public class CountryRepository : ICountryRepository
{
    private readonly DemoContext _demoContext;

    public CountryRepository(DemoContext demoContext)
    {
        _demoContext = demoContext;
    }

    public async Task<int> CreateAsync(CountryDto country)
    {
        var countryEntity = new CountryEntity
        {
            Name = country.Name,
            Description = country.Description,
            FlagUri = country.FlagUri
        };

        await _demoContext.AddAsync(countryEntity);
                     await _demoContext.SaveChangesAsync();

        return countryEntity.Id;
    }

    public async Task<int> UpdateAsync(CountryDto country)
    {
        var countryEntity = new CountryEntity
        {
            Id = country.Id,
            Name = country.Name,
            Description = country.Description,
            FlagUri = country.FlagUri
        };

        return await _demoContext.Countries
                                 .Where(x => x.Id == countryEntity.Id)
                                 .ExecuteUpdateAsync(s => s.SetProperty(p => p.Description, countryEntity.Description)
                                                           .SetProperty(p => p.FlagUri, countryEntity.FlagUri)
                                                           .SetProperty(p => p.Name, countryEntity.Name));
    }

    public async Task<int> DeleteAsync(int id)
    {
        return await _demoContext.Countries
                                 .Where(x => x.Id == id)
                                 .ExecuteDeleteAsync();
    }

    public async Task<List<CountryDto>> GetAllAsync()
    {
        var result = await Task.Run(() => 1 + 1);
        return await _demoContext.Countries
                                 .AsNoTracking()
                                 .Select(x => new CountryDto
                                 {
                                     Id = x.Id,
                                     Name = x.Name,
                                     Description = x.Description,
                                     FlagUri = x.FlagUri
                                 })
                                 .ToListAsync();
    }

    public async Task<CountryDto> RetrieveAsync(int id)
    {
        return await _demoContext.Countries
                         .AsNoTracking()
                         .Where(x => x.Id == id)
                         .Select(x => new CountryDto
                         {
                             Id = x.Id,
                             Name = x.Name,
                             Description = x.Description,
                             FlagUri = x.FlagUri
                         })
                         .FirstOrDefaultAsync();
    }

    public async Task<int> UpdateDescriptionAsync(int id, string description)
    {
        return await _demoContext.Countries
                                 .Where(x => x.Id == id)
                                 .ExecuteUpdateAsync(s => s.SetProperty(p => p.Description, description));
    }
}
