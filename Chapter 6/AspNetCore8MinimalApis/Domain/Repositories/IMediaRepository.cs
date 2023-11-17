using Refit;

namespace Domain.Repositories;

public interface IMediaRepository
{
    [Get("/countryflags/{countryShortName}.png")]
    Task<byte[]> GetCountryFlagContent(string countryShortName);
}
