using System.ComponentModel.DataAnnotations.Schema;
using PismoWebInput.Core.Infrastructure.Domain.Common;

namespace PismoWebInput.Core.Infrastructure.Domain.Entities
{
    public class UserOperation : EntityBase
    {
        public string UserId { get; set; }
        public long OperationId { get; set; }
        public string? Status { get; set; }
        public int? Order { get; set; }

        [ForeignKey("UserId")]
        public AppUser User { get; set; }

        [ForeignKey("OperationId")]
        public Master Master { get; set; }
    }
}
