using System.ComponentModel.DataAnnotations;
using AutoMapper;
using PismoWebInput.Core.Enums;
using PismoWebInput.Core.Infrastructure.Common.Mappings;

namespace PismoWebInput.Core.Infrastructure.Models.Master
{
    public class MasterModel : ICustomMappings
    {
        public long? Id { get; set; }
        [Required]
        public string? Code { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public MasterTypeEnum? Type { get; set; }
        public string? Text1 { get; set; }
        public string? Text2 { get; set; }
        public int Num1 { get; set; }
        public int Num2 { get; set; }
        public int Status { get; set; }
        public int? Order { get; set; }
        public bool Checked { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<Domain.Entities.Master, MasterModel>();
            configuration.CreateMap<MasterModel, Domain.Entities.Master>();
        }
    }
}
