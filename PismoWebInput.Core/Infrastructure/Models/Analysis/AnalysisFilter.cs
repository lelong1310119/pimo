using PismoWebInput.Core.Enums;

namespace PismoWebInput.Core.Infrastructure.Models.Analysis
{
    public class AnalysisFilter
    {
        public string? OperatorId { get; set; }
        public string? IndicationId { get; set; }
        public MasterTypeEnum? Type { get; set; }
        public long? OperationId { get; set; }
        public string? OperationName { get; set; }
    }
}
