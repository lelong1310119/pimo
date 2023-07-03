using AutoMapper;
using PismoWebInput.Core.Enums;
using PismoWebInput.Core.Infrastructure.Common.Mappings;

namespace PismoWebInput.Core.Infrastructure.Models.BlockMap
{
    public class BlockMapModel : ICustomMappings
    {
        public long? Id { get; set; }
        public string? IndicationId { get; set; }
        public long? OperationId { get; set; }
        public string? OperationName { get; set; }
        public string? BlockId { get; set; }
        public BlockType? BlockType { get; set; }
        public DateTime? DateInput { get; set; }
        public int? Status { get; set; }
        public int? Order { get; set; }
        public string? EMapId { get; set; }
        public int? Sided { get; set; }
        public int? ErrorCount { get; set; }
        public string CreatedByUser { get; set; }

        public ICollection<BlockMapDetailModel> Details { get; set; }

        public BlockMapModel()
        {
            Details = new List<BlockMapDetailModel>();
        }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<Domain.Entities.BlockMap, BlockMapModel>()
                .ForMember(x => x.CreatedByUser,
                    opt => opt.MapFrom(x => x.CreatedByUser != null ? x.CreatedByUser.UserName : ""))
                .ForMember(x => x.OperationName, opt => opt.MapFrom(x => x.Master != null ? x.Master.Name : ""));
            configuration.CreateMap<BlockMapModel, BlockMapModel>();
        }
    }
}
