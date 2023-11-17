using Asp.Versioning;
using Asp.Versioning.Conventions;
using AspNetCore8MinimalApis.Mapping;
using AspNetCore8MinimalApis.Mapping.Interfaces;
using AspNetCore8MinimalApis.Models;
using AspNetCore8MinimalApis.RouteGroups;
using AspNetCore8MinimalApis.Swagger;
using BLL.Services;
using Domain.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System;
//using Microsoft.AspNetCore.OpenApi;
using Asp.Versioning.ApiExplorer;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.Annotations;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddScoped<ICountryMapper, CountryMapper>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<IStreamingService, StreamingService>();
builder.Services.AddHttpClient();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowAnyOrigin();
        });

    options.AddPolicy("Restricted",
        builder =>
        {
            builder.AllowAnyHeader()
                   .WithMethods("GET", "POST", "PUT", "DELETE")
                   .WithOrigins("https://mydomain.com", "https://myotherdomain.com")
                   .AllowCredentials();
        });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = new HeaderApiVersionReader("api-version");
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VV"; //format the version as "'v'major[.minor]"
});

builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
    options.AddXmlComments();
});
builder.Services.AddSingleton<IConfigureOptions<SwaggerGenOptions>, SwaggerConfigurationsOptions>();

builder.Services.AddAntiforgery();

var app = builder.Build();

var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
app.UseSwagger().UseSwaggerUI(c =>
{
    c.SwaggerEndpoint($"/swagger/v1.0/swagger.json", "Version 1.0");
    c.SwaggerEndpoint($"/swagger/v2.0/swagger.json", "Version 2.0");
    //foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions.Reverse())
    //{
    //    c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
    //        description.GroupName.ToUpperInvariant());
    //}
});


app.UseCors("Restricted");


/* routing examples */
//app.MapGet("/countries/{countryId}", (int countryId) => $"CountryId {countryId}");

//app.MapGet("/countries", () => new List<string> { "France", "Canada", "Italy" });

app.MapGet("/date/{date}", (DateTime date) => date.ToString());

app.MapGet("/uniqueidentifier/{id}", (Guid id) => id.ToString());

app.MapMethods("/users/{userId}", new List<string> { "PUT", "PATCH" }, (HttpRequest request) =>
{
    var id = request.RouteValues["id"];
    var lastActivityDate = request.Form["lastactivitydate"];
});

app.MapMethods("/routeName", new List<string> { "OPTIONS", "HEAD", "TRACE" }, () => { 
    // Do action
});


app.MapGet("/provinces/{provinceId:int:max(12)}", (int provinceId) => $"ProvinceId {provinceId}");


/* routing examples */

/* Route groups examples */
//app.MapGroup("/countries").GroupCountries();
/* Route groups examples */

/* Parameter binding examples */
app.MapPost("/Addresses", ([FromBody] Address address) => {
    return Results.Created();
});

app.MapPut("/Addresses/{addressId}", ([FromRoute] int addressId, [FromForm] Address address) => {
    return Results.NoContent();
}).DisableAntiforgery();

app.MapGet("/Addresses", ([FromHeader] string coordinates, [FromQuery] int? limitCountSearch) => {
    return Results.Ok();
});

app.MapGet("/IdList", ([FromQuery] int[] id) =>
{
    return Results.Ok();
});

app.MapGet("/languages", ([FromHeader(Name = "lng")] string[] lng) =>
{
    return Results.Ok(lng);
});

/* Parameter binding examples */

/* Validation examples & CRUD examples */

app.MapPost("/countries", ([FromBody] Country country, IValidator<Country> validator, ICountryMapper mapper, ICountryService countryService) => {
    var validationResult = validator.Validate(country);

    if (validationResult.IsValid)
    {
        var countryDto = mapper.Map(country);
        return Results.CreatedAtRoute("countryById", new { Id = countryService.CreateOrUpdate(countryDto) });
    }
    return Results.ValidationProblem(validationResult.ToDictionary());
});

app.MapGet("/countries/{id}", (int id, ICountryMapper mapper, ICountryService countryService) => {
    var country = countryService.Retrieve(id);

    if (country is null)
        return Results.NotFound();

    return Results.Ok(mapper.Map(country));
}).WithName("countryById");

