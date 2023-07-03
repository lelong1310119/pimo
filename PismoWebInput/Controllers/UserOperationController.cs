using Microsoft.AspNetCore.Mvc;
using PismoWebInput.Core.Enums;
using PismoWebInput.Core.Infrastructure.Common.Mappings;
using PismoWebInput.Core.Infrastructure.Extensions;
using PismoWebInput.Core.Infrastructure.Models.BlockMap;
using PismoWebInput.Core.Infrastructure.Models.Master;
using PismoWebInput.Core.Infrastructure.Models.Operation;
using PismoWebInput.Core.Infrastructure.Services;

namespace PismoWebInput.Controllers
{
    public static class UserOperationSessionKeys
    {
        public static string InputModelKey = "OperationInputModel";
        //public static string FinalResultKey = "OperationFinalModel";
    }

    public class UserOperationController : Controller
    {
        private readonly IListService _listService;
        private readonly IUserOperationService _service;

        public UserOperationController(
            IListService listService,
            IUserOperationService service
            )
        {
            _listService = listService;
            _service = service;
        }

        public async Task<ActionResult> Index(long? id, string? name, string? type)
        {
            ViewData["Operators"] = await _listService.GetAllOperators();

            return View(id.HasValue ? new MasterModel { Id = id.Value, Name = name, Type = Enum<MasterTypeEnum>.Parse(type) } : null);
        }

