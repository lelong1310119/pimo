using Microsoft.EntityFrameworkCore;
using PismoWebInput.Core.Infrastructure.Common.Exceptions;
using PismoWebInput.Core.Infrastructure.Common.Extensions;
using PismoWebInput.Core.Infrastructure.Common.Mappings;
using PismoWebInput.Core.Infrastructure.Domain.Entities;
using PismoWebInput.Core.Infrastructure.Models.Master;
using PismoWebInput.Core.Infrastructure.Models.StatusPicking;
using PismoWebInput.Core.Persistence.Uow;

namespace PismoWebInput.Core.Infrastructure.Services
{
    public interface IMasterService
    {
        Task<IList<MasterTypeModel>> GetAll();
        Task CreateMasterType(MasterTypeModel model);
        Task<IList<MasterModel>> GetAllMasterContent(MasterContentFilter filter);
        Task<MasterModel> GetMaster(long id);
        Task CreateMaster(MasterModel model);
        Task UpdateMaster(MasterModel model);
        Task Delete(long id);
        Task<MasterModel> GetMaster(string code);
    }

    public class MasterService : IMasterService
    {
        private readonly IEfUnitOfWork _unitOfWork;
        private DbSet<MasterType> MasterTypeSet => _unitOfWork.Set<MasterType>();
        private DbSet<Master> MasterSet => _unitOfWork.Set<Master>();

        public MasterService(IEfUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IList<MasterTypeModel>> GetAll()
        {
            return (await MasterTypeSet.ToListAsync()).MapToList<MasterTypeModel>();
        }

        public async Task<IList<MasterModel>> GetAllMasterContent(MasterContentFilter filter)
        {
            return (await MasterSet
                .WhereIf(filter.Type.HasValue, x => x.Type == filter.Type)
                .WhereIf(!string.IsNullOrEmpty(filter.Name), x => filter.Name != null && x.Name != null && x.Name.ToLower().Contains(filter.Name.ToLower()))
                .WhereIf(!string.IsNullOrEmpty(filter.Code), x => filter.Code != null && x.Code != null && x.Code.ToLower().Contains(filter.Code.ToLower()))
                .OrderBy(x => x.Type).ToListAsync()).MapToList<MasterModel>();
        }
        
        public async Task<MasterModel> GetMaster(long id)
        {
            var entity = await MasterSet.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
            {
                throw new NotFoundException();
            }

            return entity.MapTo<MasterModel>();
        }

        public async Task<MasterModel> GetMaster(string code)
        {
            var entity = await MasterSet.FirstOrDefaultAsync(x => x.Code == code);
            if (entity == null)
            {
                throw new NotFoundException($"No defect code ({code}) found");
            }

            return entity.MapTo<MasterModel>();
        }

        public async Task CreateMaster(MasterModel model)
        {
            var entity = model.MapTo<Master>();
            var existing = await MasterSet.FirstOrDefaultAsync(x => x.Name == model.Name || x.Code == model.Code);
            if (existing != null)
            {
                throw new Exception("Content already exist!");
            }
            MasterSet.Add(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateMaster(MasterModel model)
        {
            var entity = await MasterSet.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (entity == null)
            {
                throw new NotFoundException();
            }

            if (entity.Name != model.Name || entity.Code != model.Code)
            {
                var existing = await MasterSet.FirstOrDefaultAsync(x => x.Name == model.Name || x.Code == model.Code);
                if (existing != null)
                {
                    throw new Exception("Content already exist!");
                }
            }

            model.MapTo(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task CreateMasterType(MasterTypeModel model)
        {
            var entity = model.MapTo<MasterType>();
            MasterTypeSet.Add(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task Delete(long id)
        {
            var entity = await MasterSet.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
            {
                throw new NotFoundException();
            }

            MasterSet.Remove(entity);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
