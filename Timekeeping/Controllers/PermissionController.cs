using Microsoft.AspNetCore.Mvc;
using TimeKeeping.Application.Services;
using TimeKeeping.Application.ViewModels.PermissionVIewModel;
using TimeKeeping.Infrastructure.ServiceImpls;

namespace Timekeeping.Controllers
{
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
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetDanhSachAccount([FromQuery] int pageIndex, [FromQuery] int pageSize)
        {
            var result = await _permissionService.GetAllAsync(pageIndex, pageSize);
            return Ok(result);
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
