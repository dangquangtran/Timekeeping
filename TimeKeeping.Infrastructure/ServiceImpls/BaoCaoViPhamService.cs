using ClosedXML.Excel;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeKeeping.Application.Helpers;
using TimeKeeping.Application.Interfaces;
using TimeKeeping.Application.Services;
using TimeKeeping.Application.ViewModels.BaoCaoViPhamViewModel;
using TimeKeeping.Application.ViewModels.ChamCongCheckInOutV1ViewModel;
using TimeKeeping.Application.ViewModels.GiaiTrinhViewModel;
using TimeKeeping.Domain.Entities;

namespace TimeKeeping.Infrastructure.ServiceImpls
{
    public class BaoCaoViPhamService : IBaoCaoViPhamService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<BaoCaoViPhamService> _logger;

        public BaoCaoViPhamService(ILogger<BaoCaoViPhamService> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<(IEnumerable<BaoCaoViPhamChamCongVanGetViewModel> List, int TotalPages)> GetListBaoCaoViPhamChamCongVanTayAsync(int pageIndex, int pageSize)
        {
            _logger.LogInformation("Bắt đầu lấy danh sách Chấm Công với bộ lọc");

            try
            {
                // 1. Lấy tất cả bản ghi vi phạm (lọc loại vi phạm khác 0)
                var allData = await _unitOfWork.ViPhamChamCongVanTayRepo
                    .GetByConditionAsync(x => x.loaiviphamid != 0);

                var totalCount = allData.Count();

                // 2. Lấy danh sách mã nhân viên từ vi phạm
                var allMaNhanViens = allData.Select(x => x.madv).Distinct().ToList();

                // 3. Lấy tất cả account có liên quan
                var allAccounts = await _unitOfWork.AccountRepo
                    .GetByConditionAsync(a => allMaNhanViens.Contains(a.MaNhanVien));

                // 4. Lấy tất cả mã đơn vị từ account
                var allMaDonVi = allAccounts.Select(a => a.MaDv).Distinct().ToList();

                // 5. Lấy thông tin các đơn vị
                var allDonVis = await _unitOfWork.DonViRepo
                    .GetByConditionAsync(dv => allMaDonVi.Contains(dv.madv));

                // 6. Phân trang dữ liệu vi phạm
                var pagedData = allData
                    .OrderByDescending(x => x.ngay) // Hoặc .OrderBy(x => x.hoten)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var viphamIds = pagedData.Select(x => x.viphamid).ToList();
                var maKyList = pagedData.Select(x => x.maky).ToList();

                // 7. Lấy danh sách giải trình theo vi phạm trong trang hiện tại
                var giaiTrinhList = await _unitOfWork.GiaiTrinhChamCongRepo
                    .GetByConditionAsync(gt => viphamIds.Contains(gt.viphamid) && maKyList.Contains(gt.maky));

                // 8. Lấy danh sách lý do
                var lyDoList = await _unitOfWork.LyDoViPhamRepo.GetAllAsync();

                // 9. Mapping kết quả ra ViewModel
                var pagedResult = pagedData.Select(x =>
                {
                    var giaiTrinh = giaiTrinhList
                        .FirstOrDefault(gt => gt.viphamid == x.viphamid && gt.maky == x.maky);

                    var lyDo = lyDoList
                        .FirstOrDefault(ld => ld.lydoid == giaiTrinh?.giaitrinhid);

                    var account = allAccounts.FirstOrDefault(a => a.MaNhanVien == x.madv);
                    var tenDonVi = allDonVis.FirstOrDefault(dv => dv.madv == account?.MaDv)?.tendv ?? "";

                    return new BaoCaoViPhamChamCongVanGetViewModel
                    {
                        MaNv = x.madv,
                        HoTen = x.hoten,
                        ChucVu= x.chucdanh,
                        DonVi = tenDonVi,
                        Thu = x.ngay.HasValue ? GetThuFromDateTime(x.ngay.Value) : "",
                        Ngay = x.ngay?.ToString("dd/MM/yyyy"),
                        GioVao = x.time_in,
                        GioRa = x.time_out,
                        GiaiTrinh = lyDo?.tenlydo ?? "",
                        TrangThai = string.IsNullOrWhiteSpace(lyDo?.tenlydo ?? "") ? "Chưa giải trình" : "Đã giải trình",
                        PhanLoai = "Vi phạm"
                    };
                }).ToList();

                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                return (pagedResult, totalPages);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Lỗi khi lấy danh sách chấm công có filter");
                return (Enumerable.Empty<BaoCaoViPhamChamCongVanGetViewModel>(), 0);
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


        private async Task<IEnumerable<tb_viphamchamcongvantay>> FilterViPhamAsync(string? maDonVi, string? maKy)
        {
            // 1. Lấy tất cả vi phạm với loại vi phạm khác 0
            var allViPhams = await _unitOfWork.ViPhamChamCongVanTayRepo
                .GetByConditionAsync(x => x.loaiviphamid != 0);

            // Nếu không có điều kiện filter nào, trả về luôn
            if (string.IsNullOrWhiteSpace(maDonVi) && string.IsNullOrWhiteSpace(maKy))
                return allViPhams;

            // 2. Lấy mã nhân viên từ vi phạm
            var maNhanViens = allViPhams.Select(x => x.madv).Distinct().ToList();

            // 3. Lấy danh sách account theo mã nhân viên
            var accounts = await _unitOfWork.AccountRepo
                .GetByConditionAsync(a => maNhanViens.Contains(a.MaNhanVien));

            // 4. Chuyển maKy từ string -> int? để so sánh với x.maky
            int? maKyInt = null;
            if (!string.IsNullOrWhiteSpace(maKy) && int.TryParse(maKy, out var parsedMaKy))
            {
                maKyInt = parsedMaKy;
            }

            // 5. Lọc dữ liệu
            return allViPhams.Where(x =>
            {
                var acc = accounts.FirstOrDefault(a => a.MaNhanVien == x.madv);

                var matchDonVi = string.IsNullOrWhiteSpace(maDonVi) || acc?.MaDv == maDonVi;
                var matchMaKy = maKyInt == null || x.maky == maKyInt;

                return matchDonVi && matchMaKy;
            });
        }

        public async Task<(IEnumerable<BaoCaoViPhamChamCongVanGetViewModel> List, int TotalPages)>
    GetListBaoCaoViPhamChamCongVanTayFilterAsync(int pageIndex, int pageSize, string? tenDonVi = null, string? tenKy = null)
        {
            _logger.LogInformation("Bắt đầu lấy danh sách Chấm Công với bộ lọc");

            try
            {
                // 1. Lấy toàn bộ dữ liệu vi phạm
                var allData = await _unitOfWork.ViPhamChamCongVanTayRepo
                    .GetByConditionAsync(x => x.loaiviphamid != 0);

                var allMaNhanViens = allData.Select(x => x.madv).Distinct().ToList();

                // 2. Lấy tài khoản nhân viên
                var allAccounts = await _unitOfWork.AccountRepo
                    .GetByConditionAsync(a => allMaNhanViens.Contains(a.MaNhanVien));

                var allMaDonVis = allAccounts.Select(a => a.MaDv).Distinct().ToList();

                // 3. Lấy thông tin đơn vị
                var allDonVis = await _unitOfWork.DonViRepo
                    .GetByConditionAsync(dv => allMaDonVis.Contains(dv.madv));

                // 4. Nếu có tên đơn vị thì ánh xạ thành mã đơn vị
                string? maDonViFilter = null;
                if (!string.IsNullOrWhiteSpace(tenDonVi))
                {
                    maDonViFilter = allDonVis.FirstOrDefault(dv => dv.tendv == tenDonVi)?.madv;
                }
                int? thangFilter = null;
                int? namFilter = null;
                if (!string.IsNullOrWhiteSpace(tenKy) &&
                    KyParserHelper.TryParseKy(tenKy, out int thang, out int nam))
                {
                    thangFilter = thang;
                    namFilter = nam;
                    _logger.LogInformation($"Tên kỳ: {tenKy} ánh xạ thành Tháng: {thang}, Năm: {nam}");
                }
                // 5. Lọc dữ liệu
                var filteredData = allData.Where(x =>
                {
                    var acc = allAccounts.FirstOrDefault(a => a.MaNhanVien == x.madv);
                    var matchDonVi = string.IsNullOrWhiteSpace(tenDonVi) || acc?.MaDv == maDonViFilter;
                    //var matchMaKy = string.IsNullOrWhiteSpace(tenKy) || x.maky == maKyInt;
                    bool matchKy = true;
                    if (thangFilter.HasValue && namFilter.HasValue && x.ngay.HasValue)
                    {
                        var ngay = x.ngay.Value;
                        matchKy = ngay.Month == thangFilter.Value && ngay.Year == namFilter.Value;
                    }

                    return matchDonVi && matchKy;
                }).ToList();

                var totalCount = filteredData.Count();

                // 6. Phân trang
                var pagedData = filteredData
                    .OrderByDescending(x => x.ngay)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var viphamIds = pagedData.Select(x => x.viphamid).ToList();
                var maKyList = pagedData.Select(x => x.maky).ToList();

                // 7. Lấy dữ liệu giải trình
                var giaiTrinhList = await _unitOfWork.GiaiTrinhChamCongRepo
                    .GetByConditionAsync(gt => viphamIds.Contains(gt.viphamid) && maKyList.Contains(gt.maky));

                // 8. Lý do vi phạm
                var lyDoList = await _unitOfWork.LyDoViPhamRepo.GetAllAsync();

                // 9. Mapping kết quả
                var pagedResult = pagedData.Select(x =>
                {
                    var giaiTrinh = giaiTrinhList.FirstOrDefault(gt => gt.viphamid == x.viphamid && gt.maky == x.maky);
                    var lyDo = lyDoList.FirstOrDefault(ld => ld.lydoid == giaiTrinh?.giaitrinhid);
                    var account = allAccounts.FirstOrDefault(a => a.MaNhanVien == x.madv);
                    var tenDonViResult = allDonVis.FirstOrDefault(dv => dv.madv == account?.MaDv)?.tendv ?? "";

                    var tenLyDo = lyDo?.tenlydo ?? "";
                    var trangThai = string.IsNullOrWhiteSpace(tenLyDo) ? "Chưa giải trình" : "Đã giải trình";

                    return new BaoCaoViPhamChamCongVanGetViewModel
                    {
                        MaNv = x.madv,
                        HoTen = x.hoten,
                        ChucVu = x.chucdanh,
                        DonVi = tenDonViResult,
                        Thu = x.ngay.HasValue ? GetThuFromDateTime(x.ngay.Value) : "",
                        Ngay = x.ngay?.ToString("dd/MM/yyyy"),
                        GioVao = x.time_in,
                        GioRa = x.time_out,
                        GiaiTrinh = tenLyDo,
                        TrangThai = trangThai,
                        PhanLoai = "Vi phạm"
                    };
                }).ToList();

                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                return (pagedResult, totalPages);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Lỗi khi lấy danh sách chấm công có filter");
                return (Enumerable.Empty<BaoCaoViPhamChamCongVanGetViewModel>(), 0);
            }
        }

        public async Task<byte[]> ExportBaoCaoViPhamChamCongVanTayToExcelAsync(BaoCaoViPhamExportRequestViewModel request)
        {
            _logger.LogInformation("Bắt đầu export báo cáo vi phạm chấm công vân tay ra Excel");

            try
            {
                var data = await _unitOfWork.ViPhamChamCongVanTayRepo
                    .GetByConditionAsync(x => x.loaiviphamid != 0);

                var maNhanViens = data.Select(x => x.madv).Distinct().ToList();
                var accounts = await _unitOfWork.AccountRepo
                    .GetByConditionAsync(a => maNhanViens.Contains(a.MaNhanVien));
                var donVis = await _unitOfWork.DonViRepo
                    .GetByConditionAsync(dv => accounts.Select(a => a.MaDv).Contains(dv.madv));

                var accountDict = accounts.ToDictionary(a => a.MaNhanVien);
                var donViDict = donVis.ToDictionary(d => d.madv, d => d.tendv);

                // Parse kỳ
                int? thangFilter = null;
                int? namFilter = null;
                if (!string.IsNullOrWhiteSpace(request.Ky) &&
                    KyParserHelper.TryParseKy(request.Ky, out int thang, out int nam))
                {
                    thangFilter = thang;
                    namFilter = nam;
                }

                // Kiểm tra đơn vị nếu được yêu cầu lọc
                if (!string.IsNullOrEmpty(request.TenDonVi))
                {
                    bool donViTonTai = donViDict.Values.Any(tdv => tdv != null &&
                        tdv.Contains(request.TenDonVi, StringComparison.OrdinalIgnoreCase));
                    if (!donViTonTai)
                    {
                        _logger.LogWarning("Không tìm thấy đơn vị có tên chứa: {TenDonVi}", request.TenDonVi);
                        return Array.Empty<byte>();
                    }
                }

                // Lọc dữ liệu
                var filteredData = data.Where(x =>
                {
                    var account = accountDict.GetValueOrDefault(x.madv);
                    if (account == null) return false;

                    var donViName = donViDict.GetValueOrDefault(account.MaDv);
                    var matchDonVi = string.IsNullOrEmpty(request.TenDonVi) ||
                                     (donViName != null && donViName.Contains(request.TenDonVi, StringComparison.OrdinalIgnoreCase));

                    var matchKy = (!thangFilter.HasValue || !namFilter.HasValue || !x.ngay.HasValue)
                        || (x.ngay.Value.Month == thangFilter && x.ngay.Value.Year == namFilter);

                    return matchDonVi && matchKy;
                }).ToList();

                if (!filteredData.Any())
                {
                    _logger.LogWarning("Không có bản ghi vi phạm nào sau khi lọc theo đơn vị: {TenDonVi}", request.TenDonVi);
                    return Array.Empty<byte>();
                }

                var viphamIds = filteredData.Select(x => x.viphamid).ToList();
                var maKyList = filteredData.Select(x => x.maky).ToList();

                var giaiTrinhList = await _unitOfWork.GiaiTrinhChamCongRepo
                    .GetByConditionAsync(gt => viphamIds.Contains(gt.viphamid) && maKyList.Contains(gt.maky));

                var lyDoList = await _unitOfWork.LyDoViPhamRepo.GetAllAsync();

                var result = filteredData.Select(x =>
                {
                    var giaiTrinh = giaiTrinhList.FirstOrDefault(gt => gt.viphamid == x.viphamid && gt.maky == x.maky);
                    var lyDo = lyDoList.FirstOrDefault(ld => ld.lydoid == giaiTrinh?.giaitrinhid);
                    var account = accountDict.GetValueOrDefault(x.madv);
                    var donViName = account != null ? donViDict.GetValueOrDefault(account.MaDv) : null;

                    return new BaoCaoViPhamChamCongVanGetViewModel
                    {
                        MaNv = x.madv,
                        HoTen = x.hoten,
                        ChucVu = x.chucdanh,
                        DonVi = donViName,
                        Thu = x.ngay.HasValue ? GetThuFromDateTime(x.ngay.Value) : "",
                        Ngay = x.ngay?.ToString("dd/MM/yyyy"),
                        GioVao = x.time_in,
                        GioRa = x.time_out,
                        GiaiTrinh = lyDo?.tenlydo ?? "",
                        TrangThai = string.IsNullOrWhiteSpace(lyDo?.tenlydo) ? "Chưa giải trình" : "Đã giải trình",
                        PhanLoai = "Vi phạm"
                    };
                }).ToList();

                // Export Excel
                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("BaoCaoViPham");

                worksheet.Cell(1, 1).Value = "STT";
                worksheet.Cell(1, 2).Value = "Mã NV";
                worksheet.Cell(1, 3).Value = "Họ Tên";
                worksheet.Cell(1, 4).Value = "Chức Vụ";
                worksheet.Cell(1, 5).Value = "Đơn Vị";
                worksheet.Cell(1, 6).Value = "Thứ";
                worksheet.Cell(1, 7).Value = "Ngày";
                worksheet.Cell(1, 8).Value = "Giờ Vào";
                worksheet.Cell(1, 9).Value = "Giờ Ra";
                worksheet.Cell(1, 10).Value = "Giải Trình";
                worksheet.Cell(1, 11).Value = "Trạng Thái";
                worksheet.Cell(1, 12).Value = "Phân Loại";

                for (int i = 0; i < result.Count; i++)
                {
                    var row = i + 2;
                    var item = result[i];

                    worksheet.Cell(row, 1).Value = i + 1;
                    worksheet.Cell(row, 2).Value = item.MaNv;
                    worksheet.Cell(row, 3).Value = item.HoTen;
                    worksheet.Cell(row, 4).Value = item.ChucVu;
                    worksheet.Cell(row, 5).Value = item.DonVi;
                    worksheet.Cell(row, 6).Value = item.Thu;
                    worksheet.Cell(row, 7).Value = item.Ngay;
                    worksheet.Cell(row, 8).Value = item.GioVao;
                    worksheet.Cell(row, 9).Value = item.GioRa;
                    worksheet.Cell(row, 10).Value = item.GiaiTrinh;
                    worksheet.Cell(row, 11).Value = item.TrangThai;
                    worksheet.Cell(row, 12).Value = item.PhanLoai;
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi export báo cáo vi phạm chấm công ra Excel");
                return Array.Empty<byte>();
            }
        }

        public async Task<IEnumerable<ListKyGetViewModel>> GetAllKyAsync()
        {
            _logger.LogInformation("Bắt đầu lấy các kỳ cho báo cáo vi phạm");
            try
            {
                var danhSachKy = await _unitOfWork.KyBaoCaoRepo.GetAllAsync();

                var result = danhSachKy.Select(x => new ListKyGetViewModel
                {
                    ky = x.tenky,
                }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy các kỳ cho báo cáo vi phạm");
                return Enumerable.Empty<ListKyGetViewModel>();
            }
        }

        public async Task<IEnumerable<ListDonViGetViewModel>> GetAllDonViAsync()
        {
            _logger.LogInformation("Bắt đầu lấy các đơn vị cho báo cáo vi phạm");
            try
            {
                var danhSachDonVi = await _unitOfWork.DonViRepo.GetAllAsync();

                var result = danhSachDonVi.Select(x => new ListDonViGetViewModel
                {
                    DonVi = x.tendv,
                }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy các đơn vị cho báo cáo vi phạm");
                return Enumerable.Empty<ListDonViGetViewModel>();
            }
        }


        public async Task<CurrentKyInfoViewModel?> GetCurrentKyInfoAsync(string? tenKy)
        {
            _logger.LogInformation("Bắt đầu lấy thông tin kỳ hiện tại");
            try
            {
                if (string.IsNullOrWhiteSpace(tenKy))
                {
                    // Nếu không có kỳ được chọn, lấy kỳ mới nhất
                    var latestKy = await _unitOfWork.KyBaoCaoRepo.GetAllAsync();
                    var latest = latestKy.OrderByDescending(k => k.denngay).FirstOrDefault();

                    if (latest != null)
                    {
                        return new CurrentKyInfoViewModel
                        {
                            TenKy = latest.tenky,
                            TuNgay = latest.tungay,
                            DenNgay = latest.denngay
                        };
                    }
                }
                else
                {
                    // Lấy thông tin kỳ được chọn
                    var kyInfo = await _unitOfWork.KyBaoCaoRepo
                        .FirstOrDefaultAsync(k => k.tenky == tenKy);

                    if (kyInfo != null)
                    {
                        return new CurrentKyInfoViewModel
                        {
                            TenKy = kyInfo.tenky,
                            TuNgay = kyInfo.tungay,
                            DenNgay = kyInfo.denngay
                        };
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin kỳ hiện tại");
                return null;
            }
        }
    }
}
