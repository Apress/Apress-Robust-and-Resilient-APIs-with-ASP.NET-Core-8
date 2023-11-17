using AspNetCore8MinimalApis.Channels;
using AspNetCore8MinimalApis.Mapping;
using AspNetCore8MinimalApis.Mapping.Interfaces;
using AspNetCore8MinimalApis.Models;
using AspNetCore8MinimalApis.Resiliency.Http;
using BLL.Services;
using Domain.Channels;
using Domain.DTOs;
using Domain.Repositories;
using Domain.Services;
using FluentValidation;
using Infrastructure.BackgroundTasks;
using Infrastructure.SQL.Database;
using Infrastructure.SQL.Repositories;
using Microsoft.EntityFrameworkCore;
using Refit;
using Microsoft.Extensions.Caching.StackExchangeRedis;

var builder = WebApplication.CreateBuilder(args);

var dbConnection = builder.Configuration.GetConnectionString("DemoDb");
builder.Services.AddDbContextPool<DemoContext>(options => 
    options.UseSqlServer(dbConnection));

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddScoped<ICountryMapper, CountryMapper>();
builder.Services.AddScoped<ICountryService, CountryService>(); //https://github.com/khellang/Scrutor/issues/208
//builder.Services.Decorate<ICountryService, CachedCountryService>();
//builder.Services.Decorate<ICountryService, DistributedCachedCountryService>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddRefitClient<IMediaRepository>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://anthonygiretti.blob.core.windows.net"))
                .AddFaultHandlingPolicy();

builder.Services.AddSingleton<ICountryFileIntegrationChannel, CountryFileIntegrationChannel>();
builder.Services.AddHostedService<CountryFileIntegrationBackgroudService>();

// Allow up to 60 seconds to complete any in-progress results processing.
builder.Services.PostConfigure<HostOptions>(option =>
{
    option.ShutdownTimeout = TimeSpan.FromSeconds(60);
});

builder.Services.AddOutputCache(options =>
{
    //options.AddBasePolicy(builder =>
    //    builder.Expire(TimeSpan.FromSeconds(30)));
    options.AddPolicy("5minutes", builder =>
        builder.Expire(TimeSpan.FromSeconds(300))
               .SetVaryByQuery("*")
    );
});

builder.Services.AddMemoryCache();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnectionString");
    options.InstanceName = "Demo";
});

var app = builder.Build();

app.UseOutputCache();

app.MapGet("/countries", async ([AsParameters] Paging paging, ICountryMapper mapper, ICountryService countryService) => {
    async IAsyncEnumerable<Country> StreamCountriesAsync()
    {
        var countries = await countryService.GetAllAsync(new PagingDto
        {
            PageIndex = paging.PageIndex.HasValue ? paging.PageIndex.Value : 1,
            PageSize = paging.PageSize.HasValue ? paging.PageSize.Value : 10
        });
        var mappedCountries = mapper.Map(countries);
        foreach (var country in mappedCountries)
        {
            yield return country;
        }
    }
    return StreamCountriesAsync();
});

app.MapGet("/cachedcountries", async ([AsParameters] Paging paging, ICountryMapper mapper, ICountryService countryService) => {
    var countries = await countryService.GetAllAsync(new PagingDto
    {
        PageIndex = paging.PageIndex.HasValue ? paging.PageIndex.Value : 1,
        PageSize = paging.PageSize.HasValue ? paging.PageSize.Value : 10
    });
    return Results.Ok(mapper.Map(countries));
}).CacheOutput("5minutes");

app.MapGet("/cachedinmemorycountries", async (ICountryMapper mapper, ICountryService countryService) => {
    var countries = await countryService.GetAllAsync(new PagingDto
    {
        PageIndex = 1,
        PageSize = 10
    });
    return Results.Ok(mapper.Map(countries));
});

app.MapPost("/countries/upload", async (IFormFile file, ICountryFileIntegrationChannel channel, CancellationToken cancellationToken) =>
{
    if (await channel.SubmitAsync(file.OpenReadStream(), cancellationToken))
        Results.Accepted();
    Results.StatusCode(StatusCodes.Status500InternalServerError);
}).DisableAntiforgery();

app.MapGet("/cancellable", async (ICountryService countryService, CancellationToken cancellationToken) =>
{
    await countryService.LongRunningQueryAsync(cancellationToken);
    return Results.Ok();
});


app.Run();