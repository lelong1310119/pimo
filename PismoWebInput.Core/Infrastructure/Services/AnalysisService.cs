using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PismoWebInput.Core.Enums;
using PismoWebInput.Core.Infrastructure.Common.Extensions;
using PismoWebInput.Core.Infrastructure.Common.Mappings;
using PismoWebInput.Core.Infrastructure.Domain.Entities;
using PismoWebInput.Core.Infrastructure.Models.Analysis;
using PismoWebInput.Core.Infrastructure.Models.BlockMap;
using PismoWebInput.Core.Infrastructure.Models.DataTrace;
using PismoWebInput.Core.Persistence.Uow;

namespace PismoWebInput.Core.Infrastructure.Services
{
    public interface IAnalysisService
    {
        Task<HeatMapResult> GetData(AnalysisFilter filter);
    }

    public class AnalysisService : IAnalysisService
    {
        private readonly IEfUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;
        private readonly UserManager<AppUser> _userManager;
        private readonly IListService _listService;
        private DbSet<BlockMap> BlockMapSet => _unitOfWork.Set<BlockMap>();

        public AnalysisService(
            IEfUnitOfWork unitOfWork,
            ICurrentUserService currentUser,
            UserManager<AppUser> userManager,
            IListService listService)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _userManager = userManager;
            _listService = listService;
        }

        public async Task<HeatMapResult> GetData(AnalysisFilter filter)
        {
            var currentUser = await _userManager.Users
                .Where(x => x.Id == _currentUser.UserId)
                .Include(x => x.UserOperations)
                .FirstOrDefaultAsync();

            if (currentUser == null)
            {
                throw new Exception("The Operator ID does not exist");
            }

            var allErrorCode = await _listService.GetAllErrorCode();
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, AppRoleEnum.Admin.ToString());
            var userOperations = currentUser.UserOperations.Select(x => x.OperationId).ToList();

            var listBlocks = await BlockMapSet
                .Where(x => currentUser.UserName == "superadmin" || x.OperationId.HasValue && userOperations.Contains(x.OperationId.Value))
                .WhereIf(!isAdmin, x => x.CreatedBy == currentUser.Id)
                .WhereIf(!string.IsNullOrEmpty(filter.IndicationId),
                    x => x.IndicationId != null && x.IndicationId.ToLower() == filter.IndicationId.ToLower())
                .WhereIf(filter.Type.HasValue, x => x.Master != null && x.Master.Type == filter.Type.Value)
                .WhereIf(filter.OperationId.HasValue, x => x.OperationId != null && x.OperationId == filter.OperationId)
                .Include(x => x.Master)
                .Include(x => x.Details)
                .ToListAsync();

            var processGroup = listBlocks.Where(x => x.Master != null).GroupBy(x => x.Master.Name).ToList();
            var result = new HeatMapResult
            {
                Labels = allErrorCode.Select(x => x.Name).ToList(),
            };

            foreach (var processItem in processGroup)
            {
                var listDetails = processItem.ToList()
                    .SelectMany(x => x.Details)
                    .Where(x => x.DefectId.HasValue)
                    .OrderBy(x => x.DefectId)
                    .GroupBy(x => x.DefectId)
                    .ToList();

                result.Data.Add(new HeatMapItem
                {
                    label = processItem.Key,
                    data = allErrorCode.Select(x => listDetails.FirstOrDefault(a => a.Key == x.Id)?.Count() ?? 0).ToList()
                });
            }

            return result;
        }

    }
}
