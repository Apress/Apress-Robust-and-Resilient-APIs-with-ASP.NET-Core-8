using AspNetCore8MinimalApis.Endpoints;
using AspNetCore8MinimalApis.Mapping;
using AspNetCore8MinimalApis.Mapping.Interfaces;
using BLL.Services; 
using Domain.DTOs;
using Domain.Repositories;
using Domain.Services;
using Infrastructure.SQL.Database;
using Infrastructure.SQL.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var dbConnection1 = builder.Configuration.GetValue<string>("ConnectionStrings:DemoDb1");

builder.Services.AddDbContextPool<DemoContext>(options => options.UseSqlServer(dbConnection1));

builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<ICountryMapper, CountryMapper>();
builder.Services.AddScoped<ICountryService, CountryService>();

var app = builder.Build();

app.MapGet("/countries", CountryEndpoints.GetCountries);

app.Run();