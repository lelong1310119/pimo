using PismoWebInput.Core.Enums;
using PismoWebInput.Core.Infrastructure.Domain.Common;

namespace PismoWebInput.Core.Infrastructure.Domain.Entities
{
    public class Master : Entity<long>
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public MasterTypeEnum? Type { get; set; }
        public string? Text1 { get; set; }
        public string? Text2 { get; set; }
        public int Num1 { get; set; }
        public int Num2 { get; set; }
        public int Status { get; set; }
        public int? Order { get; set; }

        public ICollection<UserOperation> UserOperations { get; set; }
        public ICollection<BlockMapDetail> BlockMapsDetails { get; set; }
        public ICollection<BlockMap> BlockMaps { get; set; }
    }
}
