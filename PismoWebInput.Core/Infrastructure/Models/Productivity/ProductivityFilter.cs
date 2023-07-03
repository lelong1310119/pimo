using PismoWebInput.Core.Enums;

namespace PismoWebInput.Core.Infrastructure.Models.Productivity
{
    public class ProductivityFilter
    {
        public string? OperatorId { get; set; }
        public long? OperationId { get; set; }
        public string? OperationName { get; set; }
        public MasterTypeEnum? Type { get; set; }
        public DateTime? Date { get; set; }
        public string? Shift { get; set; }
    }
}
