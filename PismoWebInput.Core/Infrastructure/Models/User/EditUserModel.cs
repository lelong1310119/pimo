using System.ComponentModel.DataAnnotations;
using AutoMapper;
using PismoWebInput.Core.Infrastructure.Common.Mappings;
using PismoWebInput.Core.Infrastructure.Domain.Entities;

namespace PismoWebInput.Core.Infrastructure.Models.User
{
    public class EditUserModel : ICustomMappings
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Username { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }

        [Required]
        public string Role { get; set; }
        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<AppUser, EditUserModel>()
                .ForMember(x => x.Role, opt => opt.MapFrom(x => x.UserRoles.Count > 0 ? x.UserRoles.First().Role.Name : ""));
            configuration.CreateMap<EditUserModel, AppUser>();
        }
    }
}
