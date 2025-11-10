using System.Security.Claims;

namespace back_end.Helpers
{
    public static class ClaimsHelpers
    {
    public static string GetUserOid(ClaimsPrincipal user)
    {
        return user.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value
            ?? user.FindFirst("oid")?.Value
            ?? user.FindFirst("preferred_username")?.Value        // fallback for customer logins
            ?? user.FindFirst("email")?.Value                     // fallback for email-based tokens
            ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value   // fallback for generic OIDC
            ?? string.Empty;
    }

        public static string GetUserDisplayName(ClaimsPrincipal user)
        {
            // Prefer given_name + family_name if either is present, else fallback to name
            var given = user.FindFirst("given_name")?.Value?.Trim();
            var family = user.FindFirst("family_name")?.Value?.Trim();
            if (!string.IsNullOrEmpty(given) || !string.IsNullOrEmpty(family))
            {
                var full = $"{given} {family}".Trim();
                return string.IsNullOrWhiteSpace(full) ? "Unknown" : full;
            }
            var name = user.FindFirst("name")?.Value?.Trim();
            return string.IsNullOrWhiteSpace(name) ? "Unknown" : name;
        }

        public static string? GetUserRole(ClaimsPrincipal user)
        {
            return user?.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Role || c.Type == "roles" || c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
                ?.Value;
        }
    }
}
