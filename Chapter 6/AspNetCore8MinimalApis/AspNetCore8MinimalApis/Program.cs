using AspNetCore8MinimalApis.EndpointFilters;
using AspNetCore8MinimalApis.ExceptionHandlers;
using AspNetCore8MinimalApis.Mapping;
using AspNetCore8MinimalApis.Mapping.Interfaces;
using AspNetCore8MinimalApis.Models;
using AspNetCore8MinimalApis.Resiliency.Http;
using BLL.Services;
using Domain.DTOs;
using Domain.Repositories;
using Domain.Services;
using FluentValidation;
using Infrastructure.SQL.Database;
using Infrastructure.SQL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Refit;
using System;

var builder = WebApplication.CreateBuilder(args);

var dbConnection = builder.Configuration.GetConnectionString("DemoDb");
builder.Services.AddDbContextPool<DemoContext>(options => 
    options.UseSqlServer(dbConnection, 
                         sqlServerOptionsAction: sqlOptions =>
                         {
                             sqlOptions.EnableRetryOnFailure(
                             maxRetryCount: 3);
                         }));

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddScoped<ICountryMapper, CountryMapper>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddRefitClient<IMediaRepository>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://anthonygiretti.blob.core.windows.net"))
                .AddFaultHandlingPolicy();
builder.Services.AddExceptionHandler<TimeOutExceptionHandler>();
builder.Services.AddExceptionHandler<DefaultExceptionHandler>();

var app = builder.Build();

app.MapPost("/countries", async ([FromBody] Country country, ICountryMapper mapper, ICountryService countryService) => {
    var countryDto = mapper.Map(country);
    var countryId = await countryService.CreateOrUpdateAsync(countryDto);
    if (countryId <= 0)
        return Results.StatusCode(StatusCodes.Status500InternalServerError);
    return Results.CreatedAtRoute("countryById", new { Id = countryId });
}).AddEndpointFilter<InputValidatorFilter<Country>>();

app.MapGet("/countries/{id}", async (int id, ICountryMapper mapper, ICountryService countryService) => {
    var country = await countryService.RetrieveAsync(id);

    if (country is null)
        return Results.NotFound();

    return Results.Ok(mapper.Map(country));
}).WithName("countryById");

app.MapGet("/countries", async (ICountryMapper mapper, ICountryService countryService) => {
    var countries = await countryService.GetAllAsync();
    return Results.Ok(mapper.Map(countries));
});

app.MapDelete("/countries/{id}", async (int id, ICountryService countryService) => {
    if (await countryService.DeleteAsync(id))
        return Results.NoContent();

    return Results.NotFound();
});

app.MapPut("/countries", async ([FromBody] Country country, ICountryMapper mapper, ICountryService countryService) => {   
    var countryDto = mapper.Map(country);
    var countryId = await countryService.CreateOrUpdateAsync(countryDto); 
    if (countryId <= 0)
        return Results.StatusCode(StatusCodes.Status500InternalServerError);

    if (country.Id is null)
        return Results.CreatedAtRoute("countryById", new { Id = countryId });
    return Results.NoContent();
}).AddEndpointFilter<InputValidatorFilter<Country>>();

app.MapPatch("/countries/{id}", async (int id, [FromBody] CountryPatch countryPatch, ICountryMapper mapper, ICountryService countryService) => {  
    if (await countryService.UpdateDescriptionAsync(id, countryPatch.Description))
        return Results.NoContent();

    return Results.NotFound();
}).AddEndpointFilter<InputValidatorFilter<CountryPatch>>();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DemoContext>();
    db.Database.SetConnectionString(dbConnection);
    db.Database.Migrate();
}

/*
using (var client = new HttpClient())
{
    byte[] fileBytes = await client.GetByteArrayAsync("https://anthonygiretti.blob.core.windows.net/countryflags/ca.png");
}
*/

app.Run();