using Microsoft.AspNetCore.Mvc;
using TimeKeeping.Application.Services;
using TimeKeeping.Application.ViewModels.AccountViewModel;
using TimeKeeping.Application.ViewModels.BaoCaokiViewModel;
using TimeKeeping.Infrastructure.ServiceImpls;

namespace Timekeeping.Controllers
{
    public class BaoCaokiController : Controller
    {
        private readonly ILogger<BaoCaokiController> _logger;
        private readonly IBaoCaokiService _baoCaokiService;
        public BaoCaokiController(ILogger<BaoCaokiController> logger,
                                  IBaoCaokiService baoCaokiService)
        {
            _baoCaokiService = baoCaokiService;
            _logger = logger;
        }

        [Route("CapNhat/KyBaoCao")]
        public async Task<IActionResult> KyBaoCao(int pageIndex = 1, int pageSize = 10)
        {
            var (list, totalCount) = await _baoCaokiService.GetAllAsync(pageIndex, pageSize);
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var vm = new KyBaoCaoPageViewModel
            {
                CreateModel = new BaoCaokiCreateViewModel(),
                List = list,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalPages = totalPages
            };
            return View("~/Views/CapNhat/KyBaoCao.cshtml", vm);
        }


        [HttpGet]
        public async Task<IActionResult> GetBaoCaoKi([FromQuery] int pageIndex, [FromQuery] int pageSize)
        {
            var result = await _baoCaokiService.GetAllAsync(pageIndex, pageSize);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBaoCaoKi(KyBaoCaoPageViewModel model)
        {
            try
            {
                await _baoCaokiService.CreateBaoCaoKiAsync(model.CreateModel);
                return RedirectToAction("KyBaoCao");
            }
            catch (Exception ex)
            {
                // Lỗi nghiệp vụ: tên kỳ đã tồn tại
                if (ex.Message.Contains("đã tồn tại", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("CreateModel.tenky", ex.Message);
                }
                else // Lỗi hệ thống
                {
                    ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.");
                }

                // Lấy lại danh sách để hiển thị bảng
                int pageIndex = model.PageIndex > 0 ? model.PageIndex : 1;
                int pageSize = model.PageSize > 0 ? model.PageSize : 10;
                var (list, totalCount) = await _baoCaokiService.GetAllAsync(pageIndex, pageSize);
                model.List = list;
                model.PageIndex = pageIndex;
                model.PageSize = pageSize;
                model.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                return View("~/Views/CapNhat/KyBaoCao.cshtml", model);
            }
        }



    }
}
