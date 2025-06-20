using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TimeKeeping.Application.Services;
using TimeKeeping.Application.ViewModels.ChamCongCheckInOutV1ViewModel;
using TimeKeeping.Application.ViewModels.GiaiTrinhViewModel;
using TimeKeeping.Infrastructure.ServiceImpls;

namespace Timekeeping.Controllers
{
    [Authorize]
    public class GiaiTrinhController : Controller
    {
        private readonly ILogger<GiaiTrinhController> _logger;
        private readonly IGiaiTrinhService _giaiTrinhService;
        private readonly IBaoCaokiService _baoCaokiService;

        public GiaiTrinhController(ILogger<GiaiTrinhController> logger,
                                   IGiaiTrinhService giaiTrinhService,
                                   IBaoCaokiService baoCaokiService)
        {
            _giaiTrinhService = giaiTrinhService;
            _logger = logger;
            _baoCaokiService = baoCaokiService;
        }

        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 10, string ky = "", 
                                                       int chamCongPageIndex = 1, int chamCongPageSize = 10)
        {
            try
            {
                var userName = User.FindFirst("UserName")?.Value;
                _logger.LogInformation("User đang đăng nhập: {userName}", userName);
                
                if (string.IsNullOrEmpty(userName))
                {
                    _logger.LogWarning("Không tìm thấy UserName trong JWT token");
                    return RedirectToAction("Index", "Login");
                }

                // Lấy thông tin user hiện tại
                var userInfo = await _giaiTrinhService.GetUserInfoAsync(userName);
                ViewBag.UserInfo = userInfo;

                // Lấy danh sách kỳ báo cáo
                var (kyList, _) = await _baoCaokiService.GetAllAsync(1, 100);
                ViewBag.DanhSachKy = kyList;
                ViewBag.SelectedKy = ky;

                // Lấy kỳ hiện tại để hiển thị thời gian
                var currentKy = kyList.FirstOrDefault(k => k.tenky == ky) ?? kyList.FirstOrDefault();
                ViewBag.CurrentKy = currentKy;

                // Lấy danh sách lý do vi phạm
                var lyDoList = await _giaiTrinhService.GetListLidoAsync();
                ViewBag.DanhSachLyDo = lyDoList;

                // Lấy danh sách vi phạm theo user và kỳ
                var (viPhamList, totalPages) = await _giaiTrinhService.GetListGiaiTrinhAsync(pageIndex, pageSize, userName, ky);

                // Lấy dữ liệu chấm công
                var (chamCongList, chamCongTotalPages) = await _giaiTrinhService.GetListChamCongAsync(chamCongPageIndex, chamCongPageSize, userName);

                var model = new GiaiTrinhPageViewModel
                {
                    List = viPhamList,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalPages = totalPages
                };

                // Truyền dữ liệu chấm công qua ViewBag
                ViewBag.ChamCongList = chamCongList;
                ViewBag.ChamCongPageIndex = chamCongPageIndex;
                ViewBag.ChamCongPageSize = chamCongPageSize;
                ViewBag.ChamCongTotalPages = chamCongTotalPages;

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải trang giải trình");
                return View(new GiaiTrinhPageViewModel());
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetDanhSachChamCong([FromQuery] int pageIndex, [FromQuery] int pageSize, string userName)
        {
            var result = await _giaiTrinhService.GetListChamCongAsync(pageIndex, pageSize, userName);
            var (list, totalPages) = result;

            return Ok(new { List = list, TotalPages = totalPages });
        }

        [HttpGet]
        public async Task<IActionResult> GetDanhSachGiaiTrinh([FromQuery] int pageIndex, [FromQuery] int pageSize, string userName, string tenKy = "")
        {
            var result = await _giaiTrinhService.GetListGiaiTrinhAsync(pageIndex, pageSize, userName, tenKy);
            var (list, totalPages) = result;

            return Ok(new { List = list, TotalPages = totalPages });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateGiaiTrinh(string viphamid, int? maky, int? giaitrinhid)
        {
            try
            {
                if (string.IsNullOrEmpty(viphamid) || !maky.HasValue || !giaitrinhid.HasValue)
                {
                    return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ" });
                }

                var giaiTrinhChamCongViewModel = new GiaiTrinhChamCongViewModel
                {
                    viphamid = viphamid,
                    maky = maky,
                    giaitrinhid = giaitrinhid
                };

                await _giaiTrinhService.SaveGiaiTrinhAsync(giaiTrinhChamCongViewModel);
                return Ok(new { success = true, message = "Lưu giải trình thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDanhSachLyDo()
        {
            var result = await _giaiTrinhService.GetListLidoAsync();
            return Ok(result);
        }
    }
}
