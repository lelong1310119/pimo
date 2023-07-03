namespace PismoWebInput.Core.Infrastructure.Common.Exceptions;

public class NotFoundException : HttpStatusCodeException
{
    public NotFoundException(string? message = "Record not found")
        : base(404, message)
    {
    }

    public NotFoundException(string name, object key)
        : this($"Entity \"{name}\" ({key}) was not found.")
    {
    }
}