app.MapGet("/countries", (ICountryMapper mapper, ICountryService countryService) => {
    var countries = countryService.GetAll();
    return Results.Ok(mapper.Map(countries));
});

app.MapDelete("/countries/{id}", (int id, ICountryService countryService) => {
    if (countryService.Delete(id))
        return Results.NoContent();

    return Results.NotFound();
});

app.MapPut("/countries", ([FromBody] Country country, IValidator<Country> validator, ICountryMapper mapper, ICountryService countryService) => {
    var validationResult = validator.Validate(country);

    if (validationResult.IsValid)
    {
        if (country.Id is null)
            return Results.CreatedAtRoute("countryById", new { Id = countryService.CreateOrUpdate(mapper.Map(country)) });
        return Results.NoContent();
    }
    return Results.ValidationProblem(validationResult.ToDictionary());
});

app.MapPatch("/countries/{id}", (int id, [FromBody] CountryPatch countryPatch, IValidator<CountryPatch> validator, ICountryMapper mapper, ICountryService countryService) => {
    var validationResult = validator.Validate(countryPatch);

    if (validationResult.IsValid)
    {
        if (countryService.UpdateDescription(id, countryPatch.Description))
            return Results.NoContent();

        return Results.NotFound();
    }
    return Results.ValidationProblem(validationResult.ToDictionary());
});

/* Validation examples */


/* Download example */

app.MapGet("/countries/download", (ICountryService countryService) =>
{

    (byte[] fileContent, string mimeType, string fileName) = countryService.GetFile();

    if (fileContent is null || mimeType is null)
        return Results.NotFound();

    return Results.File(fileContent, mimeType, fileName);
})
.Produces<Stream>(StatusCodes.Status200OK, "video/mp4")
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status500InternalServerError)
.Produces(StatusCodes.Status408RequestTimeout);

/* Download example */

/* Upload example */

app.MapPost("/countries/upload", (IFormFile file, IValidator<IFormFile> validator) =>
{
    var validationResult = validator.Validate(file);
    if (validationResult.IsValid)
    {
        return Results.Created();
    }
    return Results.ValidationProblem(validationResult.ToDictionary());
});

app.MapPost("/countries/uploadmany", (IFormFileCollection files) =>
{
    return Results.Created();
});

//WAIT FOR this bug to get fixed: https://github.com/dotnet/aspnetcore/issues/49526
app.MapPost("/countries/uploadwithmetadata", ([FromForm] CountryMetaData countryMetaData) =>
{
    return Results.Created();
}).DisableAntiforgery();

//WAIT FOR this bug to get fixed: https://github.com/dotnet/aspnetcore/issues/49526
app.MapPost("/countries/uploadmanywithmetadata", ([FromForm] CountryMetaData countryMetaData, IFormFileCollection files) =>
{
    return Results.Created();
}).DisableAntiforgery();

/* Upload example */

/* Streaming example */

app.MapGet("/streaming", async (IStreamingService streamingService) =>
{
    (Stream stream, string mimeType) = await streamingService.GetFileStream();
    return Results.Stream(stream, mimeType, enableRangeProcessing: true);
});

/* Streaming example */

/* Api versioning */
var versionSet = app.NewApiVersionSet()
                    .HasApiVersion(1.0)
                    .HasApiVersion(2.0)
                    .Build();

app.MapGet("/version", () => "Hello version 1").WithApiVersionSet(versionSet).MapToApiVersion(1.0);
app.MapGet("/version", () => "Hello version 2").WithApiVersionSet(versionSet).MapToApiVersion(2.0);
app.MapGet("/version2only", () => "Hello version 2 only").WithApiVersionSet(versionSet).MapToApiVersion(2.0);
app.MapGet("/versionneutral", [SwaggerOperation(Summary = "Neutral version", Description = "This version is neutral")] () => "Hello neutral version").WithApiVersionSet(versionSet).IsApiVersionNeutral();
//.WithOpenApi(operation => new(operation)
//{
//    Summary = "This is a summary",
//    Description = "This is a description"
//});

app.MapGroup("/v1")
   .GroupVersion1()
   .WithTags("V1")
   .WithOpenApi(operation => new(operation)
   {
       Deprecated = true
   });
   //.ExcludeFromDescription();

app.MapGroup("/v2")
   .GroupVersion2()
   .WithTags("V2");

/* Api versioning */
app.Run();
