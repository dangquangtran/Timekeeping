using Microsoft.AspNetCore.Mvc;
using TimeKeeping.Application.Services;

namespace Timekeeping.Controllers
{
    public class LyDoViPhamController : Controller
    {
        private readonly ILogger<LyDoViPhamController> _logger;
        private readonly ILyDoViPhamService _lyDoViPhamService;

        public LyDoViPhamController(ILogger<LyDoViPhamController> logger,
                                   ILyDoViPhamService lyDoViPhamService)
        {
            _logger = logger;
            _lyDoViPhamService = lyDoViPhamService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetLyDoViPham([FromQuery] int pageIndex, [FromQuery] int pageSize)
        {
            var result = await _lyDoViPhamService.GetAllAsync(pageIndex, pageSize);
            return Ok(result);
        }
    }
}
