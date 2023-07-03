namespace PismoWebInput.Core.Infrastructure.Common.Extensions;

public static class EnumExtensions
{
    public static T? ToEnum<T>(this object obj, bool ignoreCase = true)
        where T : struct
    {
        if (Enum.TryParse(obj.ToString(), ignoreCase, out T result)) return result;
        return null;
    }

    public static T ToEnum<T>(this object obj, T defaultValue)
        where T : struct
    {
        var result = obj.ToEnum<T>();
        return result ?? defaultValue;
    }

    public static bool IsEnumType<T>(this object obj)
        where T : struct
    {
        return ToEnum<T>(obj) != null;
    }
}