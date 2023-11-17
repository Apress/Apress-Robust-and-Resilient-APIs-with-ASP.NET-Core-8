using Domain.Repositories;

namespace Infrastructure.Http.Repositories;

public class MediaRepository /*: IMediaRepository*/
{
    private readonly IHttpClientFactory _httpClientFactory;

    public MediaRepository(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<(byte[] Content, string MimeType)> GetCountryFlagContent(string countryShortName)
    {
        byte[] fileBytes;

        using HttpClient client = _httpClientFactory.CreateClient();
        fileBytes = await client.GetByteArrayAsync($"https://anthonygiretti.blob.core.windows.net/countryflags/{countryShortName}.png");

        return (fileBytes, "image/png");
    }
}
