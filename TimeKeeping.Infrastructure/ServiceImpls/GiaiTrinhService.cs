using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeKeeping.Application.Helpers;
using TimeKeeping.Application.Interfaces;
using TimeKeeping.Application.Services;
using TimeKeeping.Application.ViewModels.ChamCongCheckInOutV1ViewModel;
using TimeKeeping.Application.ViewModels.GiaiTrinhViewModel;

namespace TimeKeeping.Infrastructure.ServiceImpls
{
    public class GiaiTrinhService : IGiaiTrinhService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GiaiTrinhService> _logger;

        public GiaiTrinhService(IUnitOfWork unitOfWork,
                                ILogger<GiaiTrinhService> logger)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<(IEnumerable<GiaiTrinhBaoCaoGetViewModel> List, int TotalPages)> GetListChamCongAsync(int pageIndex, int pageSize, string userName)
        {
            _logger.LogInformation("Bắt đầu lấy danh sách Chấm Công với bộ lọc");

            try
            {
                var account = await _unitOfWork.AccountRepo
                       .FirstOrDefaultAsync(acc => acc.UserName == userName);
                if (account == null)
                {
                    _logger.LogWarning("Không tìm thấy tài khoản với User Name: {userName}", userName);
                    return (Enumerable.Empty<GiaiTrinhBaoCaoGetViewModel>(), 0);
                }

                var maNhanVien = account.MaNhanVien;

                var allData = await _unitOfWork.ChamCong_CheckInOut_v1Repo
                    .GetByConditionAsync(x => x.MaNhanVien == maNhanVien);

                var totalCount = allData.Count(); 

                var pagedResult = allData
                    .OrderByDescending(x => x.NgayCham) 
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new GiaiTrinhBaoCaoGetViewModel
                    {
                        NgayCham = x.NgayCham?.ToString("dd/MM/yyyy"),
                        GioCham = x.GioCham?.ToString("HH:mm:ss"),
                    })
                    .ToList();

                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                return (pagedResult, totalPages);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Lỗi khi lấy danh sách chấm công có filter");
                return (Enumerable.Empty<GiaiTrinhBaoCaoGetViewModel>(), 0);
            }
        }

        public async Task<(IEnumerable<GiaiTrinhGetViewModel> List, int TotalPages)> GetListGiaiTrinhAsync(int pageIndex, int pageSize, string userName)
        {
            _logger.LogInformation("Bắt đầu lấy danh sách Chấm Công với bộ lọc");

            try
            {
                var account = await _unitOfWork.AccountRepo
                       .FirstOrDefaultAsync(acc => acc.UserName == userName);
                if (account == null)
                {
                    _logger.LogWarning("Không tìm thấy tài khoản với User Name: {userName}", userName);
                    return (Enumerable.Empty<GiaiTrinhGetViewModel>(), 0);
                }

                var maNhanVien = account.MaNhanVien;

                var allData = await _unitOfWork.VPCCTempRepo
                    .GetByConditionAsync(x => x.madv == maNhanVien);

                var totalCount = allData.Count();

                var pagedResult = allData
                    .OrderByDescending(x => x.hoten)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new GiaiTrinhGetViewModel
                    {
                        Thu = x.ngay.HasValue ? GetThuFromDateTime(x.ngay.Value) : "",
                        Ngay = x.ngay?.ToString("dd/MM/yyyy"),
                        GioVao = x.time_in,
                        GioRa = x.time_out,

                    })
                    .ToList();

                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                return (pagedResult, totalPages);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Lỗi khi lấy danh sách chấm công có filter");
                return (Enumerable.Empty<GiaiTrinhGetViewModel>(), 0);
            }
        }

        private string GetThuFromDateTime(DateOnly date)
        {
            return date.DayOfWeek switch
            {
                DayOfWeek.Monday => "Thứ Hai",
                DayOfWeek.Tuesday => "Thứ Ba",
                DayOfWeek.Wednesday => "Thứ Tư",
                DayOfWeek.Thursday => "Thứ Năm",
                DayOfWeek.Friday => "Thứ Sáu",
                DayOfWeek.Saturday => "Thứ Bảy",
                DayOfWeek.Sunday => "Chủ Nhật",
                _ => ""
            };
        }

    }
}
