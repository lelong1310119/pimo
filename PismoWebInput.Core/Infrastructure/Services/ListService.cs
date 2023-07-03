using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PismoWebInput.Core.Enums;
using PismoWebInput.Core.Infrastructure.Common.Mappings;
using PismoWebInput.Core.Infrastructure.Domain.Entities;
using PismoWebInput.Core.Infrastructure.Models.Master;
using PismoWebInput.Core.Persistence.Uow;

namespace PismoWebInput.Core.Infrastructure.Services
{
    public interface IListService
    {
        Task<List<List<MasterModel>>> GetAllOperators();
        Task<MasterModel?> GetErrorCode(string code);
        Task<MasterModel?> GetErrorCode(long id);
        Task<IList<MasterModel>> GetAllErrorCode();
    }

    public class ListService : IListService
    {
        private readonly IEfUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;
        private DbSet<Master> MasterSet => _unitOfWork.Set<Master>();
        private readonly UserManager<AppUser> _userManager;
        private DbSet<UserOperation> UserOperationSet => _unitOfWork.Set<UserOperation>();

        public ListService(
            IEfUnitOfWork unitOfWork,
            UserManager<AppUser> userManager,
            ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _currentUser = currentUser;
        }

        public async Task<List<List<MasterModel>>> GetAllOperators()
        {
            var userId = _currentUser.UserId;
            var user = await _userManager.FindByIdAsync(userId);
            var result = user is { UserName: "superadmin" }
                ? (await MasterSet
                .Where(x => x.Type == MasterTypeEnum.Front || x.Type == MasterTypeEnum.Back || x.Type == MasterTypeEnum.QC)
                .ToListAsync()).MapToList<MasterModel>().GroupBy(x => x.Type).Select(x => x.ToList()).ToList()
                : (await UserOperationSet
                    .Where(x => x.UserId == userId)
                    .Include(x => x.Master)
                    .Select(x => x.Master)
                    .ToListAsync()).MapToList<MasterModel>().GroupBy(x => x.Type).Select(x => x.ToList()).ToList();

            return result;
        }

        public async Task<IList<MasterModel>> GetAllErrorCode()
        {
            var result = await MasterSet.Where(x => x.Type == MasterTypeEnum.ErrorCode).ToListAsync();
            return result.MapToList<MasterModel>();
        }

        public async Task<MasterModel?> GetErrorCode(string code)
        {
            var entity = await MasterSet.FirstOrDefaultAsync(x => x.Code != null &&
                x.Code.ToLower() == code.ToLower() && x.Type == MasterTypeEnum.ErrorCode);
            if (entity == null) return null;

            return entity.MapTo<MasterModel>();
        }

        public async Task<MasterModel?> GetErrorCode(long id)
        {
            var entity = await MasterSet.FirstOrDefaultAsync(x => x.Id == id && x.Type == MasterTypeEnum.ErrorCode);
            if (entity == null) return null;

            return entity.MapTo<MasterModel>();
        }
    }
}
