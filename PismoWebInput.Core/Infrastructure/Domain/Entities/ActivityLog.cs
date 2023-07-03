using System.ComponentModel.DataAnnotations.Schema;
using PismoWebInput.Core.Infrastructure.Domain.Common;

namespace PismoWebInput.Core.Infrastructure.Domain.Entities;

public class ActivityLog : Entity<int>
{
    public string? UserId { get; set; }
    public string Action { get; set; }
    public string? Status { get; set; }
    public string? Detail { get; set; }
    public DateTime CreatedOn { get; set; }

    [ForeignKey("UserId")]
    public AppUser? User { get; set; }
}