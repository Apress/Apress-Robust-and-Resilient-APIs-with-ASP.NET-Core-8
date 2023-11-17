namespace AspNetCore8MinimalApis.Identity;

public interface IUserProfile
{
    string Name { get; }
    IEnumerable<string> Roles { get; }
}
