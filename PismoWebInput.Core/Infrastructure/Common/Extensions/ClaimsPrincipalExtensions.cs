using System.Security.Claims;

namespace PismoWebInput.Core.Infrastructure.Common.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserName(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.Name);
    }

    public static string? GetUserId(this ClaimsPrincipal user)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return userId != null ? user.FindFirstValue(ClaimTypes.NameIdentifier).To<string>() : null;
    }

    public static string GetEmail(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.Email);
    }
}