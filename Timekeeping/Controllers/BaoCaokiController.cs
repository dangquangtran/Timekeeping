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
        public async Task<IActionResult> KyBaoCao(int pageIndex = 1, int pageSize = 20)
        {
            var list = await _baoCaokiService.GetAllAsync(pageIndex, pageSize);
            var vm = new KyBaoCaoPageViewModel
            {
                CreateModel = new BaoCaokiCreateViewModel(),
                List = list
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
            var createModel = model.CreateModel;
            await _baoCaokiService.CreateBaoCaoKiAsync(createModel);
            return RedirectToAction("KyBaoCao");
        }


    }
}
