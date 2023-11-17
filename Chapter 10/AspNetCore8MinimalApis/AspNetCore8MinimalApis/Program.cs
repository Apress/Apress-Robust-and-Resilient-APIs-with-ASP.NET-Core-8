using AspNetCore8MinimalApis.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserProfile, UserProfile>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Authority = "https://login.microsoftonline.com/136544d9-038e-4646-afff-10accb370679/v2.0";
    options.Audience = "257b6c36-1168-4aac-be93-6f2cd81cec43";
    options.TokenValidationParameters.ValidateLifetime = true;
    options.TokenValidationParameters.ValidateIssuer = true;
    options.TokenValidationParameters.ClockSkew = TimeSpan.FromMinutes(5);
});

builder.Services
        .AddAuthorization(options => 
         options.AddPolicy("SurveyCreator", 
         policy =>  policy.RequireRole("SurveyCreator")
));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header required"
        
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/authenticated", () =>
{
    return Results.Ok("Authenticated !");

}).RequireAuthorization();

app.MapGet("/authorized", (IUserProfile userProfile) =>
{
    return Results.Ok("Authorized !");
}).RequireAuthorization("SurveyCreator");

app.Run();