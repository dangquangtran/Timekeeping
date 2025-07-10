using Microsoft.AspNetCore.Mvc;
using TimeKeeping.Application.Services;
using TimeKeeping.Application.ViewModels.XetDuyetViewModel;

namespace Timekeeping.Controllers
{
    public class XetDuyetGiaiTrinhController : Controller
    {
        private readonly ILogger<XetDuyetGiaiTrinhController> _logger;
        private readonly IXetDuyetGiaiTrinhService _xetDuyetGiaiTrinhService;

        public XetDuyetGiaiTrinhController(ILogger<XetDuyetGiaiTrinhController> logger,
                                           IXetDuyetGiaiTrinhService xetDuyetGiaiTrinhService)
        {
            _logger = logger;
            _xetDuyetGiaiTrinhService = xetDuyetGiaiTrinhService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetDanhSachGiaiTrinh([FromQuery] int pageIndex, [FromQuery] int pageSize, [FromQuery] string? status, [FromQuery] string? donVi, [FromQuery] string? ky)
        {
            var filter = new XetDuyetGiaiTrinhFilter
            {
                Status = status,
                DonVi = donVi,
                Ky = ky
            };
            var result = await _xetDuyetGiaiTrinhService.GetListGiaiTrinhAsync(pageIndex, pageSize, filter);
            var (list, totalPages) = result;
            return Ok(new { List = list, TotalPages = totalPages });
        }

        [HttpPost]
        public async Task<IActionResult> ApproveSelected([FromBody] List<XetDuyetApproveRequest> requests)
        {
            var ids = requests.Select(x => (x.viphamid, x.maky));
            var count = await _xetDuyetGiaiTrinhService.ApproveSelectedAsync(ids);
            return Ok(new { Success = true, ApprovedCount = count });
        }
    }
}
