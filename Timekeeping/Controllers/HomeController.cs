using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Timekeeping.Models;
using TimeKeeping.Application.Services;

namespace Timekeeping.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ILyDoViPhamService _lyDoViPhamService;

        public HomeController(ILogger<HomeController> logger, ILyDoViPhamService lyDoViPhamService)
        {
            _logger = logger;
            _lyDoViPhamService = lyDoViPhamService;
        }

        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 20)
        {
            var result = await _lyDoViPhamService.GetAllAsync(pageIndex, pageSize);
            return View(result);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
