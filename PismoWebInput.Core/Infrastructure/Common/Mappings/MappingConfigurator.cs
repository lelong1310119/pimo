using System.Reflection;
using AutoMapper;

namespace PismoWebInput.Core.Infrastructure.Common.Mappings;

internal static class MappingConfigurator
{
    internal static void Configure(IMapperConfigurationExpression cfg, params Assembly[] assemblies)
    {
        var types = assemblies
            .SelectMany(x => x.GetExportedTypes())
            .ToList();

        CreateMapFromMappings(cfg, types);
        CreateMapToMappings(cfg, types);
        CreateCustomMappings(cfg, types);
        cfg.AddMaps(assemblies);
    }

    private static void CreateCustomMappings(IMapperConfigurationExpression cfg, IEnumerable<Type> types)
    {
        var maps = (from t in types
            from i in t.GetInterfaces()
            let tInfo = t.GetTypeInfo()
            where typeof(ICustomMappings).IsAssignableFrom(t) &&
                  !tInfo.IsAbstract &&
                  !tInfo.IsInterface
            select (ICustomMappings)Activator.CreateInstance(t)!).ToArray();

        foreach (var map in maps) map.CreateMappings(cfg);
    }

    private static void CreateMapFromMappings(IMapperConfigurationExpression cfg, IEnumerable<Type> types)
    {
        var maps = FilterMappingTypes(types, typeof(IMapFrom<>));

        foreach (var map in maps) cfg.CreateMap(map.GenericType, map.ModelType);
    }

    private static void CreateMapToMappings(IMapperConfigurationExpression cfg, IEnumerable<Type> types)
    {
        var maps = FilterMappingTypes(types, typeof(IMapTo<>));

        foreach (var map in maps) cfg.CreateMap(map.ModelType, map.GenericType);
    }

    private static IEnumerable<MappingType> FilterMappingTypes(IEnumerable<Type> types, Type genericType)
    {
        return (from t in types
            from i in t.GetInterfaces()
            let tInfo = t.GetTypeInfo()
            let iInfo = i.GetTypeInfo()
            where iInfo.IsGenericType && i.GetGenericTypeDefinition() == genericType &&
                  !tInfo.IsAbstract &&
                  !tInfo.IsInterface
            select new MappingType
            {
                GenericType = i.GetGenericArguments()[0],
                ModelType = t
            }).ToArray();
    }

    private class MappingType
    {
        public Type GenericType { get; init; }
        public Type ModelType { get; init; }
    }
}