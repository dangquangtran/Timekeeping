using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeKeeping.Application.Attributes;
using TimeKeeping.Application.Services;
using TimeKeeping.Application.ViewModels.PermissionVIewModel;
using TimeKeeping.Infrastructure.ServiceImpls;

namespace Timekeeping.Controllers
{
    [AdminOnly]
    public class PermissionController : Controller
    {
        private readonly ILogger<PermissionController> _logger;
        private readonly IPermissionService _permissionService;

        public PermissionController(ILogger<PermissionController> logger,
                                    IPermissionService permissionService)
        {
            _logger = logger;
            _permissionService = permissionService;
        }
        [Route("QuanTri/PhanQuyen")]
        public async Task<IActionResult> PhanQuyen(int pageIndex = 1, int pageSize = 10, string msnv = "")
        {
            var (accounts, totalCount) = await _permissionService.GetAllAsync(pageIndex, pageSize, msnv);
            var allAccounts = await _permissionService.GetAllAccountsForDropdownAsync();
            var permissions = await _permissionService.GetAllPermissionAsync();

            var model = new PermissionPageViewModel
            {
                Accounts = accounts,
                AllAccounts = allAccounts,
                Permissions = permissions,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
            
            ViewBag.MSNV = msnv;
            
            return View("~/Views/QuanTri/PhanQuyen.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> ThemQuyen(PermissionPageViewModel model)
        {
            ModelState.Remove("Accounts");
            ModelState.Remove("AllAccounts");
            ModelState.Remove("Permissions");
            if (!ModelState.IsValid)
            {
                model.AllAccounts = await _permissionService.GetAllAccountsForDropdownAsync();
                model.Permissions = await _permissionService.GetAllPermissionAsync();
                return View("~/Views/QuanTri/PhanQuyen.cshtml", model);
            }

            try
            {
                var result = await _permissionService.UpdatePermissionAsync(model.PermissionUpdate);
                TempData["SuccessMessage"] = result.Any() ? "Cập nhật quyền thành công!" : "Có lỗi xảy ra khi cập nhật quyền!";
                return RedirectToAction("PhanQuyen", new { pageIndex = model.PageIndex, pageSize = model.PageSize });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                model.AllAccounts = await _permissionService.GetAllAccountsForDropdownAsync();
                model.Permissions = await _permissionService.GetAllPermissionAsync();
                return View("~/Views/QuanTri/PhanQuyen.cshtml", model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDanhSachAccount([FromQuery] int pageIndex, [FromQuery] int pageSize, [FromQuery] string msnv)
        {
            var (accounts, totalCount) = await _permissionService.GetAllAsync(pageIndex, pageSize, msnv);
            return Ok(new { accounts, totalCount });
        }

        [HttpGet]
        public async Task<IActionResult> GetDanhSachPermission()
        {
            var result = await _permissionService.GetAllPermissionAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePermission([FromBody] PermissionUpdateViewModel permissionUpdateViewModel )
        {
            var result = await _permissionService.UpdatePermissionAsync(permissionUpdateViewModel);
            return Ok(result);
        }
    }
}
