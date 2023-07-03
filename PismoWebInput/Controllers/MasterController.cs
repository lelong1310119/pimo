using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PismoWebInput.Core.Infrastructure.Models.Master;
using PismoWebInput.Core.Infrastructure.Services;

namespace PismoWebInput.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MasterController : Controller
    {
        private readonly IMasterService _service;

        public MasterController(IMasterService service)
        {
            _service = service;
        }

        public async Task<ActionResult> Index()
        {
            ViewData["Items"] = await _service.GetAllMasterContent(new MasterContentFilter());
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index([FromForm] MasterContentFilter filter)
        {
            ViewData["Items"] = await _service.GetAllMasterContent(filter);
            return View(filter);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([FromForm] MasterModel model)
        {
            try
            {
                await _service.CreateMaster(model);
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
            return View(await _service.GetMaster(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([FromForm] MasterModel model)
        {
            try
            {
                await _service.UpdateMaster(model);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        public async Task<ActionResult> Delete(long id)
        {
            await _service.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
