using Domain.DTOs;
using Domain.Services;
using Microsoft.Extensions.Caching.Memory;

namespace BLL.Services;

public class CachedCountryService : ICountryService
{
    private readonly ICountryService _countryService;
    private readonly IMemoryCache _memoryCache;

    public CachedCountryService(ICountryService countryService, IMemoryCache memoryCache)
    {
        _countryService = countryService;
        _memoryCache = memoryCache;
    }

    // https://github.com/khellang/Scrutor/issues/208
    public async Task<List<CountryDto>> GetAllAsync(PagingDto paging)
    {
        var cachedValue = await _memoryCache.GetOrCreateAsync(
        $"countries-{paging.PageIndex}-{paging.PageSize}",
        async cacheEntry =>
        {
            cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30);
            return await _countryService.GetAllAsync(paging);
        });

        return cachedValue;
    }

    public async Task<(byte[], string, string)> GetFileAsync()
    {
        return await _countryService.GetFileAsync();
    }

    public async Task<bool> IngestFileAsync(Stream countryFileContent)
    {
        return await _countryService.IngestFileAsync(countryFileContent);
    }

    public async Task LongRunningQueryAsync(CancellationToken cancellationToken)
    {
        await _countryService.LongRunningQueryAsync(cancellationToken);
    }
}
