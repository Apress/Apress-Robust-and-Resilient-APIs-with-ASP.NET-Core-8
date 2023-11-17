using System.Diagnostics;

namespace AspNetCore8MinimalApis.EndpointFilters;

public class LogPerformanceFilter : IEndpointFilter
{
    private readonly ILogger<LogPerformanceFilter> _logger;
    public LogPerformanceFilter(ILogger<LogPerformanceFilter> logger)
    {
        _logger = logger;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        _logger.LogInformation($"GET /longrunning endpoint getting executed");
        long startTime = Stopwatch.GetTimestamp();
        var result = await next(context);
        TimeSpan elapsedTime = Stopwatch.GetElapsedTime(startTime);
        _logger.LogInformation($"GET /longrunning endpoint took {elapsedTime.TotalSeconds} to execute");
        return result;
    }
}
