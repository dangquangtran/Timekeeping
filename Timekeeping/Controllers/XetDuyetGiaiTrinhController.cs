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

        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 10, string? status = null, string? donVi = null, string? ky = null)
        {
            try
            {
                // Get dropdown data
                var kyList = await _xetDuyetGiaiTrinhService.GetAllKyAsync();
                var donViList = await _xetDuyetGiaiTrinhService.GetAllDonViAsync();

                // Get filtered data
                var filter = new XetDuyetGiaiTrinhFilter
                {
                    Status = status,
                    DonVi = donVi,
                    Ky = ky
                };

                var result = await _xetDuyetGiaiTrinhService.GetListGiaiTrinhAsync(pageIndex, pageSize, filter);
                var (list, totalPages) = result;

                // Create page model
                var model = new XetDuyetPageViewModel
                {
                    List = list,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    TotalRecords = list.Count() > 0 ? (totalPages * pageSize) : 0,
                    SelectedDonVi = donVi,
                    SelectedKy = ky,
                    SelectedStatus = status
                };

                // Pass dropdown data to ViewBag
                ViewBag.DanhSachKy = kyList;
                ViewBag.DanhSachDonVi = donViList;
                ViewBag.SelectedDonVi = donVi;
                ViewBag.SelectedKy = ky;
                ViewBag.SelectedStatus = status;

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "L?i khi t?i trang xét duy?t gi?i trình");
                ViewBag.ErrorMessage = "Có l?i x?y ra khi t?i d? li?u";
                return View(new XetDuyetPageViewModel());
            }
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
            try
            {
                _logger.LogInformation("ApproveSelected called with {Count} requests", requests?.Count ?? 0);
                
                if (requests == null || !requests.Any())
                {
                    _logger.LogWarning("Requests is null or empty");
                    return BadRequest(new { success = false, message = "Danh sách duyệt không được để trống." });
                }

                _logger.LogInformation("Processing {Count} requests for approval", requests.Count);

                // Log từng request để debug
                foreach (var req in requests)
                {
                    _logger.LogInformation("Approving: viphamid={ViphamId}, maky={Maky}", req.viphamid, req.maky);
                }
                
                var ids = requests.Select(x => (x.viphamid, x.maky));
                var count = await _xetDuyetGiaiTrinhService.ApproveSelectedAsync(ids);
                
                _logger.LogInformation("Successfully approved {Count} items", count);
                
                // **FIX**: Thêm thông tin để client refresh đúng cách
                return Ok(new { 
                    success = true, 
                    approvedCount = count,
                    message = $"Đã duyệt thành công {count} mục. Trang sẽ được làm mới để cập nhật trạng thái.",
                    shouldRefresh = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi duyệt các giải trình được chọn");
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra khi duyệt: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ApproveAll()
        {
            try
            {
                var count = await _xetDuyetGiaiTrinhService.ApproveAllChuaXetDuyetAsync();
                _logger.LogInformation("Successfully approved all pending items: {Count}", count);
                return Ok(new { success = true, approvedCount = count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi duyệt tất cả giải trình chưa xét duyệt");
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra khi duyệt tất cả: " + ex.Message });
            }
        }
    }
}
