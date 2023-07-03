using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using PismoWebInput.Core.Enums;
using PismoWebInput.Core.Infrastructure.Common.Mappings;
using PismoWebInput.Core.Infrastructure.Domain.Entities;
using PismoWebInput.Core.Infrastructure.Models.BlockMap;
using PismoWebInput.Core.Infrastructure.Models.Operation;
using PismoWebInput.Core.Persistence.Uow;

namespace PismoWebInput.Core.Infrastructure.Services
{
    public interface IUserOperationService
    {
        Task<OperationInputModel> GenerateOperation(long Id);
        Task SaveLot(OperationInputModel model);
        Task<IList<BlockMapModel>?> GetData(long? operationId, string indicationId);
        Task<IList<BlockMapModel>?> GetData(long? operationId, string indicationId, string blockId);
    }

    public class UserOperationService : IUserOperationService
    {
        private readonly IEfUnitOfWork _unitOfWork;
        private DbSet<Master> MasterSet => _unitOfWork.Set<Master>();
        private DbSet<BlockMap> BlockMapSet => _unitOfWork.Set<BlockMap>();

        public UserOperationService(IEfUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OperationInputModel> GenerateOperation(long Id)
        {
            var result = new OperationInputModel();
            var operation = await MasterSet.Where(x => x.Id == Id).FirstOrDefaultAsync();
            if (operation == null)
            {
                throw new Exception("Operation does not exist");
            }

            result.OperatorId = operation.Id;
            result.Operation = operation.Name;

            // Generate PC144 (6 blocks - 24 patterns/block)
            if (operation.Type == MasterTypeEnum.Front)
            {
                for (var i = 1; i <= 6; i++)
                {
                    var block = new BlockMapModel();
                    // Generate 24 patterns
                    for (var j = 1; j <= 24; j++)
                    {
                        block.Details.Add(new BlockMapDetailModel());
                    }

                    result.A_Blocks.Add(block);
                    result.B_Blocks.Add(block);
                }
            }
            // Generate PC48 (2 blocks - 24 patterns/block)
            else if (operation.Type == MasterTypeEnum.Back)
            {
                for (var i = 1; i <= 2; i++)
                {
                    var block = new BlockMapModel();
                    // Generate 24 patterns
                    for (var j = 1; j <= 24; j++)
                    {
                        block.Details.Add(new BlockMapDetailModel());
                    }

                    result.A_Blocks.Add(block);
                    result.B_Blocks.Add(block);
                }
            }
            // Generate PC24 (1 blocks - 24 patterns/block)
            else if (operation.Type == MasterTypeEnum.QC)
            {
                var block = new BlockMapModel();
                // Generate 24 patterns
                for (var j = 1; j <= 24; j++)
                {
                    block.Details.Add(new BlockMapDetailModel());
                }

                result.A_Blocks.Add(block);
                result.B_Blocks.Add(block);
            }

            result.TotalWsQty = result.A_Blocks.Count;
            result.OkPcsQty = 0;
            result.NgPcsQty = 0;

            return result;
        }

        public async Task<IList<BlockMapModel>?> GetData(long? operationId, string indicationId)
        {
            var data = await BlockMapSet
                .Where(x =>
                    x.OperationId == operationId &&
                    x.IndicationId != null && x.IndicationId.ToLower() == indicationId.ToLower())
                .Include(x => x.Details)
                .Include(x => x.Master)
                .OrderBy(x => x.BlockId)
                .ToListAsync();

            if (data.Count == 0)
            {
                return null;
            }

            return data.MapToList<BlockMapModel>();
        }

        public async Task<IList<BlockMapModel>?> GetData(long? operationId, string indicationId, string blockId)
        {
            var data = await BlockMapSet
                .Where(x =>
                    x.OperationId == operationId &&
                    x.IndicationId != null && x.IndicationId.ToLower() == indicationId.ToLower() &&
                    x.BlockId != null && x.BlockId.ToLower() == blockId.ToLower())
                .Include(x => x.Details)
                .Include(x => x.Master)
                .OrderBy(x => x.BlockId)
                .ToListAsync();

            if (data.Count == 0)
            {
                return null;
            }

            return data.MapToList<BlockMapModel>();
        }

        public async Task SaveLot(OperationInputModel model)
        {
            var blocksToSave = new List<BlockMap>();
            var isPc24 = model.A_Blocks.Sum(x => x.Details.Count) == 24;

            var blocksSideA = model.A_Blocks.Select(x => new BlockMap
            {
                OperationId = model.OperatorId,
                IndicationId = model.IndicationId,
                BlockId = !isPc24 ? x.BlockId : null,
                EMapId = isPc24 ? x.BlockId : null,
                BlockType = isPc24 ? BlockType.EMapId : BlockType.BlockId,
                Sided = 1,
                ErrorCount = x.Details.Count(x => x.DefectId.HasValue),
                Details = x.Details.Select(a => new BlockMapDetail
                {
                    Location = a.Location,
                    DefectId = a.DefectId
                }).ToList()
            });
            var blocksSideB = model.B_Blocks.Select(x => new BlockMap
            {
                OperationId = model.OperatorId,
                IndicationId = model.IndicationId,
                BlockId = !isPc24 ? x.BlockId : null,
                EMapId = isPc24 ? x.BlockId : null,
                BlockType = isPc24 ? BlockType.EMapId : BlockType.BlockId,
                Sided = 2,
                ErrorCount = x.Details.Count(x => x.DefectId.HasValue),
                Details = x.Details.Select(a => new BlockMapDetail
                {
                    Location = a.Location,
                    DefectId = a.DefectId
                }).ToList()
            });

            blocksToSave.AddRange(blocksSideA);
            blocksToSave.AddRange(blocksSideB);

            BlockMapSet.AddRange(blocksToSave);
            await _unitOfWork.SaveChangesAsync();

            //await using var transaction = await _unitOfWork.DataContext.Database.BeginTransactionAsync();
            //await _unitOfWork.DataContext.BulkInsertAsync(blocksToSave);
            //await transaction.CommitAsync();
        }
    }
}
