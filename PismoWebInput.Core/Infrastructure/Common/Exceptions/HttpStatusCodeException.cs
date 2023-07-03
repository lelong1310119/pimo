namespace PismoWebInput.Core.Infrastructure.Common.Exceptions;

[Serializable]
public class HttpStatusCodeException : Exception
{
    public HttpStatusCodeException(int statusCode, string? message) : base(message)
    {
        StatusCode = statusCode;
    }

    public int StatusCode { get; }
}