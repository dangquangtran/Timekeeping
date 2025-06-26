using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeKeeping.Application.Attributes;
using TimeKeeping.Application.Helpers;
using TimeKeeping.Application.Services;
using TimeKeeping.Application.ViewModels.ChamCongCheckInOutV1ViewModel;
using TimeKeeping.Infrastructure.ServiceImpls;

namespace Timekeeping.Controllers
{
    [AdminOnly]
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
        [Route("QuanTri/DuLieuChamCong")]
        public async Task<IActionResult> DuLieuChamCong(int pageIndex = 1, int pageSize = 20, string? donVi = null, string? ky = null)
        {
            var filter = new FilterChamCongViewModel
            {
                DonVi = donVi,
                Ky = ky
            };
            
            var (list, totalPages) = await _chamCongCheckInOutV1Service.GetFilteredAsync(pageIndex, pageSize, filter);
            var vm = new ChamCongCheckInOutV1PageViewModel
            {
                List = list,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalPages = totalPages
            };
            
            // Truyền filter values vào ViewBag để giữ lại giá trị đã chọn
            ViewBag.SelectedDonVi = donVi;
            ViewBag.SelectedKy = ky;
            // Lấy danh sách kỳ và đơn vị cho dropdown
            ViewBag.DanhSachKy = await _chamCongCheckInOutV1Service.GetAllKyAsync();
            ViewBag.DanhSachDonVi = await _chamCongCheckInOutV1Service.GetAllDonViAsync();
            
            return View("~/Views/QuanTri/DuLieuChamCong.cshtml", vm);
        }


        [HttpGet]
        public async Task<IActionResult> GetDanhSachChamCong([FromQuery] int pageIndex, [FromQuery] int pageSize)
        {
            var result = await _chamCongCheckInOutV1Service.GetAllAsync(pageIndex, pageSize);
            return Ok(result);
        }


        [HttpPost]
        public async Task<IActionResult> ExportExcelDSChamCong([FromForm] ChamCongExportRequestViewModel chamCongExportRequestViewModel)
        {
            var result = await _chamCongCheckInOutV1Service.ExportChamCongToExcelAsync(chamCongExportRequestViewModel);
            if (result == null || result.Length == 0)
            {
                return BadRequest("Không có dữ liệu chấm công phù hợp với yêu cầu.");
            }
            if (!KyParserHelper.TryParseKy(chamCongExportRequestViewModel.Ky, out int thang, out int nam))
            {
                return BadRequest("Kỳ không hợp lệ. Định dạng đúng là 'Tháng 6/2025'.");
            }
            return File(
                result,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"ChamCong_{thang}_{nam}.xlsx"
            );
        }


        [HttpGet]
        public async Task<IActionResult> GetDanhSachKy()
        {
            var result = await _chamCongCheckInOutV1Service.GetAllKyAsync();
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetDanhSachDonVi()
        {
            var result = await _chamCongCheckInOutV1Service.GetAllDonViAsync();
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetDanhSachChamCongFilter([FromQuery] int pageIndex, [FromQuery] int pageSize, FilterChamCongViewModel filter)
        {
            var result = await _chamCongCheckInOutV1Service.GetFilteredAsync(pageIndex,pageSize,filter);
            var (list, totalPages) = result;

            return Ok(new { List = list, TotalPages = totalPages });
        }
    }
}
