using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace PismoWebInput.Core.Infrastructure.Common.Extensions;

public static class Strings
{
    public static string? ToSlug(this string value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.ToSlugOrDefault();
    }

    public static string? ToSlugOrDefault(this string value, string? defaultValue = null)
    {
        if (string.IsNullOrWhiteSpace(value)) return defaultValue?.ToSlug();

        value = value.RemoveDiacritics().ToLowerInvariant();
        var bytes = Encoding.GetEncoding("Cyrillic").GetBytes(value);
        value = Encoding.ASCII.GetString(bytes);
        value = Regex.Replace(value, @"\s+", "-", RegexOptions.Compiled);
        value = Regex.Replace(value, @"[^\w\s\p{Pd}]", "", RegexOptions.Compiled);
        value = value.Trim('-', '_');
        value = Regex.Replace(value, @"([-_]){2,}", "$1", RegexOptions.Compiled);
        return value.ToLower();
    }

    private static string RemoveDiacritics(this string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark) stringBuilder.Append(c);
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    public static string RandomString(int length)
    {
        var sb = new StringBuilder(length);
        var random = new Random(Guid.NewGuid().GetHashCode());
        for (var i = 0; i < length; i++) sb.Append((char)('A' + random.Next(0, 26)));
        return sb.ToString();
    }
}