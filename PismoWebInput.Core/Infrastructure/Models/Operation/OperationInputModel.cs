using AutoMapper;
using PismoWebInput.Core.Infrastructure.Common.Mappings;
using PismoWebInput.Core.Infrastructure.Domain.Entities;
using PismoWebInput.Core.Infrastructure.Models.BlockMap;

namespace PismoWebInput.Core.Infrastructure.Models.Operation
{
    public class OperationInputModel : ICustomMappings
    {
        public string? Operation { get; set; }
        public long? OperatorId { get; set; }
        public string? IndicationId { get; set; }
        public string? BlockId { get; set; }
        public int? PatternNo { get; set; }
        public string? DefectCode { get; set; }
        public string? DefectName { get; set; }
        public int TotalWsQty { get; set; }
        public int OkPcsQty { get; set; }
        public int NgPcsQty { get; set; }
        public int? Sided { get; set; }
        public string? CodeError { get; set; }
        public bool EnterHit { get; set; }
        public bool DisableAll { get; set; }
        public string? SummaryError { get; set; }
        public List<BlockMapModel> A_Blocks { get; set; }
        public List<BlockMapModel> B_Blocks { get; set; }

        public OperationInputModel()
        {
            A_Blocks = new List<BlockMapModel>();
            B_Blocks = new List<BlockMapModel>();
            Sided = 1;
        }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<OperationInputModel, OperationInputModel>();
        }
    }
}
