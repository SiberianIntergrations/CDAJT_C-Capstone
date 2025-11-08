using System.Security.Claims;

namespace back_end.Helpers
{
    public static class ClaimsHelpers
    {
        public static string GetUserOid(ClaimsPrincipal user)
        {
            return user.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value
                ?? user.FindFirst("oid")?.Value
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
    }
}
