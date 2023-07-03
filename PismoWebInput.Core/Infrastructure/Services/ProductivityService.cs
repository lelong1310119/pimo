using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PismoWebInput.Core.Enums;
using PismoWebInput.Core.Infrastructure.Common.Extensions;
using PismoWebInput.Core.Infrastructure.Common.Mappings;
using PismoWebInput.Core.Infrastructure.Domain.Entities;
using PismoWebInput.Core.Infrastructure.Models.BlockMap;
using PismoWebInput.Core.Infrastructure.Models.Productivity;
using PismoWebInput.Core.Persistence.Uow;

namespace PismoWebInput.Core.Infrastructure.Services
{
    public interface IProductivityService
    {
        Task<IList<BlockMapModel>> GetBLocks(ProductivityFilter filter);
    }

    public class ProductivityService : IProductivityService
    {
        private readonly IEfUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;
        private readonly UserManager<AppUser> _userManager;
        private DbSet<BlockMap> BlockMapSet => _unitOfWork.Set<BlockMap>();

        public ProductivityService(
            IEfUnitOfWork unitOfWork,
            ICurrentUserService currentUser,
            UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _userManager = userManager;
        }

        public async Task<IList<BlockMapModel>> GetBLocks(ProductivityFilter filter)
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
                .Where(x => currentUser.UserName == "superadmin" || x.OperationId.HasValue && userOperations.Contains(x.OperationId.Value))
                .WhereIf(!isAdmin, x => x.CreatedBy == currentUser.Id)
                .WhereIf(filter.OperationId.HasValue, x => x.OperationId != null && x.OperationId == filter.OperationId)
                .WhereIf(filter.Type.HasValue, x => x.Master != null && x.Master.Type == filter.Type.Value)
                .Include(x => x.Master)
                .Include(x => x.Details)
                .Include(x => x.CreatedByUser)
                .ToListAsync();

            return result.MapToList<BlockMapModel>();
        }
    }
}
