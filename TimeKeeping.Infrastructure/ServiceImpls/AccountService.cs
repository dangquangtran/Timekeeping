using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TimeKeeping.Application.Exceptions;
using TimeKeeping.Application.Interfaces;
using TimeKeeping.Application.Services;
using TimeKeeping.Application.ViewModels.AccountViewModel;
using TimeKeeping.Domain.Entities;

namespace TimeKeeping.Infrastructure.ServiceImpls
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AccountService> _logger;
        private readonly string _jwtSecretKey;

        public AccountService(IUnitOfWork unitOfWork, ILogger<AccountService> logger, string jwtSecretKey)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _jwtSecretKey = jwtSecretKey; 
        }
        private async Task<string> GenerateUniqueMaNhanVienAsync()
        {
            string maNhanVien;
            var random = new Random(); 
            tb_Account existingMaNhanVien;  

            do
            {
                maNhanVien = random.Next(10000, 99999).ToString();

                existingMaNhanVien = await _unitOfWork.AccountRepo.FirstOrDefaultAsync(a => a.MaNhanVien == maNhanVien);

            } while (existingMaNhanVien != null);  

            return maNhanVien; 
        }

        //Method Hash password
        private string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException(nameof(password), "Password cannot be null or empty.");
            }

            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        // Method Generate JWT Token
        private string GenerateJwtToken(tb_Account account)
        {
            var claims = new List<Claim>
            {
                new Claim("UserID", account.ID.ToString()),
                new Claim("UserName", account.UserName),
                new Claim("FullName", account.FullName),
                new Claim(ClaimTypes.Name, account.FullName), // Thêm dòng này
                new Claim(ClaimTypes.Role, account.GroupFuncID.ToString()),
                new Claim("GroupFuncID", account.GroupFuncID.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "YourIssuer",   
                audience: "YourAudience",
                expires: DateTime.Now.AddHours(1),
                claims: claims,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public async Task<IEnumerable<RegisterViewModel>> CreateAccountAsync(RegisterViewModel registerViewModel)
        {
            _logger.LogInformation("Bắt đầu tạo tài khoản mới");

            try
            {
                var existingAccount = await _unitOfWork.AccountRepo.FirstOrDefaultAsync(a => a.UserName == registerViewModel.UserName);

                if (existingAccount != null) 
                {
                    _logger.LogWarning("Username đã tồn tại: " + registerViewModel.UserName);
                    throw new UsernameAlreadyExistsException("Username đã tồn tại.");
                }
               

                var hashedPassword = HashPassword(registerViewModel.Password);

                string maNhanVien = await GenerateUniqueMaNhanVienAsync();

                var newAccount = new tb_Account
                {
                    UserName = registerViewModel.UserName,
                    MaNhanVien = maNhanVien,
                    FullName = registerViewModel.FullName,
                    HashPassword = hashedPassword,
                    GroupFuncID = registerViewModel.GroupFuncID,
                    IsActive = true,
                    LastLogin = DateTime.Now
                };

                await _unitOfWork.AccountRepo.AddAsync(newAccount);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                var result = new RegisterViewModel
                {
                    UserName = newAccount.UserName,
                    FullName = newAccount.FullName,
                    GroupFuncID = newAccount.GroupFuncID
                };

                return new List<RegisterViewModel> { result };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }
        public async Task<IEnumerable<LoginResponseViewModel>> AuthenticateAsync(LoginViewModel loginViewModel)
        {
            _logger.LogInformation("Bắt đầu xác thực tài khoản");

            try
            {
                var hashedPassword = HashPassword(loginViewModel.Password);

                var account = await _unitOfWork.AccountRepo.FirstOrDefaultAsync(a => a.UserName == loginViewModel.UserName 
                                                                         && a.HashPassword == hashedPassword && a.IsActive);

                if (account == null)
                {
                    throw new Exception("Thông tin đăng nhập không chính xác.");
                }

                var jwtToken = GenerateJwtToken(account);

                var result = new LoginResponseViewModel
                {
                    JwtToken = jwtToken 
                };

                return new List<LoginResponseViewModel> { result };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xác thực tài khoản");
                throw;
            }
        }
    }
}
