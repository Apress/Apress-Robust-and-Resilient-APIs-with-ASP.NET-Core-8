using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace AspNetCore8MinimalApis.Swagger;

public static class SwaggerXmlComments
{
    public static void AddXmlComments(this SwaggerGenOptions options)
    {
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        options.IncludeXmlComments(xmlPath);
    }
}
