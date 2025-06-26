using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeKeeping.Application.Attributes;
using TimeKeeping.Application.Services;

namespace Timekeeping.Controllers
{
    [AdminOnly]
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
        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 20)
        {
            var result = await _lyDoViPhamService.GetAllAsync(pageIndex, pageSize);
            return View(result);
        }


        [HttpGet]
        public async Task<IActionResult> GetLyDoViPham([FromQuery] int pageIndex, [FromQuery] int pageSize)
        {
            var result = await _lyDoViPhamService.GetAllAsync(pageIndex, pageSize);
            return Ok(result);
        }
    }
}
