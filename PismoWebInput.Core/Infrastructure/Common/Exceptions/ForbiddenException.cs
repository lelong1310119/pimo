namespace PismoWebInput.Core.Infrastructure.Common.Exceptions;

public class ForbiddenException : HttpStatusCodeException
{
    public ForbiddenException(string? message = null)
        : base(403, message)
    {
    }
}