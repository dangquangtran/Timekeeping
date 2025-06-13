using Microsoft.AspNetCore.Mvc;
using TimeKeeping.Application.Services;
using TimeKeeping.Application.ViewModels.ChamCongCheckInOutV1ViewModel;
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
        [Route("QuanTri/DuLieuChamCong")]
        public async Task<IActionResult> DuLieuChamCong(int pageIndex = 1, int pageSize = 20)
        {
            var data = await _chamCongCheckInOutV1Service.GetAllAsync(pageIndex, pageSize);
            return View("~/Views/QuanTri/DuLieuChamCong.cshtml", data);
        }

        [HttpGet]
        public async Task<IActionResult> GetDanhSachChamCong([FromQuery] int pageIndex, [FromQuery] int pageSize)
        {
            var result = await _chamCongCheckInOutV1Service.GetAllAsync(pageIndex, pageSize);
            return Ok(result);
        }


        [HttpPost]
        public async Task<IActionResult> ExportExcelDSChamCong([FromBody] ChamCongExportRequestViewModel chamCongExportRequestViewModel)
        {
            var result = await _chamCongCheckInOutV1Service.ExportChamCongToExcelAsync(chamCongExportRequestViewModel);
            //return Ok(result);
            if (result == null || result.Length == 0)
            {
                return BadRequest("Không có dữ liệu chấm công phù hợp với yêu cầu.");
            }
            return File(
        result,
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        $"ChamCong_{chamCongExportRequestViewModel.Thang}_{chamCongExportRequestViewModel.Nam}.xlsx"
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
    }
}
