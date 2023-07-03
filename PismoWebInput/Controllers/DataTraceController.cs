using Microsoft.AspNetCore.Mvc;
using PismoWebInput.Core.Infrastructure.Models.BlockMap;
using PismoWebInput.Core.Infrastructure.Models.DataTrace;
using PismoWebInput.Core.Infrastructure.Services;

namespace PismoWebInput.Controllers
{
    public class DataTraceController : Controller
    {
        private readonly IListService _listService;
        private readonly IMasterService _masterService;
        private readonly IDataTraceService _service;

        public DataTraceController(
            IDataTraceService service,
            IListService listService,
            IMasterService masterService
            )
        {
            _listService = listService;
            _masterService = masterService;
            _service = service;
        }

        public async Task<ActionResult> Index()
        {
            try
            {
                ViewData["Operators"] = await _listService.GetAllOperators();
                ViewData["DataList"] = await _service.GetBLocks(new DataTraceFilter());
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index([FromForm] DataTraceFilter filter)
        {
            try
            {
                ViewData["Operators"] = await _listService.GetAllOperators();
                ViewData["DataList"] = await _service.GetBLocks(filter);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View(filter);
        }

        public async Task<ActionResult> Details(int id, int? location)
        {
            var block = await _service.GetBlock(id);

            if (location.HasValue)
            {
                ViewData["CellModel"] = block.Details.FirstOrDefault(x => x.Location == location);
            }
            return View(block);
        }

        [HttpPost]
        public async Task<IActionResult> Details(BlockMapDetailModel model)
        {
            await _service.UpdatePcDetail(model);

            return RedirectToAction("Details", new { id = model.BlockMapId, location = model.Location });
        }

        [HttpPost]
        public async Task<IActionResult> GetErrorCode(string code)
        {
            try
            {
                var data = await _masterService.GetMaster(code);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
