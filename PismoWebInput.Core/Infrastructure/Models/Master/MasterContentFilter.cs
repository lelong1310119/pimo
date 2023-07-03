using PismoWebInput.Core.Enums;

namespace PismoWebInput.Core.Infrastructure.Models.Master
{
    public class MasterContentFilter
    {
        public MasterTypeEnum? Type { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
    }
}
