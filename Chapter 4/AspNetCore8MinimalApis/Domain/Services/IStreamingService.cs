namespace Domain.Services;

public interface IStreamingService
{
    Task<(Stream stream, string mimeType)> GetFileStream();
}
