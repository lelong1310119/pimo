using System.Text;
using Microsoft.IdentityModel.Tokens;
using PismoWebInput.Core.Infrastructure.Domain.Options;

namespace PismoWebInput.Core.Infrastructure.Extensions;

internal static class JwtExtensions
{
    public static SymmetricSecurityKey GetSecurityKey(this JwtOptions options)
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key));
    }
}