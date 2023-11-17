using AspNetCore8MinimalApis.EndpointFilters;
using AspNetCore8MinimalApis.Endpoints;
using AspNetCore8MinimalApis.ExceptionHandlers;
using AspNetCore8MinimalApis.Mapping;
using AspNetCore8MinimalApis.Mapping.Interfaces;
using AspNetCore8MinimalApis.Middlewares;
using AspNetCore8MinimalApis.Models;
using AspNetCore8MinimalApis.RouteGroups;
using BLL.Services;
using Domain.Enum;
using Domain.Services;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi.Models;
using System.Diagnostics;
using System.Net;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddScoped<ICountryMapper, CountryMapper>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<IPricingTierService, PricingTierService>();
builder.Services.AddExceptionHandler<TimeOutExceptionHandler>();
builder.Services.AddExceptionHandler<DefaultExceptionHandler>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1.0", new OpenApiInfo { Title = "ASP.NET Core 8 minimal APIs"});
});
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = (int)HttpStatusCode.TooManyRequests;
    options.OnRejected = async (context, token) =>
    {
        await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.");
    };
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        var priceTierService = httpContext.RequestServices.GetRequiredService<IPricingTierService>();
        var ip = httpContext.Connection.RemoteIpAddress.ToString();
        var priceTier = priceTierService.GetPricingTier(ip);

        return priceTier switch
        {
            PricingTier.Paid => RateLimitPartition.GetFixedWindowLimiter(
                ip,
                _ => new FixedWindowRateLimiterOptions
                {
                    QueueLimit = 10,
                    PermitLimit = 50,
                    Window = TimeSpan.FromSeconds(15)
                }),
            PricingTier.Free => RateLimitPartition.GetFixedWindowLimiter(
                ip,
                _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 1,
                    Window = TimeSpan.FromSeconds(15)
                })
        };
    });
    /*
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        var priceTierService = httpContext.RequestServices.GetRequiredService<IPricingTierService>();
        var ip = httpContext.Connection.RemoteIpAddress.ToString();
        var priceTier = priceTierService.GetPricingTier(ip);

        return priceTier switch
        {
            PricingTier.Paid => RateLimitPartition.GetSlidingWindowLimiter(
                ip,
                _ => new SlidingWindowRateLimiterOptions
                {
                    QueueLimit = 10,
                    PermitLimit = 50,
                    SegmentsPerWindow = 2,
                    Window = TimeSpan.FromSeconds(15)
                }),
            PricingTier.Free => RateLimitPartition.GetSlidingWindowLimiter(
                ip,
                _ => new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = 2,
                    SegmentsPerWindow = 2,
                    Window = TimeSpan.FromSeconds(15)
                })
        };
    });
    */

    /*
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        var priceTierService = httpContext.RequestServices.GetRequiredService<IPricingTierService>();
        var ip = httpContext.Connection.RemoteIpAddress.ToString();
        var priceTier = priceTierService.GetPricingTier(ip);

        return priceTier switch
        {
            PricingTier.Paid => RateLimitPartition.GetTokenBucketLimiter(
                ip,
                _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 50,
                    TokensPerPeriod = 25,
                    ReplenishmentPeriod = TimeSpan.FromSeconds(15)
                }),
            PricingTier.Free => RateLimitPartition.GetTokenBucketLimiter(
                ip,
                _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 10,
                    TokensPerPeriod = 5,
                    ReplenishmentPeriod = TimeSpan.FromSeconds(15)
                })
        };
    });
    */

    /*
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        var priceTierService = httpContext.RequestServices.GetRequiredService<IPricingTierService>();
        var ip = httpContext.Connection.RemoteIpAddress.ToString();
        var priceTier = priceTierService.GetPricingTier(ip);

        return priceTier switch
        {
            PricingTier.Paid => RateLimitPartition.GetConcurrencyLimiter(
                ip,
                _ => new ConcurrencyLimiterOptions
                {
                    QueueLimit = 10,
                    PermitLimit = 50
                }),
            PricingTier.Free => RateLimitPartition.GetConcurrencyLimiter(
                ip,
                _ => new ConcurrencyLimiterOptions
                {
                    QueueLimit = 0,
                    PermitLimit = 10
                })
        };
    });
    */

    options.AddPolicy(policyName: "ShortLimit", context =>
    {
        var ip = context.Connection.RemoteIpAddress.ToString();
        return RateLimitPartition.GetFixedWindowLimiter(ip,
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 2,
                Window = TimeSpan.FromSeconds(15)
            });
    });
});
var app = builder.Build();

