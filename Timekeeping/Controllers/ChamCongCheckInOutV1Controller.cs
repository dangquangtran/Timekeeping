using Microsoft.AspNetCore.Mvc;
using TimeKeeping.Application.Services;
using TimeKeeping.Infrastructure.ServiceImpls;

namespace Timekeeping.Controllers
{
    public class ChamCongCheckInOutV1Controller : Controller
    {
        private readonly ILogger<ChamCongCheckInOutV1Controller> _logger;
        private readonly IChamCongCheckInOutV1Service _chamCongCheckInOutV1Service;
        public ChamCongCheckInOutV1Controller(ILogger<ChamCongCheckInOutV1Controller> logger,
                                              IChamCongCheckInOutV1Service chamCongCheckInOutV1Service)
        {
            _logger = logger;
            _chamCongCheckInOutV1Service = chamCongCheckInOutV1Service;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetDanhSachChamCong([FromQuery] int pageIndex, [FromQuery] int pageSize)
        {
            var result = await _chamCongCheckInOutV1Service.GetAllAsync(pageIndex, pageSize);
            return Ok(result);
        }
    }
}
