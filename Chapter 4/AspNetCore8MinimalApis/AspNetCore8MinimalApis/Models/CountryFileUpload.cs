using Microsoft.AspNetCore.Mvc;

namespace AspNetCore8MinimalApis.Models;

public class CountryFileUpload
{
    public IFormFile File { get; set; }

    public string AuthorName { get; set; }
    public string Description { get; set; }
}
