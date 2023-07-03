using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PismoWebInput.Core.Infrastructure.Models.Productivity;
using PismoWebInput.Core.Infrastructure.Services;

namespace PismoWebInput.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductivityController : Controller
    {
        private readonly IListService _listService;
        private readonly IProductivityService _service;

        public ProductivityController(
            IListService listService,
            IProductivityService service
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
                ViewData["DataList"] = await _service.GetBLocks(new ProductivityFilter());
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index([FromForm] ProductivityFilter filter)
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

        [HttpPost]
        public IActionResult Export(string GridHtml)
        {
            return File(System.Text.Encoding.ASCII.GetBytes(GridHtml), "application/vnd.ms-excel", $"Productivity_{DateTime.Now:ddMMyyyyhhmm}.xls");
        }
    }
}
