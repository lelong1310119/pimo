using PismoWebInput.Core.Infrastructure.Domain.Common;

namespace PismoWebInput.Core.Infrastructure.Domain.Entities
{
    public class MasterType : AuditedEntity<int>
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
