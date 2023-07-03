using AutoMapper;
using PismoWebInput.Core.Infrastructure.Common.Mappings;
using PismoWebInput.Core.Infrastructure.Domain.Entities;
using PismoWebInput.Core.Infrastructure.Models.Master;
using PismoWebInput.Core.Infrastructure.Models.Role;

namespace PismoWebInput.Core.Infrastructure.Models.User
{
    public class UserModel : ICustomMappings
    {
        public UserModel()
        {
            Operators = new List<MasterModel>();
            Roles = new List<RoleModel>();
        }

        public string? Id { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public List<RoleModel> Roles { get; set; }
        public List<MasterModel> Operators { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<AppUser, UserModel>()
                .ForMember(
                    x => x.Roles,
                    opt => opt.MapFrom(x => x.UserRoles.Select(r => r.Role).MapToList<RoleModel>().ToList())
                    )
                .ForMember(
                    x => x.Operators,
                    opt => opt.MapFrom(x => x.UserOperations.Select(r => r.Master).MapToList<MasterModel>().ToList())
                    )
                ;
        }
    }
}
