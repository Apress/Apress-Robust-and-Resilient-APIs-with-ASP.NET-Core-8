using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AspNetCore8MinimalApis.Healthchecks;

public class ReadyHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var result = Ready.IsReady
            ? HealthCheckResult.Healthy()
            : HealthCheckResult.Unhealthy("Application not ready");

        return Task.FromResult(result);
    }
}
