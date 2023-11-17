using System.Security.Claims;

namespace AspNetCore8MinimalApis.Identity;

public class UserProfile : IUserProfile
{
    private readonly IHttpContextAccessor _context;
    public UserProfile(IHttpContextAccessor context)
    {
        _context = context;
    }

    public string Name => _context.HttpContext.User?.Claims.FirstOrDefault(x => x.Type == "name")?.Value;

    public IEnumerable<string> Roles => _context.HttpContext.User?.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value);
}
