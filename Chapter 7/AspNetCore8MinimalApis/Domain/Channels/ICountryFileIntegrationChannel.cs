namespace Domain.Channels;

public interface ICountryFileIntegrationChannel
{
    IAsyncEnumerable<Stream> ReadAllAsync(CancellationToken cancellationToken);
    Task<bool> SubmitAsync(Stream twilioRouteProgrammerParameters, CancellationToken cancellationToken);
}
