using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeKeeping.Application.ViewModels.AccountViewModel;

namespace TimeKeeping.Application.Services
{
    public interface IAccountService
    {
        Task<IEnumerable<RegisterViewModel>> CreateAccountAsync(RegisterViewModel registerViewModel);
        Task<IEnumerable<LoginResponseViewModel>> AuthenticateAsync(LoginViewModel loginViewModel);
    }
}
