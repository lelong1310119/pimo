using Autofac.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PismoWebInput.Core.Infrastructure.Models.Master;
using PismoWebInput.Core.Infrastructure.Models.StatusPicking;
using PismoWebInput.Core.Infrastructure.Services;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;

namespace PismoWebInput.Controllers
{
    [Authorize()]
    public class PickingStatusController : Controller
    {
        private static int pageSize = 5;
        private readonly IPickingStatusService _pickingStatusService;

        public PickingStatusController(IPickingStatusService pickingStatusService)
        {
            _pickingStatusService = pickingStatusService;
        }


        // GET: PickingStatusController
        public async Task<ActionResult> Index(int? page)
        {
            PickingStatusFilter filter;
            byte[] filterBytes = HttpContext.Session.Get("filter");
            if (filterBytes != null)
            {
                var filterJson = System.Text.Encoding.UTF8.GetString(filterBytes);
                filter = System.Text.Json.JsonSerializer.Deserialize<PickingStatusFilter>(filterJson);
            }
            else
            {
                filter = new PickingStatusFilter();
            }
            ViewData["page"] = (page ?? 1);
            int pageNumber = (page ?? 1);
            int count = await _pickingStatusService.GetTotalFilter(filter);
            int totalPage = (count % pageSize == 0) ? (count / pageSize) : (count / pageSize + 1);
            ViewData["totalPage"] = totalPage;
            ViewData["items"] = await _pickingStatusService.GetAllPickingStatus(filter, pageNumber, pageSize);
            HttpContext.Session.SetInt32("page", pageNumber);
            return View(filter);
        }


        [HttpPost]
        public async Task<ActionResult> Index([FromForm] PickingStatusFilter filter)
        {
            var filterBytes = System.Text.Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(filter));
            HttpContext.Session.Set("filter", filterBytes);
            ViewData["page"] = 1;
            int count = await _pickingStatusService.GetTotalFilter(filter);
            int totalPage = (count % pageSize == 0) ? (count / pageSize) : (count / pageSize + 1);
            ViewData["totalPage"] = totalPage;
            ViewData["items"] = await _pickingStatusService.GetAllPickingStatus(filter, 1, pageSize);
            HttpContext.Session.SetInt32("page", 1);
            return View(filter);
        }

        public IActionResult ClearSession()
        {
            HttpContext.Session.Remove("filter");
            HttpContext.Session.Remove("page");
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ExportExcel()
        {
            var data = await _pickingStatusService.GetAll();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Data");
                worksheet.Cells.LoadFromCollection(data, true);
                var stream = new MemoryStream(package.GetAsByteArray());

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "data.xlsx");
            }
        }
        // GET: PickingStatusController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PickingStatusController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([FromForm] PickingStatusModel model)
        {
            try
            {
                await _pickingStatusService.Create(model);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        public async Task<ActionResult> Edit(long id)
        {
            return View(await _pickingStatusService.GetPickingStatus(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([FromForm] PickingStatusModel model)
        {
            try
            {
                int? page = HttpContext.Session.GetInt32("page");
                await _pickingStatusService.UpdatePickingStatus(model);
                return RedirectToAction(nameof(Index), new {page});
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }


        public async Task<IActionResult> Delete(long id)
        {
            await _pickingStatusService.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
