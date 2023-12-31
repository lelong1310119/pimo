namespace PismoWebInput.Core.Infrastructure.Common.Timing;

/// <summary>
///     Used to perform some common date-time operations.
/// </summary>
public static class Clock
{
    private static IClockProvider _provider;

    static Clock()
    {
        Provider = new LocalClockProvider();
    }

    /// <summary>
    ///     This object is used to perform all <see cref="Clock" /> operations.
    ///     Default value: <see cref="LocalClockProvider" />.
    /// </summary>
    public static IClockProvider Provider
    {
        get => _provider;
        set => _provider = value ?? throw new SystemException("Can not set Clock to null!");
    }

    /// <summary>
    ///     Gets Now using current <see cref="Provider" />.
    /// </summary>
    public static DateTime Now => Provider.Now;

    /// <summary>
    ///     Normalizes given <see cref="DateTime" /> using current <see cref="Provider" />.
    /// </summary>
    /// <param name="dateTime">DateTime to be normalized.</param>
    /// <returns>Normalized DateTime</returns>
    public static DateTime Normalize(DateTime dateTime)
    {
        return Provider.Normalize(dateTime);
    }
}