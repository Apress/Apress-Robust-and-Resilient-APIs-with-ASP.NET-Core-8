using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AspNetCore8MinimalApis.Swagger;

public class SwaggerConfigurationsOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _apiVersionDescriptionProvider;

    public SwaggerConfigurationsOptions(IApiVersionDescriptionProvider apiVersionDescriptionProvider)
    {
        _apiVersionDescriptionProvider = apiVersionDescriptionProvider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateOpenApiInfo(description));
        }
    }

    private static OpenApiInfo CreateOpenApiInfo(ApiVersionDescription description)
    {
        var info = new OpenApiInfo()
        {
            Title = "ASP.NET Core 8 Minimal APIs",
            Version = description.ApiVersion.ToString()
        };

        return info;
    }
}
