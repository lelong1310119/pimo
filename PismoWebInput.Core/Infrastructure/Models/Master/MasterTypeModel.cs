using AutoMapper;
using PismoWebInput.Core.Infrastructure.Common.Mappings;
using PismoWebInput.Core.Infrastructure.Domain.Entities;

namespace PismoWebInput.Core.Infrastructure.Models.Master
{
    public class MasterTypeModel : ICustomMappings
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<MasterType, MasterTypeModel>();
            configuration.CreateMap<MasterTypeModel, MasterType>();
        }
    }
}
