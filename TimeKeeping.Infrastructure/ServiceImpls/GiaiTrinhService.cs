using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
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
using TimeKeeping.Application.ViewModels.LyDoViPhamViewModel;
using TimeKeeping.Domain.Entities;

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

        //public async Task<(IEnumerable<GiaiTrinhGetViewModel> List, int TotalPages)> GetListGiaiTrinhAsync(int pageIndex, int pageSize, string userName)
        //{
        //    _logger.LogInformation("Bắt đầu lấy danh sách Chấm Công với bộ lọc");

        //    try
        //    {
        //        var account = await _unitOfWork.AccountRepo
        //               .FirstOrDefaultAsync(acc => acc.UserName == userName);
        //        if (account == null)
        //        {
        //            _logger.LogWarning("Không tìm thấy tài khoản với User Name: {userName}", userName);
        //            return (Enumerable.Empty<GiaiTrinhGetViewModel>(), 0);
        //        }

        //        var maNhanVien = account.MaNhanVien;

        //        var allData = await _unitOfWork.VPCCTempRepo
        //            .GetByConditionAsync(x => x.madv == maNhanVien && x.loaiviphamid !=0);

        //        var totalCount = allData.Count();

        //        var pagedResult = allData
        //            .OrderByDescending(x => x.hoten)
        //            .Skip((pageIndex - 1) * pageSize)
        //            .Take(pageSize)
        //            .Select(x => new GiaiTrinhGetViewModel
        //            {
        //                Thu = x.ngay.HasValue ? GetThuFromDateTime(x.ngay.Value) : "",
        //                Ngay = x.ngay?.ToString("dd/MM/yyyy"),
        //                GioVao = x.time_in,
        //                GioRa = x.time_out,
        //                GiaiTrinh = 
        //            })
        //            .ToList();

        //        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        //        return (pagedResult, totalPages);
        //    }
        //    catch (Exception ex)
        //    {
        //        await _unitOfWork.RollbackTransactionAsync();
        //        _logger.LogError(ex, "Lỗi khi lấy danh sách chấm công có filter");
        //        return (Enumerable.Empty<GiaiTrinhGetViewModel>(), 0);
        //    }
        //}
        public async Task<(IEnumerable<GiaiTrinhGetViewModel> List, int TotalPages)> GetListGiaiTrinhAsync(int pageIndex, int pageSize, string userName, string tenKy = "")
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

                // 1. Lấy tất cả bản ghi vi phạm theo nhân viên
                var allDataQuery = await _unitOfWork.ViPhamChamCongVanTayRepo
                    .GetByConditionAsync(x => x.madv == maNhanVien && x.loaiviphamid != 0);

                // 2. Filter theo kỳ nếu có
                if (!string.IsNullOrEmpty(tenKy))
                {
                    // Lấy thông tin kỳ để có maky
                    var kyInfo = await _unitOfWork.KyBaoCaoRepo
                        .FirstOrDefaultAsync(k => k.tenky == tenKy);
                    
                    if (kyInfo != null)
                    {
                        allDataQuery = allDataQuery.Where(x => x.maky == kyInfo.maky);
                    }
                }

                var allData = allDataQuery.ToList();

                var totalCount = allData.Count();

                // 3. Phân trang
                var pagedData = allData
                    .OrderByDescending(x => x.ngay)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var viphamIds = pagedData.Select(x => x.viphamid).ToList();
                var maKyList = pagedData.Select(x => x.maky).ToList();

                // 4. Lấy dữ liệu giải trình theo các vi phạm được phân trang
                var giaiTrinhList = await _unitOfWork.GiaiTrinhChamCongRepo
                    .GetByConditionAsync(gt => viphamIds.Contains(gt.viphamid) && maKyList.Contains(gt.maky));

                // 5. Lấy danh sách lý do vi phạm
                var lyDoList = await _unitOfWork.LyDoViPhamRepo.GetAllAsync();

                // 6. Mapping kết quả ra ViewModel
                var pagedResult = pagedData.Select(x =>
                {
                    var giaiTrinh = giaiTrinhList
                        .FirstOrDefault(gt => gt.viphamid == x.viphamid && gt.maky == x.maky);

                    var lyDo = lyDoList
                        .FirstOrDefault(ld => ld.lydoid == giaiTrinh?.giaitrinhid);

                    return new GiaiTrinhGetViewModel
                    {
                        Thu = x.ngay.HasValue ? GetThuFromDateTime(x.ngay.Value) : "",
                        Ngay = x.ngay?.ToString("dd/MM/yyyy"),
                        GioVao = x.time_in,
                        GioRa = x.time_out,
                        GiaiTrinh = lyDo?.lydoid,
                        File = lyDo?.kemfile,
                        viphamid = x.viphamid,
                        maky = x.maky
                    };
                }).ToList();

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


        public async Task SaveGiaiTrinhAsync(GiaiTrinhChamCongViewModel giaiTrinhChamCongViewModel)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var existing = await _unitOfWork.GiaiTrinhChamCongRepo
                    .FirstOrDefaultAsync(x => x.viphamid == giaiTrinhChamCongViewModel.viphamid && x.maky == giaiTrinhChamCongViewModel.maky);

                if (existing != null)
                {
                    existing.giaitrinhid = giaiTrinhChamCongViewModel.giaitrinhid;
                    _unitOfWork.GiaiTrinhChamCongRepo.Update(existing);
                }
                else
                {
                    var newItem = new tb_giaitrinh_chamcong
                    {
                        viphamid = giaiTrinhChamCongViewModel.viphamid,
                        maky = giaiTrinhChamCongViewModel.maky,
                        giaitrinhid = giaiTrinhChamCongViewModel.giaitrinhid
                    };
                    await _unitOfWork.GiaiTrinhChamCongRepo.AddAsync(newItem);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<IEnumerable<LyDoViPhamGiaiTrinhGetViewModel>> GetListLidoAsync()
        {
            try
            {
                var getListLido = await _unitOfWork.LyDoViPhamRepo.GetAllAsync();
                var result = getListLido.Select(x => new LyDoViPhamGiaiTrinhGetViewModel
                {
                    lydoid = x.lydoid,
                    tenlydo = x.tenlydo,
                    kemfile = x.kemfile
                });
                return result;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Lỗi khi lấy danh sách chấm công có filter");
                return (Enumerable.Empty<LyDoViPhamGiaiTrinhGetViewModel>());
            }
        }

        public async Task<UserInfoViewModel?> GetUserInfoAsync(string userName)
        {
            try
            {
                var account = await _unitOfWork.AccountRepo
                    .FirstOrDefaultAsync(acc => acc.UserName == userName);

                if (account == null)
                {
                    _logger.LogWarning("Không tìm thấy tài khoản với User Name: {userName}", userName);
                    return null;
                }

                // Lấy thông tin đơn vị
                var donVi = await _unitOfWork.DonViRepo
                    .FirstOrDefaultAsync(dv => dv.madv == account.MaDv);

                return new UserInfoViewModel
                {
                    MaNhanVien = account.MaNhanVien,
                    HoTen = account.FullName,
                    ChucDanh = account.ChucVu,
                    DonVi = donVi?.tendv ?? "Chưa xác định",
                    UserName = account.UserName
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin user: {userName}", userName);
                return null;
            }
        }

    }
}
