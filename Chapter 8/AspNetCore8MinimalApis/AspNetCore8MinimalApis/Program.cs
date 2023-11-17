using AspNetCore8MinimalApis.ExceptionHandlers;
using AspNetCore8MinimalApis.Mapping;
using AspNetCore8MinimalApis.Mapping.Interfaces;
using BLL.Services; 
using Domain.DTOs;
using Domain.Repositories;
using Domain.Services;
using Infrastructure.SQL.Database;
using Infrastructure.SQL.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;
using AspNetCore8MinimalApis.Healthchecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using AspNetCore8MinimalApis;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

var dbConnection1 = builder.Configuration.GetValue<string>("ConnectionStrings:DemoDb1");
var dbConnection2 = builder.Configuration.GetValue<string>("ConnectionStrings:DemoDb2");

builder.Services.AddDbContextPool<DemoContext>(options => options.UseSqlServer(dbConnection1));

builder.Services.AddHealthChecks()
                 .AddSqlServer(name: "SQL1", connectionString: dbConnection1, tags: new[] { "live" })
                 .AddSqlServer(name: "SQL2", connectionString: dbConnection2, tags: new[] { "live" })
                 .AddCheck<ReadyHealthCheck>("Readiness check", tags: new[] { "ready" });

builder.Services.AddExceptionHandler<DefaultExceptionHandler>();

builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<ICountryMapper, CountryMapper>();
builder.Services.AddScoped<ICountryService, CountryService>();

builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();

app.UseExceptionHandler(opt => { });

app.MapHealthChecks("/ready", new HealthCheckOptions
{
    Predicate = healthCheck => healthCheck.Tags.Contains("ready")
});

app.MapHealthChecks("/live", new HealthCheckOptions
{
    Predicate = healthCheck => healthCheck.Tags.Contains("live")
});

app.MapGet("/logging", (ILogger<Program> logger) => {

    logger.LogInformation("/logging endpoint has been invoked.");
    return Results.Ok();
});

app.MapGet("/countries", async (int? pageIndex, int? pageSize, ICountryMapper mapper, ICountryService countryService, ILogger<Program> logger) => {

    var paging = new PagingDto
    {
        PageIndex = pageIndex.HasValue ? pageIndex.Value : 1,
        PageSize = pageSize.HasValue ? pageSize.Value : 10
    };
    var countries = await countryService.GetAllAsync(paging);

    using (logger.BeginScope("Getting countries with page index {pageIndex} and page size {pageSize}", paging.PageIndex, paging.PageSize))
    {
        logger.LogInformation("Received {count} countries from the query", countries.Count);
        return Results.Ok(mapper.Map(countries));
    }
});

// simulate back groudn task
Task.Run(() =>  { Thread.Sleep(10000); Ready.IsReady = true; });

app.Run();