        [HttpPost]
        public ActionResult Index([FromForm] MasterModel model)
        {
            if (model.Id != null)
            {
                return RedirectToAction("Details", new { operationId = model.Id });
            }

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Details(long operationId)
        {
            var currentDataModel = HttpContext.Session.GetObjectFromJson<OperationInputModel?>(UserOperationSessionKeys.InputModelKey);
            var data = await _service.GenerateOperation(operationId);

            if (currentDataModel == null || currentDataModel.Operation != data.Operation)
            {
                HttpContext.Session.Remove(UserOperationSessionKeys.InputModelKey);
                HttpContext.Session.SetObjectAsJson(UserOperationSessionKeys.InputModelKey, data);

                return View(data);
            }

            return View(currentDataModel);
        }

        public ActionResult ClearPattern()
        {
            var currentDataModel = HttpContext.Session.GetObjectFromJson<OperationInputModel>(UserOperationSessionKeys.InputModelKey);
            var activeBlocks = currentDataModel.Sided is null or 1 ? currentDataModel.A_Blocks : currentDataModel.B_Blocks;
            var listPatterns = activeBlocks.SelectMany(x => x.Details).ToList();

            currentDataModel.DefectCode = null;
            currentDataModel.DefectName = null;
            currentDataModel.CodeError = null;

            var currentPattern = listPatterns.FirstOrDefault(x => x.Location == currentDataModel.PatternNo);
            if (currentPattern != null)
            {
                currentPattern.DefectId = null;
                currentPattern.DefectName = null;
                currentPattern.DefectCode = null;
            }

            HttpContext.Session.SetObjectAsJson(UserOperationSessionKeys.InputModelKey, currentDataModel);

            return View("Details", currentDataModel);
        }

        public async Task<ActionResult> SelectPattern(int? nextLocation)
        {
            var currentDataModel = HttpContext.Session.GetObjectFromJson<OperationInputModel>(UserOperationSessionKeys.InputModelKey);
            var activeBlocks = currentDataModel.Sided is null or 1 ? currentDataModel.A_Blocks : currentDataModel.B_Blocks;
            var listPatterns = activeBlocks.SelectMany(x => x.Details).ToList();
            var defectCode = currentDataModel.DefectCode;
            currentDataModel.CodeError = null;
            currentDataModel.DefectName = null;

            var currentPattern = listPatterns.FirstOrDefault(x => x.Location == currentDataModel.PatternNo);
            if (currentPattern != null)
            {
                if (!string.IsNullOrEmpty(defectCode))
                {
                    var errorCode = await _listService.GetErrorCode(defectCode);
                    currentPattern.DefectId = errorCode?.Id;
                    currentPattern.DefectName = errorCode?.Name;
                }
                else
                {
                    currentPattern.DefectId = null;
                }
            }

            currentDataModel.PatternNo = nextLocation ?? (currentDataModel.PatternNo.HasValue ? currentDataModel.PatternNo + 1 : 1);
            if (currentDataModel.PatternNo > 144) currentDataModel.PatternNo = 1;
            // Set new selection 
            var selectedPattern = listPatterns.FirstOrDefault(x => x.Location == currentDataModel.PatternNo);
            if (selectedPattern != null)
            {
                if (selectedPattern.DefectId.HasValue)
                {
                    var errorCode = await _listService.GetErrorCode(selectedPattern.DefectId.Value);
                    if (errorCode != null)
                    {
                        currentDataModel.DefectCode = errorCode.Code;
                        currentDataModel.DefectName = errorCode.Name;
                    }
                }
                else
                {
                    currentDataModel.DefectCode = null;
                    currentDataModel.DefectName = null;
                }
            }
            else
            {
                currentDataModel.DefectCode = null;
                currentDataModel.DefectName = null;
                currentDataModel.CodeError = null;
            }

            if (currentDataModel.PatternNo == 0)
            {
                currentDataModel.PatternNo = null;
            }

            HttpContext.Session.SetObjectAsJson(UserOperationSessionKeys.InputModelKey, currentDataModel);

            return View("Details", currentDataModel);
        }

        public async Task<ActionResult> SaveBlock()
        {
            var currentDataModel = HttpContext.Session.GetObjectFromJson<OperationInputModel>(UserOperationSessionKeys.InputModelKey);
            try
            {
                if (!currentDataModel.DisableAll)
                {
                    await _service.SaveLot(currentDataModel);
                }
                var data = await _service.GenerateOperation(currentDataModel.OperatorId.Value);
                data.IndicationId = currentDataModel.IndicationId;
                data.TotalWsQty += currentDataModel.TotalWsQty;
                data.OkPcsQty += currentDataModel.OkPcsQty + currentDataModel.A_Blocks.Sum(x => x.Details.Count(d => !d.DefectId.HasValue)) + currentDataModel.B_Blocks.Sum(x => x.Details.Count(d => !d.DefectId.HasValue));
                data.NgPcsQty += currentDataModel.NgPcsQty + currentDataModel.A_Blocks.Sum(x => x.Details.Count(d => d.DefectId.HasValue)) + currentDataModel.B_Blocks.Sum(x => x.Details.Count(d => d.DefectId.HasValue));

                HttpContext.Session.SetObjectAsJson(UserOperationSessionKeys.InputModelKey, data);
                return View("Details", data);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return View("Details", currentDataModel);
        }

        public async Task<ActionResult> FinishLot()
        {
            var currentDataModel = HttpContext.Session.GetObjectFromJson<OperationInputModel>(UserOperationSessionKeys.InputModelKey);

            try
            {
                if (!string.IsNullOrEmpty(currentDataModel.BlockId) && !currentDataModel.DisableAll)
                {
                    await _service.SaveLot(currentDataModel);
                }

                var data = await _service.GenerateOperation(currentDataModel.OperatorId.Value);
                HttpContext.Session.SetObjectAsJson(UserOperationSessionKeys.InputModelKey, data);

                return View("Details", data);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return View("Details", currentDataModel);
        }

        public async Task<ActionResult> ResetLot()
        {
            var currentDataModel = HttpContext.Session.GetObjectFromJson<OperationInputModel>(UserOperationSessionKeys.InputModelKey);

            try
            {
                var data = await _service.GenerateOperation(currentDataModel.OperatorId.Value);
                HttpContext.Session.SetObjectAsJson(UserOperationSessionKeys.InputModelKey, data);

                return RedirectToAction("Details", new { operationId = currentDataModel.OperatorId.Value });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return View("Details", currentDataModel);
        }

        public async Task<ActionResult> ResetBlock()
        {
            var currentDataModel = HttpContext.Session.GetObjectFromJson<OperationInputModel>(UserOperationSessionKeys.InputModelKey);

            try
            {
                var data = await _service.GenerateOperation(currentDataModel.OperatorId.Value);
                data.IndicationId = currentDataModel.IndicationId;
                HttpContext.Session.SetObjectAsJson(UserOperationSessionKeys.InputModelKey, data);

                return RedirectToAction("Details", new { operationId = currentDataModel.OperatorId.Value });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return View("Details", currentDataModel);
        }

        public async Task<ActionResult> Back()
        {
            var currentDataModel = HttpContext.Session.GetObjectFromJson<OperationInputModel>(UserOperationSessionKeys.InputModelKey);

            try
            {
                if (!string.IsNullOrEmpty(currentDataModel.BlockId) && !currentDataModel.DisableAll)
                {
                    await _service.SaveLot(currentDataModel);
                }

                HttpContext.Session.Remove(UserOperationSessionKeys.InputModelKey);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return View("Details", currentDataModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateViewData(OperationInputModel model)
        {
            ModelState.Clear();
            var currentDataModel = HttpContext.Session.GetObjectFromJson<OperationInputModel>(UserOperationSessionKeys.InputModelKey);
            try
            {
                currentDataModel.PatternNo = model.PatternNo;
                currentDataModel.CodeError = null;

                if (string.IsNullOrEmpty(currentDataModel.IndicationId) && currentDataModel.IndicationId != model.IndicationId)
                {
                    currentDataModel.IndicationId = model.IndicationId;
                    var existingData = await _service.GetData(currentDataModel.OperatorId, currentDataModel.IndicationId);
                    if (existingData != null)
                    {
                        currentDataModel.SummaryError =
                            "The IndicationId already exists. Please Finish Lot to input a new one";
                        currentDataModel.A_Blocks = existingData.Where(x => x.Sided == 1).OrderBy(x => x.BlockId).ToList();
                        currentDataModel.B_Blocks = existingData.Where(x => x.Sided == 2).OrderBy(x => x.BlockId).ToList();
                        currentDataModel.BlockId = currentDataModel.A_Blocks.FirstOrDefault()?.BlockId;
                        currentDataModel.DisableAll = true;
                    }
                }
                else
                {
                    // Process Data when BlockId changed
                    if (string.IsNullOrEmpty(currentDataModel.BlockId) && currentDataModel.BlockId != model.BlockId)
                    {
                        currentDataModel.BlockId = model.BlockId;
                        var existingData = await _service.GetData(currentDataModel.OperatorId, currentDataModel.IndicationId, currentDataModel.BlockId);
                        if (existingData != null)
                        {
                            currentDataModel.SummaryError = "The BlockId already exists.";
                            currentDataModel.A_Blocks = existingData.Where(x => x.Sided == 1).OrderBy(x => x.BlockId).ToList();
                            currentDataModel.B_Blocks = existingData.Where(x => x.Sided == 2).OrderBy(x => x.BlockId).ToList();
                            currentDataModel.BlockId = currentDataModel.A_Blocks.FirstOrDefault()?.BlockId;
                            currentDataModel.DisableAll = true;
                        }
                        else
                        {
                            GenerateBlocks(currentDataModel, currentDataModel.A_Blocks);
                            GenerateBlocks(currentDataModel, currentDataModel.B_Blocks);
                        }
                    }

                    // Handle DefectCode changed
                    if (currentDataModel.DefectCode != model.DefectCode)
                    {
                        currentDataModel.DefectCode = model.DefectCode;
                        if (!string.IsNullOrEmpty(currentDataModel.DefectCode))
                        {
                            var errorCode = await _listService.GetErrorCode(currentDataModel.DefectCode);
                            if (errorCode != null)
                            {
                                currentDataModel.DefectName = errorCode.Name;
                            }
                            else
                            {
                                currentDataModel.CodeError = $"No defect code ({currentDataModel.DefectCode}) found";
                                currentDataModel.DefectName = null;
                            }
                        }
                        else
                        {
                            currentDataModel.DefectName = null;
                        }
                    }

                    // Handle Side changed
                    if (currentDataModel.Sided != model.Sided)
                    {
                        currentDataModel.Sided = model.Sided;
                    }
                }

                HttpContext.Session.SetObjectAsJson(UserOperationSessionKeys.InputModelKey, currentDataModel);

                if (model.EnterHit && string.IsNullOrEmpty(currentDataModel.CodeError))
                {
                    await SelectPattern(0);
                }
                currentDataModel = HttpContext.Session.GetObjectFromJson<OperationInputModel>(UserOperationSessionKeys.InputModelKey);

                return PartialView("_DetailPartial", currentDataModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return PartialView("_DetailPartial", currentDataModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetErrorCode(string code)
        {
            try
            {
                var data = await _listService.GetErrorCode(code);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private void GenerateBlocks(OperationInputModel currentDataModel, List<BlockMapModel> activeBlocks)
        {
            if (!string.IsNullOrEmpty(currentDataModel.BlockId))
            {
                var blockId = currentDataModel.BlockId.Trim();
                var lastNumberChar = blockId.LastOrDefault().ToString();
                var blockIdProcessed = blockId.Remove(blockId.Length - 1);
                var lastCharNumberValid = int.TryParse(lastNumberChar, out var startBlockNumber);
                int blockNumberCount = 1;
                var pcIdCount = 1;
                if (lastCharNumberValid)
                {
                    if (startBlockNumber == 0 || startBlockNumber > activeBlocks.Count)
                    {
                        blockIdProcessed = blockId;
                        lastCharNumberValid = false;
                    }
                }
                else
                {
                    blockIdProcessed = blockId;
                }

                foreach (var blockMap in activeBlocks)
                {
                    blockMap.BlockId = $"{blockIdProcessed}{(!lastCharNumberValid ? $" - {blockNumberCount}" : blockNumberCount)}";
                    foreach (var blockMapDetail in blockMap.Details)
                    {
                        blockMapDetail.Location = pcIdCount;
                        pcIdCount++;
                    }

                    blockNumberCount++;
                }

                currentDataModel.PatternNo = null;
            }
            else
            {
                foreach (var blockMap in activeBlocks)
                {
                    blockMap.BlockId = null;
                    foreach (var blockMapDetail in blockMap.Details)
                    {
                        blockMapDetail.Location = null;
                    }
                }

                currentDataModel.PatternNo = null;
            }
        }
    }
}
