namespace AspNetCore8MinimalApis.RouteGroups;
public static class MyGroups
{
    public static RouteGroupBuilder GroupCountries(this RouteGroupBuilder group)
    {
        var countries = new string[] { "France", "Canada", "USA" };

        var languages = new Dictionary<string, List<string>>()
        {
            { "France", new List<string> { "french" } },
            { "Canada", new List<string> { "french", "english" } },
            { "USA", new List<string> { "english", "spanish" } }
        };

        group.MapGet("/", () => countries);

        var idGroup = group.MapGroup("/{id}");
        idGroup.MapGet("/", (int id) => countries[id]);
        idGroup.MapGet("/languages", (int id) =>
        {
            var country = countries[id];
            return languages[country];
        });

        return group;
    }
}