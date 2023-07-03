using System.ComponentModel.DataAnnotations.Schema;
using PismoWebInput.Core.Enums;
using PismoWebInput.Core.Infrastructure.Domain.Common;

namespace PismoWebInput.Core.Infrastructure.Domain.Entities
{
    public class BlockMap : AuditedEntity<long>
    {
        public string? IndicationId { get; set; }
        public long? OperationId { get; set; }
        public string? BlockId { get; set; }
        public BlockType? BlockType { get; set; }
        public DateTime? DateInput { get; set; }
        public int? Status { get; set; }
        public int? Order { get; set; }
        public string? EMapId { get; set; }
        public int? Sided { get; set; }
        public int? ErrorCount { get; set; }

        [ForeignKey("OperationId")]
        public Master? Master { get; set; }
        public ICollection<BlockMapDetail> Details { get; set; }
        [ForeignKey("CreatedBy")]
        public AppUser CreatedByUser { get; set; }
    }
}
