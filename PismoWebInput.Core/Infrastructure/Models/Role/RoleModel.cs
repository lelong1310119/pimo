using AutoMapper;
using Microsoft.AspNetCore.Identity;
using PismoWebInput.Core.Infrastructure.Common.Mappings;
using PismoWebInput.Core.Infrastructure.Domain.Entities;

namespace PismoWebInput.Core.Infrastructure.Models.Role
{
    public class RoleModel : ICustomMappings
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<AppRole, RoleModel>();
        }
    }
}
