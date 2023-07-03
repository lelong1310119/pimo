using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PismoWebInput.Core.Infrastructure.Models.UserManager;
using PismoWebInput.Core.Infrastructure.Services;

namespace PismoWebInput.Controllers
{
    [Authorize(Roles = "Admin,Leader,Staff")]
    public class UserManagerController : Controller
    {
        private readonly IListService _listService;
        private readonly IUserManagerService _service;

        public UserManagerController(
            IListService listService,
            IUserManagerService service
            )
        {
            _listService = listService;
            _service = service;
        }

        public async Task<ActionResult> Index(long operatorId)
        {
            ViewData["Operators"] = await _listService.GetAllOperators();
            ViewData["selectedOperator"] = ViewData["selectedOperator"] != null && operatorId.ToString() == ViewData["selectedOperator"].ToString() ? null : operatorId;

            return View(await _service.GetAll(operatorId));
        }

        public async Task<ActionResult> Create()
        {
            return View(new CreateUserManagerModel
            {
                Operators = await _listService.GetAllOperators()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([FromForm] CreateUserManagerModel model)
        {
            try
            {
                await _service.CreateUser(model);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        public async Task<ActionResult> Edit(string id)
        {
            return View(await _service.GetUser(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([FromForm] EditUserManagerModel model)
        {
            try
            {
                await _service.EditUser(model);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                await _service.DeleteUser(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("Index");
            }
        }
    }
}