app.UseExceptionHandler(opt => { });

app.UseRateLimiter();

app.UseSwagger().UseSwaggerUI(c =>
{
    c.SwaggerEndpoint($"/swagger/v1.0/swagger.json", "Version 1.0");
});

/* Examples Route groups */
app.AddCountryEndpoints();
/* Examples Route groups */

/* Examples custom binding */
app.MapPost("/countries/upload", (IFormFile file, Country country) =>
{
    Results.NoContent();
}).DisableAntiforgery();

app.MapGet("/countries/ids", ([FromHeader] CountryIds ids) =>
{
    Results.NoContent();
});
/* Examples custom binding */

/* Examples middlewares */
app.Use(async (context, next) =>
{
    //app.Logger.LogInformation("Middleware 1 executed");
    await next();
});// Don't stop the execution

app.MapGet("/test", () =>
{
    //app.Logger.LogInformation("Endpoint GET /test has been invoked");
    return Results.Ok();
});

//app.UseMiddleware<LoggingMiddleware>();// Doesn't stop the execution

app.UseWhen(ctx => !string.IsNullOrEmpty(ctx.Request.Query["p"].ToString()),
builder => {
    builder.Use(async (context, next) =>
    {
        //app.Logger.LogInformation("Nested middleware executed");
        await next();
    });
    
    builder.Run(async (context) =>
    {
        //app.Logger.LogInformation("End of the pipeline end");
        await Task.CompletedTask;
    });  
});// Stops the execution if the condition is met because the UseWhen contains Run Middleware

app.MapWhen(ctx => !string.IsNullOrEmpty(ctx.Request.Query["q"].ToString()),
builder => {
    builder.Use(async (context, next) =>
    {
        //app.Logger.LogInformation("New middleware pipeline branch has been initiated");
        await next();
    });
    builder.Run(async context =>
    {
        //app.Logger.LogInformation("New middleware pipeline will end here");
        await Task.CompletedTask;
    });
});// Stops the execution if the condition is met because the new branch contains Run Middleware

/*
app.Map(new PathString("/test"),
builder =>
{
    builder.Use(async (context, next) =>
    {
        app.Logger.LogInformation("New middleware pipeline branch has been initiated");
        await next();
    });
    builder.Run(async context =>
    {
        app.Logger.LogInformation("New middleware pipeline will end here");
        await Task.CompletedTask;
    });
});// stop execution
*/
/* Examples middlewares */

/* Examples Endpoint Filter */
app.MapGet("/longrunning", async () =>
{
    await Task.Delay(5000);
    return Results.Ok();
}).AddEndpointFilter<LogPerformanceFilter>();

app.MapPost("/countries", ([FromBody] Country country) => {

    return Results.CreatedAtRoute("countryById", new { Id = 1 });
}).AddEndpointFilter<InputValidatorFilter<Country>>();
/* Examples Endpoint Filter */

/* Examples RateLimiting */
app.MapGet("/notlimited", () =>
{
    return Results.Ok();
}).DisableRateLimiting();

app.MapGet("/limited", () =>
{
    return Results.Ok();
}).RequireRateLimiting("ShortLimit");
/* Examples RateLimiting */

/* Examples Exception handling */
app.MapGet("/exception", () => {
    throw new Exception();
});

app.MapGet("/timeout", () => {
    throw new TimeoutException();
});
/* Examples Exception handling */
app.Run(); // Final
