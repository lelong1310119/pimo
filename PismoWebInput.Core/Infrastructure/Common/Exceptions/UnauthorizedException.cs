namespace PismoWebInput.Core.Infrastructure.Common.Exceptions;

public class UnauthorizedException : HttpStatusCodeException
{
    public UnauthorizedException(string? message = null)
        : base(401, message)
    {
    }
}