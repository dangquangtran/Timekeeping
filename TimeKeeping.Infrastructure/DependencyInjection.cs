using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeKeeping.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using TimeKeeping.Infrastructure.Persistence;
using TimeKeeping.Infrastructure.Repositories;
using TimeKeeping.Application.Services;
using TimeKeeping.Infrastructure.ServiceImpls;
using Microsoft.Extensions.Logging;

namespace TimeKeeping.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ILyDoViPhamService, LyDoViPhamService>();
            services.AddScoped<IChamCongCheckInOutV1Service, ChamCongCheckInOutV1Service>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IBaoCaokiService, BaoCaokiService>();
           
            var jwtSecretKey = configuration["Jwt:SecretKey"];

            // Đăng ký AccountService và truyền jwtSecretKey vào constructor
            services.AddScoped<IAccountService>(provider =>
                new AccountService(
                    provider.GetRequiredService<IUnitOfWork>(),
                    provider.GetRequiredService<ILogger<AccountService>>(),
                    jwtSecretKey
                ));
            return services;
        }
    }
}
