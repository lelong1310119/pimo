using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PismoWebInput.Core.Infrastructure.Common.Extensions;

namespace PismoWebInput.Core.Persistence.Extensions;

public static class ValueConversionExtensions
{
    public static PropertyBuilder<T> HasJsonConversion<T>(this PropertyBuilder<T> propertyBuilder)
        where T : class, new()
    {
        var converter = new ValueConverter<T?, string>
        (
            v => v.ToJson(),
            v => v.FromJson(new T())
        );

        var comparer = new ValueComparer<T?>
        (
            (l, r) => l.ToJson() == r.ToJson(),
            v => v == null ? 0 : v.ToJson().GetHashCode(),
            v => v.ToJson().FromJson(new T())
        );

        propertyBuilder.HasConversion(converter!);
        propertyBuilder.Metadata.SetValueConverter(converter);
        propertyBuilder.Metadata.SetValueComparer(comparer);
        propertyBuilder.HasColumnType("nvarchar(max)");

        return propertyBuilder;
    }
}