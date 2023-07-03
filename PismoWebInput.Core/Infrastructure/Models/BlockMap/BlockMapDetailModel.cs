using AutoMapper;
using PismoWebInput.Core.Infrastructure.Common.Mappings;
using PismoWebInput.Core.Infrastructure.Domain.Entities;

namespace PismoWebInput.Core.Infrastructure.Models.BlockMap
{
    public class BlockMapDetailModel : ICustomMappings
    {
        public long Id { get; set; }
        public long BlockMapId { get; set; }
        public int? Location { get; set; }
        public long? DefectId { get; set; }
        public string? DefectCode { get; set; }
        public string? DefectName { get; set; }
        public string? PcsId { get; set; }
        public DateTime? PcsIdUpdateTime { get; set; }
        public int? Status { get; set; }
        public int? Order { get; set; }
        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<BlockMapDetail, BlockMapDetailModel>()
                .ForMember(x => x.DefectCode, opt => opt.MapFrom(x => x.Master!.Code))
                .ForMember(x => x.DefectName, opt => opt.MapFrom(x => x.Master!.Name));
        }
    }
}
