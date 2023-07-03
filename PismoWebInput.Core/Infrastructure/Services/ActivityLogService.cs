using Microsoft.EntityFrameworkCore;
using PismoWebInput.Core.Infrastructure.Common.Mappings;
using PismoWebInput.Core.Infrastructure.Domain.Entities;
using PismoWebInput.Core.Infrastructure.Models.Log;
using PismoWebInput.Core.Persistence.Uow;

namespace PismoWebInput.Core.Infrastructure.Services;

public interface IActivityLogService
{
    Task<IList<ActivityLogModel>> GetList();
    Task<ActivityLog> Create(ActivityLog model);
}

public class ActivityLogService : IActivityLogService
{
    public ActivityLogService(IEfUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    private readonly IEfUnitOfWork _unitOfWork;
    private DbSet<ActivityLog> LogSet => _unitOfWork.Set<ActivityLog>();

    public async Task<IList<ActivityLogModel>> GetList()
    {
        var list = await LogSet.AsNoTracking()
            .ProjectTo<ActivityLogModel>()
            .ToListAsync();
        return list;
    }

    public async Task<ActivityLog> Create(ActivityLog model)
    {
        await LogSet.AddAsync(model);
        await _unitOfWork.SaveChangesAsync();
        return model;
    }
}
