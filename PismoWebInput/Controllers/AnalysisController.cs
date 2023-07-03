using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PismoWebInput.Core.Infrastructure.Models.Analysis;
using PismoWebInput.Core.Infrastructure.Services;

namespace PismoWebInput.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AnalysisController : Controller
    {
        private readonly IListService _listService;
        private readonly IAnalysisService _service;

        public AnalysisController(
            IListService listService,
            IAnalysisService service
        )
        {
            _listService = listService;
            _service = service;
        }

        public async Task<ActionResult> Index()
        {
            try
            {
                ViewData["Operators"] = await _listService.GetAllOperators();
                ViewData["DataList"] = await _service.GetData(new AnalysisFilter());
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index([FromForm] AnalysisFilter filter)
        {
            try
            {
                ViewData["Operators"] = await _listService.GetAllOperators();
                ViewData["DataList"] = await _service.GetData(filter);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View(filter);
        }
    }
}
