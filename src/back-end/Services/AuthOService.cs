using System.Security.Claims;

namespace back_end.Services
{
    public class AuthOService : IAuthOService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string RoleClaimType = "https://schemas.sushitoshi.com/roles";

        public AuthOService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetUserId(ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? user.FindFirst("sub")?.Value 
                ?? string.Empty;
        }

            public string GetUserEmail(ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Email)?.Value 
            ?? user.FindFirst("email")?.Value 
            ?? string.Empty;
    }
    
    public IEnumerable<string> GetUserRoles(ClaimsPrincipal user)
    {
        var rolesClaim = user.FindFirst(RoleClaimType)?.Value;
        
        if (string.IsNullOrEmpty(rolesClaim))
            return Enumerable.Empty<string>();
        
        // Auth0 may return roles as JSON array string
        if (rolesClaim.StartsWith("["))
        {
            return System.Text.Json.JsonSerializer.Deserialize<string[]>(rolesClaim) 
                ?? Enumerable.Empty<string>();
        }
        
        return new[] { rolesClaim };
    }
    
    public bool IsInRole(ClaimsPrincipal user, string role)
    {
        return GetUserRoles(user).Contains(role, StringComparer.OrdinalIgnoreCase);
    }
    }
}