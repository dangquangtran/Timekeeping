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
        [Route("BaoCao/ChamCongVanTay")]
        public async Task<IActionResult> ChamCongVanTay(int pageIndex = 1, int pageSize = 10,
                                               string? tenDonVi = null, string? tenKy = null)
        {
            try
            {
                // Lấy dữ liệu vi phạm
                var (list, totalPages) = string.IsNullOrWhiteSpace(tenDonVi) && string.IsNullOrWhiteSpace(tenKy)
                    ? await _baoCaoViPhamService.GetListBaoCaoViPhamChamCongVanTayAsync(pageIndex, pageSize)
                    : await _baoCaoViPhamService.GetListBaoCaoViPhamChamCongVanTayFilterAsync(pageIndex, pageSize, tenDonVi, tenKy);

                // Lấy danh sách đơn vị và kỳ từ service
                var kyList = await _baoCaoViPhamService.GetAllKyAsync();
                var donViList = await _baoCaoViPhamService.GetAllDonViAsync();

                // Lấy thông tin kỳ hiện tại được chọn
                var currentKy = await _baoCaoViPhamService.GetCurrentKyInfoAsync(tenKy);

                // Tạo model cho view
                var model = new BaoCaoViPhamPageViewModel
                {
                    List = list,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    TotalRecords = list.Count() > 0 ? (totalPages * pageSize) : 0,
                    SelectedDonVi = tenDonVi,
                    SelectedKy = tenKy
                };

                // Truyền dữ liệu qua ViewBag
                ViewBag.DanhSachKy = kyList;
                ViewBag.DanhSachDonVi = donViList;
                ViewBag.SelectedDonVi = tenDonVi;
                ViewBag.SelectedKy = tenKy;
                ViewBag.CurrentKy = currentKy; 

                // Chỉ định rõ đường dẫn view
                return View("~/Views/BaoCao/ChamCongVanTay.cshtml", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải trang báo cáo vi phạm");
                ViewBag.ErrorMessage = "Có lỗi xảy ra khi tải dữ liệu";
                return View("~/Views/BaoCao/ChamCongVanTay.cshtml", new BaoCaoViPhamPageViewModel());
            }
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
            var result = await _baoCaoViPhamService.GetListBaoCaoViPhamChamCongVanTayFilterAsync(pageIndex, pageSize, maDonVi, maKy);
            var (list, totalPages) = result;

            return Ok(new { List = list, TotalPages = totalPages });
        }

        [HttpPost]
        public async Task<IActionResult> ExportExcelViPhamChamCong([FromForm] BaoCaoViPhamExportRequestViewModel request)
        {
            try
            {
                var result = await _baoCaoViPhamService.ExportBaoCaoViPhamChamCongVanTayToExcelAsync(request);

                if (result == null || result.Length == 0)
                {
                    return BadRequest("Không có dữ liệu vi phạm chấm công phù hợp với yêu cầu.");
                }

                // Parse lại kỳ để đặt tên file
                var fileName = "BaoCaoViPhamChamCong.xlsx";
                if (!string.IsNullOrWhiteSpace(request.Ky) && 
                    KyParserHelper.TryParseKy(request.Ky, out int thang, out int nam))
                {
                    fileName = $"BaoCaoViPhamChamCong_{thang}_{nam}.xlsx";
                }

                return File(
                    result,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xuất Excel báo cáo vi phạm");
                return BadRequest("Có lỗi xảy ra khi xuất file Excel");
            }
        }
    }
}
