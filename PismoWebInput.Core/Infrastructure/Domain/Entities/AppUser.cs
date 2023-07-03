using Microsoft.AspNetCore.Identity;
using PismoWebInput.Core.Enums;
using PismoWebInput.Core.Infrastructure.Domain.Common;

namespace PismoWebInput.Core.Infrastructure.Domain.Entities
{
    public class AppUser : IdentityUser, ITimeAudited, IAudited
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public UserStatusEnum? Status { get; set; }

        public ICollection<UserOperation> UserOperations { get; set; }
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}
