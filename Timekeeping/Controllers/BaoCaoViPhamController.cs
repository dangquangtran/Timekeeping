using Microsoft.AspNetCore.Mvc;
using TimeKeeping.Application.Helpers;
using TimeKeeping.Application.Services;
using TimeKeeping.Application.ViewModels.BaoCaoViPhamViewModel;
using TimeKeeping.Application.ViewModels.ChamCongCheckInOutV1ViewModel;

namespace Timekeeping.Controllers
{
    public class BaoCaoViPhamController : Controller
    {
        private readonly ILogger<BaoCaoViPhamController> _logger;
        private readonly IBaoCaoViPhamService _baoCaoViPhamService;

        public BaoCaoViPhamController(ILogger<BaoCaoViPhamController> logger,
                                      IBaoCaoViPhamService baoCaoViPhamService)
        {
            _baoCaoViPhamService = baoCaoViPhamService;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetListBaoCaoViPhamChamCongVanTayAsync([FromQuery] int pageIndex, [FromQuery] int pageSize)
        {
            var result = await _baoCaoViPhamService.GetListBaoCaoViPhamChamCongVanTayAsync(pageIndex, pageSize);
            var (list, totalPages) = result;

            return Ok(new { List = list, TotalPages = totalPages });
        }

        [HttpGet]
        public async Task<IActionResult> GetListBaoCaoViPhamChamCongVanTayFilterAsync([FromQuery] int pageIndex, [FromQuery] int pageSize, [FromQuery] string? maDonVi, [FromQuery] string? maKy)
        {
            var result = await _baoCaoViPhamService.GetListBaoCaoViPhamChamCongVanTayFilterAsync(pageIndex, pageSize,maDonVi,maKy);
            var (list, totalPages) = result;

            return Ok(new { List = list, TotalPages = totalPages });
        }

        [HttpPost]
        public async Task<IActionResult> ExportExcelViPhamChamCong([FromQuery] BaoCaoViPhamExportRequestViewModel request)
        {
            var result = await _baoCaoViPhamService.ExportBaoCaoViPhamChamCongVanTayToExcelAsync(request);

            if (result == null || result.Length == 0)
            {
                return BadRequest("Không có dữ liệu vi phạm chấm công phù hợp với yêu cầu.");
            }

            // Parse lại kỳ để đặt tên file
            if (!KyParserHelper.TryParseKy(request.Ky, out int thang, out int nam))
            {
                return BadRequest("Kỳ không hợp lệ. Định dạng đúng là 'Tháng 06/2025'.");
            }

            return File(
                result,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"BaoCaoViPhamChamCong_{thang}_{nam}.xlsx"
            );
        }

    }
}
