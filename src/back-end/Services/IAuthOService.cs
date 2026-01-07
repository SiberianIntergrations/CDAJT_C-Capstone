using System.Security.Claims;

namespace back_end.Services
{
    public interface IAuthOService
    {
        string GetUserId(ClaimsPrincipal user);
        string GetUserEmail(ClaimsPrincipal user);
        IEnumerable<string> GetUserRoles(ClaimsPrincipal user);
        bool IsInRole(ClaimsPrincipal user, string role);
    }
}