using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeKeeping.Application.Interfaces;
using TimeKeeping.Application.Services;
using TimeKeeping.Application.ViewModels.ChamCongCheckInOutV1ViewModel;
using TimeKeeping.Application.ViewModels.LyDoViPhamViewModel;
using TimeKeeping.Domain.Entities;

namespace TimeKeeping.Infrastructure.ServiceImpls
{
    public class ChamCongCheckInOutV1Service: IChamCongCheckInOutV1Service
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ChamCongCheckInOutV1Service> _logger;

        public ChamCongCheckInOutV1Service(IUnitOfWork unitOfWork,
                            ILogger<ChamCongCheckInOutV1Service> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<ChamCongCheckInOutV1GetViewModel>> GetAllAsync(int pageIndex, int pageSize)
        {
            _logger.LogInformation("Bắt đầu lấy danh sách Chấm Công");
            try
            {
                //var (pagedData, totalCount) = await _unitOfWork.ChamCong_CheckInOut_v1Repo.GetPagedAsync(pageIndex, pageSize);

                ////var result = pagedData.Select(c => new ChamCongCheckInOutV1GetViewModel
                ////{

                ////    NgayCham = c.NgayCham,
                ////    GioCham = c.GioCham,
                ////    TenMay = c.TenMay,
                ////}).ToList();

                ////return result;

                //var result = await Task.WhenAll(pagedData.Select(async c =>
                //{
                //    // Lấy account thông qua MaNhanVien
                //    var account = await _unitOfWork.AccountRepo.FirstOrDefaultAsync(d => d.MaNhanVien == c.MaNhanVien);

                //    // Lấy đơn vị từ bảng tb_donvi qua MaDv từ bảng tb_Account
                //    var donVi = account != null ? await _unitOfWork.DonViRepo.FirstOrDefaultAsync(d => d.madv == account.MaDv) : null;

                //    return new ChamCongCheckInOutV1GetViewModel
                //    {
                //        MaChamCong = c.ID,
                //        HoTen = account?.FullName,
                //        ChucDanh = account?.ChucVu,
                //        DonVi = donVi?.tendv, 
                //        NgayCham = c.NgayCham,
                //        GioCham = c.GioCham,
                //        TenMay = c.TenMay,
                //    };
                //}));


                //return result;

                var (pagedData, totalCount) = await _unitOfWork.ChamCong_CheckInOut_v1Repo.GetPagedAsync(pageIndex, pageSize);

                // Lấy tất cả các tài khoản liên quan tới các MaNhanVien trong pagedData
                var maNhanViens = pagedData.Select(c => c.MaNhanVien).Distinct().ToList();
                var accounts = await _unitOfWork.AccountRepo.GetByConditionAsync(a => maNhanViens.Contains(a.MaNhanVien));

                // Lấy tất cả các đơn vị (tb_donvi)
                var donVis = await _unitOfWork.DonViRepo.GetByConditionAsync(d => accounts.Select(a => a.MaDv).Contains(d.madv));

                // Tạo một dictionary để ánh xạ MaNhanVien -> account
                var accountDict = accounts.ToDictionary(a => a.MaNhanVien);

                // Tạo một dictionary để ánh xạ MaDv -> tên đơn vị
                var donViDict = donVis.ToDictionary(d => d.madv, d => d.tendv);

                // Ánh xạ dữ liệu từ pagedData vào viewmodel
                var result = pagedData.Select(c =>
                {
                    var account = accountDict.GetValueOrDefault(c.MaNhanVien);
                    var donVi = account != null ? donViDict.GetValueOrDefault(account.MaDv) : null;
                    var formattedNgayCham = c.NgayCham.HasValue ? c.NgayCham.Value.ToString("dd/MM/yyyy") : null;
                    var formattedGioCham = c.GioCham.HasValue ? c.GioCham.Value.ToString("HH:mm:ss") : null;

                    return new ChamCongCheckInOutV1GetViewModel
                    {
                        MaChamCong = c.ID,
                        HoTen = account?.FullName,
                        ChucDanh = account?.ChucVu,
                        DonVi = donVi,
                        NgayCham = formattedNgayCham,
                        GioCham = formattedGioCham,
                        TenMay = c.TenMay,
                    };
                }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Lỗi khi lấy danh sách chấm công");
                return Enumerable.Empty<ChamCongCheckInOutV1GetViewModel>();
            }
        }
    }
}
