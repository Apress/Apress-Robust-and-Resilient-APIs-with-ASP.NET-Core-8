using Domain.DTOs;
using Domain.Repositories;
using Domain.Services;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;

namespace BLL.Services;

public class CountryService : ICountryService
{
    private readonly ICountryRepository _countryRepository;

    public CountryService(ICountryRepository countryRepository, IMemoryCache memoryCache)
    {
        _countryRepository = countryRepository;
    }
 
    public async Task<List<CountryDto>> GetAllAsync(PagingDto paging)
    {
        return await _countryRepository.GetAllAsync(paging);
    }

    public async Task LongRunningQueryAsync(CancellationToken cancellationToken)
    {
        await _countryRepository.LongRunningQueryAsync(cancellationToken);
    }

    public async Task<bool> IngestFileAsync(Stream countryFileContent)
    {
        throw new NotImplementedException();
    }

    public async Task<(byte[], string, string)> GetFileAsync()
    {
        string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"beach.png");
        return (await File.ReadAllBytesAsync(path), "image/png", "beach.png");
    }
}
