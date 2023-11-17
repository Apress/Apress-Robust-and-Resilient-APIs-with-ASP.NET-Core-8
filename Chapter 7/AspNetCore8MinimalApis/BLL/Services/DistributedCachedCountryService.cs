using Domain.DTOs;
using Domain.Services;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace BLL.Services;

internal class DistributedCachedCountryService : ICountryService
{
    private readonly ICountryService _countryService;
    private readonly IDistributedCache _distributedCache;

    public DistributedCachedCountryService(ICountryService countryService, IDistributedCache distributedCache)
    {
        _countryService = countryService;
        _distributedCache = distributedCache;
    }

    public async Task<List<CountryDto>> GetAllAsync(PagingDto paging)
    {
        var key = $"countries-{paging.PageIndex}-{paging.PageSize}";

        var cachedValue = await _distributedCache.GetStringAsync(key);
        if (cachedValue == null)
        {
            var data = await _countryService.GetAllAsync(paging);
            await _distributedCache.SetStringAsync(key, JsonSerializer.Serialize(data), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
            });
            return data;
        }
        return JsonSerializer.Deserialize<List<CountryDto>>(cachedValue);
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
