using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PismoWebInput.Core.Enums;
using PismoWebInput.Core.Infrastructure.Common.Exceptions;
using PismoWebInput.Core.Infrastructure.Common.Extensions;
using PismoWebInput.Core.Infrastructure.Common.Mappings;
using PismoWebInput.Core.Infrastructure.Domain.Entities;
using PismoWebInput.Core.Infrastructure.Models.BlockMap;
using PismoWebInput.Core.Infrastructure.Models.DataTrace;
using PismoWebInput.Core.Persistence.Uow;

namespace PismoWebInput.Core.Infrastructure.Services
{
    public interface IDataTraceService
    {
        Task<IList<BlockMapModel>> GetBLocks(DataTraceFilter filter);
        Task<BlockMapModel> GetBlock(long id);
        Task UpdatePcDetail(BlockMapDetailModel model);
    }

    public class DataTraceService : IDataTraceService
    {
        private readonly IEfUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;
        private readonly UserManager<AppUser> _userManager;
        private DbSet<BlockMap> BlockMapSet => _unitOfWork.Set<BlockMap>();
        private DbSet<BlockMapDetail> BlockMapDetailSet => _unitOfWork.Set<BlockMapDetail>();
        private DbSet<Master> MasterSet => _unitOfWork.Set<Master>();

        public DataTraceService(
            IEfUnitOfWork unitOfWork,
            ICurrentUserService currentUser,
            UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _userManager = userManager;
        }

        public async Task<IList<BlockMapModel>> GetBLocks(DataTraceFilter filter)
        {
            var currentUser = await _userManager.Users
                .Where(x => x.Id == _currentUser.UserId)
                .Include(x => x.UserOperations)
                .FirstOrDefaultAsync();

            if (currentUser == null)
            {
                throw new Exception("The Operator ID does not exist");
            }

            var isAdmin = await _userManager.IsInRoleAsync(currentUser, AppRoleEnum.Admin.ToString());
            var userOperations = currentUser.UserOperations.Select(x => x.OperationId).ToList();

            var result = await BlockMapSet
                .Where(x => x.ErrorCount > 0)
                .Where(x => currentUser.UserName == "superadmin" || x.OperationId.HasValue && userOperations.Contains(x.OperationId.Value))
                .WhereIf(!isAdmin, x => x.CreatedBy == currentUser.Id)
                .WhereIf(!string.IsNullOrEmpty(filter.IndicationId),
                    x => x.IndicationId != null && x.IndicationId.ToLower() == filter.IndicationId.ToLower())
                .WhereIf(filter.OperationId.HasValue, x => x.OperationId != null && x.OperationId == filter.OperationId)
                .WhereIf(!string.IsNullOrEmpty(filter.BlockEmapID),
                    x => x.BlockId != null && x.BlockId.ToLower() == filter.BlockEmapID.ToLower())
                .ToListAsync();

            return result.MapToList<BlockMapModel>();
        }

        public async Task<BlockMapModel> GetBlock(long id)
        {
            var entity = await BlockMapSet
                .Where(x => x.Id == id)
                .Include(x => x.Details)
                .ThenInclude(x => x.Master)
                .FirstOrDefaultAsync();
            if (entity == null)
            {
                throw new NotFoundException();
            }

            return entity.MapTo<BlockMapModel>();
        }

        public async Task UpdatePcDetail(BlockMapDetailModel model)
        {
            var entity = await BlockMapDetailSet.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (entity == null)
            {
                throw new NotFoundException();
            }

            var errorCode = await MasterSet.FirstOrDefaultAsync(x => x.Code == model.DefectCode);
            if (errorCode == null)
            {
                throw new NotFoundException();
            }

            entity.DefectId = errorCode.Id;

            BlockMapDetailSet.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
