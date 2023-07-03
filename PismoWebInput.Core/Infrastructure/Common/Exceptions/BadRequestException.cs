namespace PismoWebInput.Core.Infrastructure.Common.Exceptions;

public class BadRequestException : HttpStatusCodeException
{
    public BadRequestException(string? message = null)
        : base(400, message)
    {
    }
}