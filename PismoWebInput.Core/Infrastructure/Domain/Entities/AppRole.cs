using Microsoft.AspNetCore.Identity;

namespace PismoWebInput.Core.Infrastructure.Domain.Entities
{
    public class AppRole : IdentityRole
    {
        public int? Category { get; set; }
        public int? Status { get; set; }
        public int? Order { get; set; }
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}
