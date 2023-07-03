using AutoMapper;

namespace PismoWebInput.Core.Infrastructure.Common.Mappings;

public interface ICustomMappings
{
    void CreateMappings(IMapperConfigurationExpression configuration);
}