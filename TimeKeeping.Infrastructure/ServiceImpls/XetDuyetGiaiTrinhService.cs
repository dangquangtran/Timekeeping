using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeKeeping.Application.Interfaces;
using TimeKeeping.Application.Services;
using TimeKeeping.Application.ViewModels.GiaiTrinhViewModel;
using TimeKeeping.Application.ViewModels.XetDuyetViewModel;

namespace TimeKeeping.Infrastructure.ServiceImpls
{
    public class XetDuyetGiaiTrinhService : IXetDuyetGiaiTrinhService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<XetDuyetGiaiTrinhService> _logger;

        public XetDuyetGiaiTrinhService(ILogger<XetDuyetGiaiTrinhService> logger,
                                       IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<(IEnumerable<XetDuyetGetViewModel> List, int TotalPages)> GetListGiaiTrinhAsync(int pageIndex, int pageSize, XetDuyetGiaiTrinhFilter filter)
        {
            _logger.LogInformation("Bắt đầu lấy danh sách giải trình cho xét duyệt (có filter)");
            try
            {
                var allData = (await _unitOfWork.ViPhamChamCongVanTayRepo
                    .GetByConditionAsync(x => x.loaiviphamid != 0)).ToList();

                // Lấy danh sách account và đơn vị để join
                var allAccounts = (await _unitOfWork.AccountRepo.GetAllAsync()).ToList();
                var allDonVi = (await _unitOfWork.DonViRepo.GetAllAsync()).ToList();

                // Filter theo tên đơn vị nếu có
                if (!string.IsNullOrEmpty(filter?.DonVi))
                {
                    // Join lấy tendv từ madv (mã nhân viên => account.MaDv => donvi.tendv)
                    var nhanVienToDonVi = allAccounts
                        .Where(a => !string.IsNullOrEmpty(a.MaNhanVien))
                        .ToDictionary(a => a.MaNhanVien, a => a.MaDv);
                    var donViDict = allDonVi.ToDictionary(dv => dv.madv, dv => dv.tendv);

                    allData = allData.Where(x =>
                    {
                        if (x.madv == null) return false;
                        if (!nhanVienToDonVi.TryGetValue(x.madv, out var maDonVi)) return false;
                        if (!donViDict.TryGetValue(maDonVi, out var tenDonVi)) return false;
                        return tenDonVi.Contains(filter.DonVi, StringComparison.OrdinalIgnoreCase);
                    }).ToList();
                }

                // Filter theo kỳ nếu có
                if (!string.IsNullOrEmpty(filter?.Ky))
                {
                    var kyNormalized = filter.Ky.Normalize(NormalizationForm.FormC).Trim();
                    var kyParts = kyNormalized.Split(' ');
                    if (kyParts.Length == 2 && kyParts[0].Equals("Tháng", StringComparison.OrdinalIgnoreCase))
                    {
                        var thangNam = kyParts[1].Split('/');
                        if (thangNam.Length == 2 && int.TryParse(thangNam[0], out int thang) && int.TryParse(thangNam[1], out int nam))
                        {
                            allData = allData.Where(x => x.ngay.HasValue && x.ngay.Value.Month == thang && x.ngay.Value.Year == nam).ToList();
                        }
                    }
                }

                var totalCount = allData.Count();

                var pagedData = allData
                    .OrderByDescending(x => x.hoten)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var viphamIds = pagedData.Select(x => x.viphamid).ToList();
                var maKyList = pagedData.Select(x => x.maky).ToList();

                var giaiTrinhList = await _unitOfWork.GiaiTrinhChamCongRepo
                    .GetByConditionAsync(gt => viphamIds.Contains(gt.viphamid) && maKyList.Contains(gt.maky));

                var lyDoList = await _unitOfWork.LyDoViPhamRepo.GetAllAsync();

                var pagedResult = pagedData.Select(x =>
                {
                    var giaiTrinh = giaiTrinhList
                        .FirstOrDefault(gt => gt.viphamid == x.viphamid && gt.maky == x.maky);

                    var lyDo = lyDoList
                        .FirstOrDefault(ld => ld.lydoid == giaiTrinh?.giaitrinhid);

                    string statusStr = "Chưa Xét Duyệt";
                    if (giaiTrinh?.Status == true)
                        statusStr = "Đã Xét Duyệt";

                    return new XetDuyetGetViewModel
                    {
                        Thu = x.ngay.HasValue ? GetThuFromDateTime(x.ngay.Value) : "",
                        Ngay = x.ngay?.ToString("dd/MM/yyyy"),
                        GioVao = x.time_in,
                        GioRa = x.time_out,
                        GiaiTrinh = lyDo?.lydoid,
                        File = lyDo?.kemfile,
                        viphamid = x.viphamid,
                        maky = x.maky,
                        Status = statusStr
                    };
                }).ToList();

                // Filter theo Status nếu có
                if (!string.IsNullOrEmpty(filter?.Status))
                {
                    string Normalize(string input) => input == null ? null : input.Normalize(NormalizationForm.FormC).Trim();
                    pagedResult = pagedResult.Where(x =>
                        Normalize(x.Status)?.Equals(Normalize(filter.Status), StringComparison.OrdinalIgnoreCase) == true
                    ).ToList();
                }

                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                return (pagedResult, totalPages);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Lỗi khi lấy danh sách giải trình cho xét duyệt");
                return (Enumerable.Empty<XetDuyetGetViewModel>(), 0);
            }
        }

        public async Task<int> ApproveAllChuaXetDuyetAsync()
        {
            _logger.LogInformation("Bắt đầu duyệt tất cả giải trình chưa xét duyệt");
            try
            {
                var chuaXetDuyetList = await _unitOfWork.GiaiTrinhChamCongRepo
                    .GetByConditionAsync(x => x.Status == false || x.Status == null);

                int count = 0;
                foreach (var item in chuaXetDuyetList)
                {
                    item.Status = true;
                    _unitOfWork.GiaiTrinhChamCongRepo.Update(item);
                    count++;
                }
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
                return count;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Lỗi khi duyệt tất cả giải trình chưa xét duyệt");
                throw;
            }
        }

        public async Task<int> ApproveSelectedAsync(IEnumerable<(string viphamid, int? maky)> ids)
        {
            _logger.LogInformation("Bắt đầu duyệt các giải trình được chọn");
            try
            {
                int count = 0;
                foreach (var (viphamid, maky) in ids)
                {
                    var item = await _unitOfWork.GiaiTrinhChamCongRepo.FirstOrDefaultAsync(x => x.viphamid == viphamid && x.maky == maky);
                    if (item != null && (item.Status == false || item.Status == null))
                    {
                        item.Status = true;
                        _unitOfWork.GiaiTrinhChamCongRepo.Update(item);
                        count++;
                    }
                }
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
                return count;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Lỗi khi duyệt các giải trình được chọn");
                throw;
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
