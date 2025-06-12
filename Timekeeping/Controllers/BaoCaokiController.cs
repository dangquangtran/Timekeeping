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
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetBaoCaoKi([FromQuery] int pageIndex, [FromQuery] int pageSize)
        {
            var result = await _baoCaokiService.GetAllAsync(pageIndex, pageSize);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBaoCaoKi([FromBody] BaoCaokiCreateViewModel  baoCaokiCreateViewModel)
        {
            var result = await _baoCaokiService.CreateBaoCaoKiAsync(baoCaokiCreateViewModel);
            return Ok(result);
        }
    }
}
