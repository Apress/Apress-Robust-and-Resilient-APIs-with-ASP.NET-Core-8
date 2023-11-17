using Domain.Channels;
using Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.BackgroundTasks;


public class CountryFileIntegrationBackgroudService : BackgroundService
{
    private readonly ICountryFileIntegrationChannel _channel;
    private readonly IServiceProvider _serviceProvider;

    public CountryFileIntegrationBackgroudService(ICountryFileIntegrationChannel channel, IServiceProvider serviceProvider)
    {
        _channel = channel;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await foreach (var fileContent in _channel.ReadAllAsync(cancellationToken))
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var service = scope.ServiceProvider.GetRequiredService<ICountryService>();
                    await service.IngestFileAsync(fileContent);
                }
            }
            catch { }
        }
    }
}










