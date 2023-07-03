using Humanizer;
using Microsoft.EntityFrameworkCore;

namespace PismoWebInput.Core.Persistence.Extensions;

internal static class ModelBuilderExtensions
{
    public static ModelBuilder UseDefaultConventions(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            entityType.SetTableName(entityType.GetTableName().Singularize());

        foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(t => t.GetProperties()))
        {
            var entity = modelBuilder.Entity(property.DeclaringEntityType.Name).Property(property.Name);
            if (property.ClrType == typeof(decimal) || property.ClrType == typeof(decimal?))
                entity.HasColumnType("decimal(18,4)");
            else if (property.ClrType.IsEnum)
                entity.HasMaxLength(50)
                    .IsRequired()
                    .HasConversion<string>();
            else if (property.ClrType == typeof(string)) entity.HasMaxLength(450);
        }

        return modelBuilder;
    }
}