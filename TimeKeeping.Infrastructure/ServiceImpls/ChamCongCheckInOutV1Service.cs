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
                }).OrderBy(vm => vm.HoTen)
        .ThenByDescending(vm => DateTime.ParseExact(vm.NgayCham, "dd/MM/yyyy", null))
        .ToList();

                return result;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Lỗi khi lấy danh sách chấm công");
                return Enumerable.Empty<ChamCongCheckInOutV1GetViewModel>();
            }
        }

        public async Task<byte[]> ExportChamCongToExcelAsync(ChamCongExportRequestViewModel request)
        {
            _logger.LogInformation("Bắt đầu export dữ liệu chấm công ra Excel");

            try
            {
                if (!KyParserHelper.TryParseKy(request.Ky, out int thang, out int nam))
                {
                    _logger.LogWarning("Kỳ không hợp lệ: {Ky}", request.Ky);
                    return Array.Empty<byte>();
                }
                // Lấy toàn bộ dữ liệu chấm công trong tháng/năm
                var data = await _unitOfWork.ChamCong_CheckInOut_v1Repo
                    .GetByConditionAsync(c => c.NgayCham.HasValue &&
                                              c.NgayCham.Value.Month == thang &&
                                              c.NgayCham.Value.Year == nam);

                if (!data.Any())
                {
                    _logger.LogWarning("Không có dữ liệu chấm công cho tháng {Thang} năm {Nam}", thang, nam);
                    return Array.Empty<byte>();
                }

                var maNhanViens = data.Select(c => c.MaNhanVien).Distinct().ToList();
                var accounts = await _unitOfWork.AccountRepo.GetByConditionAsync(a => maNhanViens.Contains(a.MaNhanVien));
                var donVis = await _unitOfWork.DonViRepo.GetByConditionAsync(d => accounts.Select(a => a.MaDv).Contains(d.madv));

                // Dictionary ánh xạ
                var accountDict = accounts.ToDictionary(a => a.MaNhanVien);
                var donViDict = donVis.ToDictionary(d => d.madv, d => d.tendv);

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

                // Lọc theo tên đơn vị (nếu có)
                var filteredData = data.Where(c =>
                {
                    var account = accountDict.GetValueOrDefault(c.MaNhanVien);
                    if (account == null) return false;

                    var donViName = donViDict.GetValueOrDefault(account.MaDv);
                    return string.IsNullOrEmpty(request.TenDonVi) ||
                           (donViName != null && donViName.Contains(request.TenDonVi, StringComparison.OrdinalIgnoreCase));
                }).ToList();

                if (!filteredData.Any())
                {
                    _logger.LogWarning("Không có bản ghi chấm công nào sau khi lọc theo đơn vị: {TenDonVi}", request.TenDonVi);
                    return Array.Empty<byte>();
                }

                // Chuyển sang ViewModel
                var result = filteredData.Select(c =>
                {
                    var account = accountDict.GetValueOrDefault(c.MaNhanVien);
                    var donVi = account != null ? donViDict.GetValueOrDefault(account.MaDv) : null;
                    var formattedNgayCham = c.NgayCham?.ToString("dd/MM/yyyy");
                    var formattedGioCham = c.GioCham?.ToString("HH:mm:ss");

                    return new ChamCongCheckInOutV1GetViewModel
                    {
                        MaChamCong = c.ID,
                        HoTen = account?.FullName,
                        ChucDanh = account?.ChucVu,
                        DonVi = donVi,
                        NgayCham = formattedNgayCham,
                        GioCham = formattedGioCham,
                        TenMay = c.TenMay
                    };
                }).ToList();

                // Export Excel
                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("ChamCong");

                worksheet.Cell(1, 1).Value = "Mã Chấm Công";
                worksheet.Cell(1, 2).Value = "Họ Tên";
                worksheet.Cell(1, 3).Value = "Chức Danh";
                worksheet.Cell(1, 4).Value = "Đơn Vị";
                worksheet.Cell(1, 5).Value = "Ngày Chấm";
                worksheet.Cell(1, 6).Value = "Giờ Chấm";
                worksheet.Cell(1, 7).Value = "Tên Máy";

                for (int i = 0; i < result.Count; i++)
                {
                    var r = result[i];
                    worksheet.Cell(i + 2, 1).Value = r.MaChamCong;
                    worksheet.Cell(i + 2, 2).Value = r.HoTen;
                    worksheet.Cell(i + 2, 3).Value = r.ChucDanh;
                    worksheet.Cell(i + 2, 4).Value = r.DonVi;
                    worksheet.Cell(i + 2, 5).Value = r.NgayCham;
                    worksheet.Cell(i + 2, 6).Value = r.GioCham;
                    worksheet.Cell(i + 2, 7).Value = r.TenMay;
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi export dữ liệu chấm công ra Excel");
                return Array.Empty<byte>();
            }
        }

        public async Task<IEnumerable<ListKyGetViewModel>> GetAllKyAsync()
        {
            _logger.LogInformation("Bắt đầu lấy các kỳ");
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
                _logger.LogError(ex, "Lỗi khi lấy các kỳ ");
                return Enumerable.Empty<ListKyGetViewModel>();
            }
        }

        public async Task<IEnumerable<ListDonViGetViewModel>> GetAllDonViAsync()
        {
            _logger.LogInformation("Bắt đầu lấy các đơn vị");
            try
            {
                var danhSachKy = await _unitOfWork.DonViRepo.GetAllAsync();

                var result = danhSachKy.Select(x => new ListDonViGetViewModel
                {
                    DonVi = x.tendv,

                }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy các đơn vị");
                return Enumerable.Empty<ListDonViGetViewModel>();
            }
        }

        public async Task<(IEnumerable<ChamCongCheckInOutV1GetViewModel> List, int TotalPages)> GetFilteredAsync(int pageIndex, int pageSize, FilterChamCongViewModel filter)
        {
            _logger.LogInformation("Bắt đầu lấy danh sách Chấm Công với bộ lọc");

            try
            {
                //var (pagedData, totalCount) = await _unitOfWork.ChamCong_CheckInOut_v1Repo.GetPagedAsync(pageIndex, pageSize);
                var allData = await _unitOfWork.ChamCong_CheckInOut_v1Repo.GetAllAsync();
                // Lấy mã nhân viên
                var maNhanViens = allData.Select(c => c.MaNhanVien).Distinct().ToList();
                var accounts = await _unitOfWork.AccountRepo.GetByConditionAsync(a => maNhanViens.Contains(a.MaNhanVien));
                var donVis = await _unitOfWork.DonViRepo.GetByConditionAsync(d => accounts.Select(a => a.MaDv).Contains(d.madv));

                var accountDict = accounts.ToDictionary(a => a.MaNhanVien);
                var donViDict = donVis.ToDictionary(d => d.madv, d => d.tendv);

                // Ánh xạ dữ liệu có DonVi để dùng filter
                var mappedData = allData.Select(c =>
                {
                    var acc = accountDict.GetValueOrDefault(c.MaNhanVien);
                    var donVi = acc != null ? donViDict.GetValueOrDefault(acc.MaDv) : null;
                    return new
                    {
                        ChamCong = c,
                        Account = acc,
                        DonVi = donVi
                    };
                });

                // Filter theo kỳ
                if (!string.IsNullOrWhiteSpace(filter.Ky) &&
                    KyParserHelper.TryParseKy(filter.Ky, out int thang, out int nam))
                {
                    mappedData = mappedData.Where(x => x.ChamCong.NgayCham.HasValue &&
                                                       x.ChamCong.NgayCham.Value.Month == thang &&
                                                       x.ChamCong.NgayCham.Value.Year == nam);
                }

                // Filter theo tên đơn vị
                if (!string.IsNullOrWhiteSpace(filter.DonVi))
                {
                    var donViFilter = filter.DonVi.Trim().ToLower();
                    mappedData = mappedData.Where(x => !string.IsNullOrWhiteSpace(x.DonVi) &&
                                                       x.DonVi.ToLower().Contains(donViFilter));
                }

                // Sau khi filter, tính lại totalCount & phân trang lại (trong RAM)
                var filteredList = mappedData.ToList();
                var totalFiltered = filteredList.Count;
                var pagedResult = filteredList
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x =>
                    {
                        return new ChamCongCheckInOutV1GetViewModel
                        {
                            MaChamCong = x.ChamCong.ID,
                            HoTen = x.Account?.FullName,
                            ChucDanh = x.Account?.ChucVu,
                            DonVi = x.DonVi,
                            NgayCham = x.ChamCong.NgayCham?.ToString("dd/MM/yyyy"),
                            GioCham = x.ChamCong.GioCham?.ToString("HH:mm:ss"),
                            TenMay = x.ChamCong.TenMay,
                        };
                    }).ToList();

                var totalPages = (int)Math.Ceiling(totalFiltered / (double)pageSize);

                return (pagedResult, totalPages);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Lỗi khi lấy danh sách chấm công có filter");
                return (Enumerable.Empty<ChamCongCheckInOutV1GetViewModel>(), 0);
            }
        }


    }
}
