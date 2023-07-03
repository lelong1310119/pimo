using AutoMapper;
using PismoWebInput.Core.Infrastructure.Common.Mappings;
using PismoWebInput.Core.Infrastructure.Domain.Entities;

namespace PismoWebInput.Core.Infrastructure.Models.Log
{
    public class ActivityLogModel : ICustomMappings
    {
        public int UserId { get; set; }
        public string Action { get; set; }
        public string? Status { get; set; }
        public string? Detail { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UserName { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<ActivityLog, ActivityLogModel>().ForMember(x => x.UserName, opt => opt.MapFrom(x => x.User.UserName));
            configuration.CreateMap<ActivityLogModel, ActivityLog>();
        }
    }
}
