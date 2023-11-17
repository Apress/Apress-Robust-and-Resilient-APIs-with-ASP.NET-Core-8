using Domain.Channels;
using System.Threading.Channels;

namespace AspNetCore8MinimalApis.Channels;

public class CountryFileIntegrationChannel : ICountryFileIntegrationChannel
{
    private readonly Channel<Stream> _channel;

    public CountryFileIntegrationChannel()
    {

        var options = new UnboundedChannelOptions
        {
            SingleWriter = false,
            SingleReader = true
        };

        _channel = Channel.CreateUnbounded<Stream>(options);
    }


    public async Task<bool> SubmitAsync(Stream fieldContent, CancellationToken cancellationToken)
    {
        while (await _channel.Writer.WaitToWriteAsync(cancellationToken) && !cancellationToken.IsCancellationRequested)
        {
            if (_channel.Writer.TryWrite(fieldContent))
            {
                return true;
            }
        }

        return false;
    }


    public IAsyncEnumerable<Stream> ReadAllAsync(CancellationToken cancellationToken) => _channel.Reader.ReadAllAsync(cancellationToken);
}
