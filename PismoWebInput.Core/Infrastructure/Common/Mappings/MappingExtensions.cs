using System.Reflection;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PismoWebInput.Core.Infrastructure.Common.Mappings;

public static class MappingExtensions
{
    private static IMapper Mapper { get; set; }

    public static IHost EnsureAutoMapper(this IHost host)
    {
        host.Services.EnsureAutoMapper();
        return host;
    }

    public static void EnsureAutoMapper(this IServiceProvider serviceProvider)
    {
        Mapper = serviceProvider.GetRequiredService<IMapper>();
    }

    public static IServiceCollection ConfigureAutoMapper(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddAutoMapper(cfg => MappingConfigurator.Configure(cfg, assemblies), assemblies);
        return services;
    }

    public static IQueryable<TDestination> ProjectTo<TDestination>(this IQueryable source)
    {
        return source.ProjectTo<TDestination>(Mapper.ConfigurationProvider);
    }

    public static TDestination MapTo<TDestination>(this object source)
    {
        return Mapper.Map<TDestination>(source);
    }

    public static IList<TDestination> MapToList<TDestination>(this object source)
    {
        return Mapper.Map<List<TDestination>>(source);
    }

    public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
    {
        return Mapper.Map(source, destination);
    }

    public static TDestination MapToList<TSource, TDestination>(this TSource source, TDestination destination)
    {
        return Mapper.Map(source, destination);
    }
}