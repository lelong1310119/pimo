namespace PismoWebInput.Core.Infrastructure.Common.Extensions;

public static class ExceptionExtensions
{
    public static string GetBaseErrorMessage(this Exception ex)
    {
        var message = ex.GetBaseException().Message;
        return string.IsNullOrWhiteSpace(message) || message.StartsWith("Exception of type") ? string.Empty : message;
    }
}