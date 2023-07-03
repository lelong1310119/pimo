using Microsoft.EntityFrameworkCore;
using PismoWebInput.Core.Infrastructure.Common.Exceptions;
using PismoWebInput.Core.Infrastructure.Common.Extensions;
using PismoWebInput.Core.Infrastructure.Common.Mappings;
using PismoWebInput.Core.Infrastructure.Domain.Entities;
using PismoWebInput.Core.Infrastructure.Models.Master;
using PismoWebInput.Core.Infrastructure.Models.StatusPicking;
using PismoWebInput.Core.Persistence.Uow;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PismoWebInput.Core.Infrastructure.Services
{
    public interface IPickingStatusService
    {
        Task<IList<PickingStatusModel>> GetAll();
        Task<IList<PickingStatusModel>> GetPage(int page, int size);
        Task Create(PickingStatusModel model);
        Task Delete(long id);
        Task<PickingStatusModel> GetPickingStatus(long id);
        Task UpdatePickingStatus(PickingStatusModel model);
        Task<int> GetTotalCount();
        Task<int> GetTotalFilter(PickingStatusFilter filter);
        Task<IList<PickingStatusModel>> GetAllPickingStatus(PickingStatusFilter filter, int page, int size);
    }

    public class PickingStatusService : IPickingStatusService
    {
        private readonly IEfUnitOfWork _unitOfWork;
        private DbSet<PickingStatus> _pickingStatusSet => _unitOfWork.Set<PickingStatus>();
        public PickingStatusService(IEfUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IList<PickingStatusModel>> GetAll()
        {
            return (await _pickingStatusSet.ToListAsync()).MapToList<PickingStatusModel>();
        }

        public async Task<IList<PickingStatusModel>> GetPage(int page, int size)
        {
            int skipAmount = size * (page - 1);
            return await _pickingStatusSet
                .OrderBy(p => p.Id)
                .Skip(skipAmount)
                .Take(size)
                .Select(p => p.MapTo<PickingStatusModel>())
                .ToListAsync();
        }
        public async Task<int> GetTotalFilter(PickingStatusFilter filter)
        {
            return (await _pickingStatusSet
                .WhereIf(filter.PickingStatusID.HasValue, x => x.PickingStatusID == filter.PickingStatusID)
                .WhereIf(!string.IsNullOrEmpty(filter.PickingInstructionNo), x => filter.PickingInstructionNo != null && x.PickingInstructionNo != null && x.PickingInstructionNo.ToLower().Contains(filter.PickingInstructionNo.ToLower()))
                .WhereIf(!string.IsNullOrEmpty(filter.MFInstructionNo), x => filter.MFInstructionNo != null && x.MFInstructionNo != null && x.MFInstructionNo.ToLower().Contains(filter.MFInstructionNo.ToLower()))
                .CountAsync());
        }

        public async Task<IList<PickingStatusModel>> GetAllPickingStatus(PickingStatusFilter filter, int page, int size)
        {
            var pickingStatusQuery = _pickingStatusSet
                .WhereIf(filter.PickingStatusID.HasValue, x => x.PickingStatusID == filter.PickingStatusID)
                .WhereIf(!string.IsNullOrEmpty(filter.PickingInstructionNo), x => filter.PickingInstructionNo != null && x.PickingInstructionNo != null && x.PickingInstructionNo.ToLower().Contains(filter.PickingInstructionNo.ToLower()))
                .WhereIf(!string.IsNullOrEmpty(filter.MFInstructionNo), x => filter.MFInstructionNo != null && x.MFInstructionNo != null && x.MFInstructionNo.ToLower().Contains(filter.MFInstructionNo.ToLower()));
            
            int skipAmount = size * (page - 1);
            var total = await pickingStatusQuery.CountAsync();
            return await pickingStatusQuery
                .OrderBy(x => x.Id)
                .Skip(skipAmount)
                .Take(size)
                .Select(p => p.MapTo<PickingStatusModel>())
                .ToListAsync();
                
        }

        public async Task<PickingStatusModel> GetPickingStatus(long id)
        {
            var entity = await _pickingStatusSet.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
            {
                throw new NotFoundException();
            }

            return entity.MapTo<PickingStatusModel>();
        }

        public async Task Create(PickingStatusModel model)
        {
            var entity = model.MapTo<PickingStatus>();
            var existing = await _pickingStatusSet.FirstOrDefaultAsync(x => x.PickingStatusID == model.PickingStatusID);
            if (existing != null)
            {
                throw new Exception("Content already exist!");
            }
            _pickingStatusSet.Add(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> GetTotalCount()
        {
            return await _pickingStatusSet.CountAsync();
        }

        public async Task UpdatePickingStatus(PickingStatusModel model)
        {
            var entity = await _pickingStatusSet.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (entity == null)
            {
                throw new NotFoundException();
            }

            if (entity.PickingStatusID != model.PickingStatusID)
            {
                var existing = await _pickingStatusSet.FirstOrDefaultAsync(x => x.PickingStatusID == model.PickingStatusID);
                if (existing != null)
                {
                    throw new Exception("Content already exist!");
                }
            }

            model.MapTo(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task Delete(long id)
        {
            var entity = await _pickingStatusSet.FirstOrDefaultAsync(x => x.Id == id);  
            if (entity == null)
            {
                throw new NotFoundException();
            }

            _pickingStatusSet.Remove(entity);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
