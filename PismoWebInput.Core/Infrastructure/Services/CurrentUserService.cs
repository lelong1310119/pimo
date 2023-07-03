using Microsoft.AspNetCore.Http;
using PismoWebInput.Core.Infrastructure.Common.Extensions;

namespace PismoWebInput.Core.Infrastructure.Services;

public interface ICurrentUserService
{
    string? UserId { get; }
}

public class CurrentUserService : ICurrentUserService
{
    private readonly HttpContext _httpContext;
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContext = httpContextAccessor.HttpContext;
    }

    private string? _userId;
    public string? UserId => _userId ??= _httpContext?.User.GetUserId();
}