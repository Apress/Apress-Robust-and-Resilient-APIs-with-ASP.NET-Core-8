using Microsoft.AspNetCore.Mvc;

namespace AspNetCore8MinimalApis.Models;

public class Address
{
    public int StreetNumber { get; set; }

    public string StreetName { get; set; }

    public string StreetType { get; set; }

    public string City { get; set; }

    public string Country { get; set; }

    public int PostalCode { get; set; }
}