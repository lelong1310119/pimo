using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Text;

namespace PismoWebInput.Core.Infrastructure.Common.Extensions;

public static class Objects
{
    public static T? To<T>(this object value)
    {
        var conversionType = typeof(T);
        return (T?)To(value, conversionType);
    }

    public static T? To<T>(this object value, object defaultValue)
    {
        try
        {
            var conversionType = typeof(T);
            return (T?)To(value, conversionType);
        }
        catch (Exception)
        {
            return (T?)defaultValue;
        }
    }

    public static object? To(this object? value, Type conversionType)
    {
        // Note: This if block was taken from Convert.ChangeType as is, and is needed here since we're
        // checking properties on conversionType below.
        if (conversionType == null)
            throw new ArgumentNullException(nameof(conversionType));

        // If it's not a nullable type, just pass through the parameters to Convert.ChangeType

        if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            // It's a nullable type, so instead of calling Convert.ChangeType directly which would throw a
            // InvalidCastException (per http://weblogs.asp.net/pjohnson/archive/2006/02/07/437631.aspx),
            // determine what the underlying type is
            // If it's null, it won't convert to the underlying type, but that's fine since nulls don't really
            // have a type--so just return null
            // Note: We only do this check if we're converting to a nullable type, since doing it outside
            // would diverge from Convert.ChangeType's behavior, which throws an InvalidCastException if
            // value is null and conversionType is a value type.
            if (value == null)
                return null;

            // It's a nullable type, and not null, so that means it can be converted to its underlying type,
            // so overwrite the passed-in conversion type with this underlying type
            var nullableConverter = new NullableConverter(conversionType);
            conversionType = nullableConverter.UnderlyingType;
        }
        else if (conversionType == typeof(Guid))
        {
            return new Guid(value?.ToString() ?? string.Empty);
        }
        else if (conversionType == typeof(long) && value is int)
        {
            //there is an issue with SQLite where the PK is ALWAYS int64. If this conversion type is Int64
            //we need to throw here - suggesting that they need to use LONG instead


            throw new InvalidOperationException(
                "Can't convert an Int64 (long) to Int32(int). If you're using SQLite - this is probably due to your PK being an INTEGER, which is 64bit. You'll need to set your key to long.");
        }

        // Now that we've guaranteed conversionType is something Convert.ChangeType can handle (i.e. not a
        // nullable type), pass the call on to Convert.ChangeType
        return Convert.ChangeType(value, conversionType);
    }

    public static IDictionary<string, object?> ToDictionary(this object value)
    {
        var result = new Dictionary<string, object?>();
        var props = value.GetType().GetProperties();
        foreach (var pi in props)
            try
            {
                result.Add(pi.Name, pi.GetValue(value, null));
            }
            catch
            {
                // ignored
            }

        return result;
    }

    public static IDictionary<string, object?> ToDictionary(this Type type)
    {
        var result = new Dictionary<string, object?>();
        FieldInfo[] fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);

        foreach (FieldInfo fi in fields)
            try
            {
                result.Add(fi.Name, fi.GetValue(null).ToString());
            }
            catch
            {
                // ignored
            }

        return result;
    }

    public static string ReplaceWhitespace(this string input)
    {
        return Regex.Replace(input, @"\s+", "");
    }

    public static string NonUnicode(this string text)
    {
        string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
    "đ",
    "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
    "í","ì","ỉ","ĩ","ị",
    "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
    "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
    "ý","ỳ","ỷ","ỹ","ỵ"};
        string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
    "d",
    "e","e","e","e","e","e","e","e","e","e","e",
    "i","i","i","i","i",
    "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
    "u","u","u","u","u","u","u","u","u","u","u",
    "y","y","y","y","y"};
        for (int i = 0; i < arr1.Length; i++)
        {
            text = text.Replace(arr1[i], arr2[i]);
            text = text.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
        }
        return text;
    }
}