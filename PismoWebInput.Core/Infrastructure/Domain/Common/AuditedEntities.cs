namespace PismoWebInput.Core.Infrastructure.Domain.Common;

public interface ITimeAudited
{
    DateTime CreatedOn { get; set; }
    DateTime? ModifiedOn { get; set; }
}

public interface IAudited : ITimeAudited
{
    string? CreatedBy { get; set; }
    string? ModifiedBy { get; set; }
}

public interface ICreationAudited
{
    public DateTime CreatedOn { get; set; }
    string? CreatedBy { get; set; }
}

public abstract class TimeAuditedEntity<T> : Entity<T>, ITimeAudited
{
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
}

public abstract class AuditedEntity<T> : Entity<T>, IAudited
{
    public DateTime CreatedOn { get; set; }
    public string? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }
    public string? ModifiedBy { get; set; }
}