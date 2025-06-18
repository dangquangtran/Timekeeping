using Microsoft.AspNetCore.Mvc;
using TimeKeeping.Application.Services;
using TimeKeeping.Application.ViewModels.ChamCongCheckInOutV1ViewModel;
using TimeKeeping.Infrastructure.ServiceImpls;

namespace Timekeeping.Controllers
{
    public class GiaiTrinhController : Controller
    {
        private readonly ILogger<GiaiTrinhController> _logger;
        private readonly IGiaiTrinhService _giaiTrinhService;

        public GiaiTrinhController(ILogger<GiaiTrinhController> logger,
                                   IGiaiTrinhService giaiTrinhService)
        {
            _giaiTrinhService = giaiTrinhService;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> GetDanhSachChamCong([FromQuery] int pageIndex, [FromQuery] int pageSize, string userName)
        {
            var result = await _giaiTrinhService.GetListChamCongAsync(pageIndex, pageSize, userName);
            var (list, totalPages) = result;

            return Ok(new { List = list, TotalPages = totalPages });
        }

        [HttpGet]
        public async Task<IActionResult> GetDanhSachGiaiTrinh([FromQuery] int pageIndex, [FromQuery] int pageSize, string userName)
        {
            var result = await _giaiTrinhService.GetListGiaiTrinhAsync(pageIndex, pageSize, userName);
            var (list, totalPages) = result;

            return Ok(new { List = list, TotalPages = totalPages });
        }
    }
}
