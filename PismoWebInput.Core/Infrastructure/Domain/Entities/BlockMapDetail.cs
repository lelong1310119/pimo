using System.ComponentModel.DataAnnotations.Schema;
using PismoWebInput.Core.Infrastructure.Domain.Common;

namespace PismoWebInput.Core.Infrastructure.Domain.Entities
{
    public class BlockMapDetail : AuditedEntity<long>
    {
        public long BlockMapId { get; set; }
        public int? Location { get; set; }
        public long? DefectId { get; set; }
        public string? PcsId { get; set; }
        public DateTime? PcsIdUpdateTime { get; set; }
        public int? Status { get; set; }
        public int? Order { get; set; }

        [ForeignKey("BlockMapId")]
        public BlockMap BlockMap { get; set; }
        [ForeignKey("DefectId")]
        public Master? Master { get; set; }
    }
}
