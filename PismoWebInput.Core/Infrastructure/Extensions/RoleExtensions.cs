using System.Security.Claims;

namespace PismoWebInput.Core.Infrastructure.Extensions
{
    public static class RoleExtensions
    {
        public static bool IsInRoles(this ClaimsPrincipal user, IEnumerable<string> roles)
        {
            foreach (var role in roles)
            {
                if (user.IsInRole(role))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
