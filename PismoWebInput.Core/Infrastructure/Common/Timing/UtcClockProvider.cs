namespace PismoWebInput.Core.Infrastructure.Common.Timing;

public class UtcClockProvider : IClockProvider
{
    public DateTime Now => DateTime.UtcNow;

    public DateTime Normalize(DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Unspecified) return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);

        if (dateTime.Kind == DateTimeKind.Local) return dateTime.ToUniversalTime();

        return dateTime;
    }
}