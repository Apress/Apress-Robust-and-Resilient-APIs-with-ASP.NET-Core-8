namespace AspNetCore8MinimalApis.Models;

public class CountryIds
{
    public List<int> Ids { get; set; }

    public static bool TryParse(string? value, IFormatProvider? provider, out CountryIds countryIds)
    {
        countryIds = new CountryIds();
        countryIds.Ids = new List<int>();
        try
        {
            if (value is not null && value.Contains("-"))
            {
                countryIds.Ids = value.Split('-').Select(int.Parse).ToList();
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }
}
