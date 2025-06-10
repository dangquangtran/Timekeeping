using Microsoft.AspNetCore.Mvc;
using TimeKeeping.Application.Services;
using TimeKeeping.Application.ViewModels.AccountViewModel;
using TimeKeeping.Infrastructure.ServiceImpls;

namespace Timekeeping.Controllers
{
    public class LoginController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<LoginController> _logger;

        public LoginController(ILogger<LoginController> logger,
                               IAccountService accountService)
        {
            _logger = logger;
            _accountService = accountService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody]RegisterViewModel registerViewModel)
        {
            var result = await _accountService.CreateAccountAsync(registerViewModel);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginViewModel loginViewModel)
        {
            var result = await _accountService.AuthenticateAsync(loginViewModel);
            return Ok(result);
        }
    }
}
