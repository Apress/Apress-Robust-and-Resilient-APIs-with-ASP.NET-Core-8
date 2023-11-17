using System.ComponentModel.DataAnnotations;

namespace AspNetCore8MinimalApis.Models;

public class Country
{
    /// <summary>
    /// The country Id
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// The country name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The country description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// The country flag URI
    /// </summary>
    public string FlagUri { get; set; }
}