using DocumentFormat.OpenXml.Office2021.DocumentTasks;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using TimeKeeping.Application.Exceptions;
using TimeKeeping.Application.Interfaces;
using TimeKeeping.Application.Services;
using TimeKeeping.Application.ViewModels.AccountViewModel;
using TimeKeeping.Application.ViewModels.BaoCaokiViewModel;
using TimeKeeping.Application.ViewModels.LyDoViPhamViewModel;
using TimeKeeping.Domain.Entities;

namespace TimeKeeping.Infrastructure.ServiceImpls
{
    public class BaoCaokiService : IBaoCaokiService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<BaoCaokiService> _logger;

        public BaoCaokiService(IUnitOfWork unitOfWork,
                               ILogger<BaoCaokiService> logger)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<(IEnumerable<BaoCaokiGetViewModel> List, int TotalCount)> GetAllAsync(int pageIndex, int pageSize)
        {
            _logger.LogInformation("Bắt đầu lấy báo cáo kì");
            try
            {
                var (pagedData, totalCount) = await _unitOfWork.KyBaoCaoRepo.GetPagedAsync(pageIndex, pageSize);
                var trangthaiList = await _unitOfWork.TrangThaiKyRepo.GetAllAsync();
                var today = DateOnly.FromDateTime(DateTime.Now);

                var result = pagedData.Select(c => new BaoCaokiGetViewModel
                {
                    maky = c.maky,
                    tenky = c.tenky,
                    tungay = c.tungay,
                    denngay = c.denngay,
                    ngaydong_giaitrinh = c.ngaydong_giaitrinh,
                    ngay_duyet = c.ngay_duyet,
                    ngaydong_auto = c.ngaydong_auto,
                    TrangThai = (c.ngaydong_auto.HasValue && today >= c.ngaydong_auto.Value)
                        ? trangthaiList.FirstOrDefault(t => t.trangthaiid == 2)?.trangthaiten
                        : trangthaiList.FirstOrDefault(t => t.trangthaiid == 1)?.trangthaiten
                })
                .OrderByDescending(vm => vm.denngay ?? DateOnly.MinValue)
                .ThenBy(cm => cm.TrangThai == "Mở" ? 0 : 1)
                .ToList();

                return (result, totalCount);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }


        public async Task<IEnumerable<BaoCaokiCreateViewModel>> CreateBaoCaoKiAsync(BaoCaokiCreateViewModel baoCaokiCreateViewModel)
        {
            _logger.LogInformation("Bắt đầu tạo báo cáo kì");
            try
            {

                // Kiểm tra tungay và denngay
                if (!baoCaokiCreateViewModel.tungay.HasValue || !baoCaokiCreateViewModel.denngay.HasValue)
                {
                    throw new Exception("'tungay' và 'denngay' là bắt buộc.");
                }
                var fromDate = baoCaokiCreateViewModel.tungay.Value;
                var toDate = baoCaokiCreateViewModel.denngay.Value;

                if (fromDate.Month != toDate.Month || fromDate.Year != toDate.Year)
                {
                    throw new Exception("'tungay' và 'denngay' phải nằm trong cùng một tháng và năm.");
                }

                var generatedTenKy = $"Tháng {fromDate.Month}/{fromDate.Year}";


                var existingTenKi = await _unitOfWork.KyBaoCaoRepo.FirstOrDefaultAsync(a => a.tenky == generatedTenKy);

                if (existingTenKi != null)
                {
                    _logger.LogWarning("Tên Kì đã tồn tại: " + baoCaokiCreateViewModel.tenky);
                    throw new Exception("Tên Kì đã tồn tại.");
                }

                var dataInMonth = await _unitOfWork.ChamCong_CheckInOut_v1Repo.GetByConditionAsync(x =>
                                 x.NgayCham >= fromDate && x.NgayCham <= toDate);

                if (!dataInMonth.Any())
                {
                    throw new Exception($"Không có dữ liệu chấm công nào trong tháng {fromDate.Month}/{fromDate.Year}.");
                }

                var newBaoCaoKi = new tb_kybaocao
                {
                    // maky = baoCaokiCreateViewModel.maky,
                    tenky = generatedTenKy,
                    tungay = baoCaokiCreateViewModel.tungay.HasValue ? DateOnly.FromDateTime(baoCaokiCreateViewModel.tungay.Value) : (DateOnly?)null,
                    denngay = baoCaokiCreateViewModel.denngay.HasValue ? DateOnly.FromDateTime(baoCaokiCreateViewModel.denngay.Value) : (DateOnly?)null,
                    ngaydong_giaitrinh = baoCaokiCreateViewModel.ngaydong_giaitrinh.HasValue ? DateOnly.FromDateTime(baoCaokiCreateViewModel.ngaydong_giaitrinh.Value) : (DateOnly?)null,
                    ngay_duyet = baoCaokiCreateViewModel.ngay_duyet.HasValue ? DateOnly.FromDateTime(baoCaokiCreateViewModel.ngay_duyet.Value) : (DateOnly?)null,
                    ngaydong_auto = baoCaokiCreateViewModel.ngaydong_auto.HasValue ? DateOnly.FromDateTime(baoCaokiCreateViewModel.ngaydong_auto.Value) : (DateOnly?)null,
                };

                await _unitOfWork.KyBaoCaoRepo.AddAsync(newBaoCaoKi);
                await _unitOfWork.SaveChangesAsync();

                await CheckViPhamAsync(fromDate, toDate, newBaoCaoKi.maky);

                await _unitOfWork.CommitTransactionAsync();

                var result = new BaoCaokiCreateViewModel
                {
                    //maky = newBaoCaoKi.maky,
                    tenky = generatedTenKy,
                    tungay = newBaoCaoKi.tungay.HasValue ? newBaoCaoKi.tungay.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                    denngay = newBaoCaoKi.denngay.HasValue ? newBaoCaoKi.denngay.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                    ngaydong_giaitrinh = newBaoCaoKi.ngaydong_giaitrinh.HasValue ? newBaoCaoKi.ngaydong_giaitrinh.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                    ngay_duyet = newBaoCaoKi.ngay_duyet.HasValue ? newBaoCaoKi.ngay_duyet.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                    ngaydong_auto = newBaoCaoKi.ngaydong_auto.HasValue ? newBaoCaoKi.ngaydong_auto.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                };


                return new List<BaoCaokiCreateViewModel> { result };
            }
            catch (Exception ex) 
            {
               // await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }


        public async System.Threading.Tasks.Task CheckViPhamAsync1(DateTime fromDate, DateTime toDate, int maky)
        {
            try
            {


                var dataInMonth = await _unitOfWork.ChamCong_CheckInOut_v1Repo
                    .GetByConditionAsync(x => x.NgayCham >= fromDate && x.NgayCham <= toDate);

                var fromDateOnly = DateOnly.FromDateTime(fromDate);
                var toDateOnly = DateOnly.FromDateTime(toDate);

                var nghiPhepList = await _unitOfWork.NghiPhepRepo
                    .GetByConditionAsync(x => x.NGAY >= fromDateOnly && x.NGAY <= toDateOnly);

                //// ✅ Thêm dòng này để lấy danh sách lịch làm việc trong kỳ
                //var lichLamViecList = await _unitOfWork.LichLamViecRepo
                //    .GetByConditionAsync(x => x.Ngay >= fromDateOnly && x.Ngay <= toDateOnly);

                //// ✅ Tạo dictionary để tra nhanh lịch làm việc theo ngày
                //var lichLamViecDict = lichLamViecList.ToDictionary(x => x.Ngay, x => x);

                var groupData = dataInMonth
                    .GroupBy(x => new { x.MaNhanVien, Ngay = x.NgayCham.Value.Date });

                _logger.LogInformation($"Số bản ghi chấm công trong kỳ: {dataInMonth.Count()}");
                _logger.LogInformation($"Tổng số group NV/ngày: {groupData.Count()}");

                var maNhanVienList = groupData.Select(g => g.Key.MaNhanVien).Distinct().ToList();
                var accountList = await _unitOfWork.AccountRepo
                    .GetByConditionAsync(c => maNhanVienList.Contains(c.MaNhanVien));
                var accountDict = accountList.ToDictionary(x => x.MaNhanVien, x => x);

                foreach (var group in groupData)
                {
                    var maNV = group.Key.MaNhanVien;
                    // var inForNV = await _unitOfWork.AccountRepo.FirstOrDefaultAsync(c=>c.MaNhanVien == maNV);
                    accountDict.TryGetValue(maNV, out var inForNV);
                    var ngayCham = group.Key.Ngay;

                    //var ngayDateOnly = DateOnly.FromDateTime(ngayCham);

                    //// ✅ Kiểm tra nếu ngày này không làm việc và không phải ngày làm bù thì bỏ qua
                    //if (lichLamViecDict.TryGetValue(ngayDateOnly, out var lich))
                    //{
                    //    if (!lich.LaNgayLamViec && lich.LoaiNgay != "Làm bù")
                    //    {
                    //        _logger.LogInformation($"Bỏ qua ngày {ngayCham:yyyy-MM-dd} (Không phải ngày làm việc)");
                    //        continue;
                    //    }
                    //}

                    var checkin = group.Min(x => x.GioCham);
                    var checkout = group.Max(x => x.GioCham);

                    var checkinTime = checkin.HasValue ? checkin.Value.TimeOfDay : TimeSpan.Zero;
                    var checkoutTime = checkout.HasValue ? checkout.Value.TimeOfDay : TimeSpan.Zero;

                    bool hasCheckIn = group.Any(x => x.GioCham.HasValue && x.GioCham.Value.TimeOfDay <= new TimeSpan(12, 0, 0));
                    bool hasCheckOut = group.Any(x => x.GioCham.HasValue && x.GioCham.Value.TimeOfDay >= new TimeSpan(12, 0, 0));

                    var phep = nghiPhepList.FirstOrDefault(x =>
                        x.MANS == maNV &&
                        x.NGAY == DateOnly.FromDateTime(ngayCham)
                    );

                    string sangPhep = phep?.SANG ?? ".";
                    string chieuPhep = phep?.CHIEU ?? ".";

                    int loaiViPham = 0;

                    if (!hasCheckIn && !hasCheckOut)
                    {
                        loaiViPham = 10;
                    }
                    else if (!hasCheckIn || !hasCheckOut)
                    {
                        loaiViPham = 9;
                    }
                    else
                    {
                        bool muon = (checkinTime - new TimeSpan(8, 0, 0)).TotalMinutes >= 10 && sangPhep == ".";
                        bool som = (new TimeSpan(17, 30, 0) - checkoutTime).TotalMinutes >= 10 && chieuPhep == ".";

                        if (muon && som)
                        {
                            loaiViPham = 7;
                        }
                        else if (muon)
                        {
                            loaiViPham = 1;
                        }
                        else if (som)
                        {
                            loaiViPham = 2;
                        }
                    }

                    string viPhamId = $"{maNV}{ngayCham:ddMMyyyy}";

                    try
                    {
                        var isExist = await _unitOfWork.VPCCTempRepo
                            .GetByConditionAsync(x => x.viphamid == viPhamId);

                        if (!isExist.Any())
                        {
                            _logger.LogInformation($"Insert vpcc_temp: maNV = {maNV}, ngay = {ngayCham:yyyy-MM-dd}, viphamid = {viPhamId}, loai = {loaiViPham}");

                            await _unitOfWork.VPCCTempRepo.AddAsync(new tb_vpcc_temp
                            {
                                maky = maky,
                                viphamid = viPhamId,
                                mans = maNV.ToString(),
                                ngay = DateOnly.FromDateTime(ngayCham),
                                loaiviphamid = loaiViPham,
                                time_in = checkinTime.ToString(),
                                time_out = checkoutTime.ToString(),
                                madv = maNV,
                                hoten = inForNV?.FullName ?? "",
                                chucdanh = inForNV?.ChucVu ?? ""
                            });
                        }
                        else
                        {
                            _logger.LogWarning($"Đã tồn tại viphamid = {viPhamId}, bỏ qua insert.");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Lỗi khi insert vpcc_temp: maNV = {maNV}, viphamid = {viPhamId}, ngày = {ngayCham:yyyy-MM-dd}");
                        throw;
                    }
                }

                try
                {
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi SaveChangesAsync sau khi insert các vi phạm");
                    throw;
                }
            }
            catch (Exception ex){
                _logger.LogError(ex, "Lỗi");
                throw;
            }
        }

        public async System.Threading.Tasks.Task CheckViPhamAsync(DateTime fromDate, DateTime toDate, int maky)
        {
            try
            {
                var dataInMonth = await _unitOfWork.ChamCong_CheckInOut_v1Repo
                    .GetByConditionAsync(x => x.NgayCham >= fromDate && x.NgayCham <= toDate);

                var fromDateOnly = DateOnly.FromDateTime(fromDate);
                var toDateOnly = DateOnly.FromDateTime(toDate);

                var nghiPhepList = await _unitOfWork.NghiPhepRepo
                    .GetByConditionAsync(x => x.NGAY >= fromDateOnly && x.NGAY <= toDateOnly);

                var groupData = dataInMonth
                    .GroupBy(x => new { x.MaNhanVien, Ngay = x.NgayCham.Value.Date });

                _logger.LogInformation($"Số bản ghi chấm công trong kỳ: {dataInMonth.Count()}");
                _logger.LogInformation($"Tổng số group NV/ngày: {groupData.Count()}");

                var maNhanVienList = groupData.Select(g => g.Key.MaNhanVien).Distinct().ToList();
                var accountList = await _unitOfWork.AccountRepo
                    .GetByConditionAsync(c => maNhanVienList.Contains(c.MaNhanVien));
                var accountDict = accountList.ToDictionary(x => x.MaNhanVien, x => x);

                // Tạo danh sách tất cả các ngày cần kiểm tra
                var allDates = Enumerable.Range(0, (toDate - fromDate).Days + 1)
                    .Select(offset => fromDate.AddDays(offset))
                    .ToList();

                // Lấy toàn bộ nhân viên cần kiểm tra
                var allNhanVien = await _unitOfWork.AccountRepo.GetAllAsync();

                foreach (var ngay in allDates)
                {
                    // Skip nếu là ngày nghỉ
                    if (!await IsWorkingDayAsync(ngay))
                    {
                        _logger.LogInformation($"Bỏ qua {ngay:yyyy-MM-dd} vì không phải ngày làm việc.");
                        continue;
                    }

                    foreach (var nv in allNhanVien)
                    {
                        _logger.LogInformation($"--- Kiểm tra NV: {nv.MaNhanVien} ({nv.FullName}), ngày: {ngay:yyyy-MM-dd} ---");
                        var group = groupData.FirstOrDefault(g =>
                            g.Key.MaNhanVien == nv.MaNhanVien &&
                            g.Key.Ngay.Date == ngay.Date);

                        TimeSpan checkinTime = TimeSpan.Zero;
                        TimeSpan checkoutTime = TimeSpan.Zero;
                        bool hasCheckIn = false;
                        bool hasCheckOut = false;

                        if (group != null)
                        {
                            checkinTime = group.Min(x => x.GioCham)?.TimeOfDay ?? TimeSpan.Zero;
                            checkoutTime = group.Max(x => x.GioCham)?.TimeOfDay ?? TimeSpan.Zero;

                            hasCheckIn = group.Any(x => x.GioCham.HasValue && x.GioCham.Value.TimeOfDay <= new TimeSpan(12, 0, 0));
                            hasCheckOut = group.Any(x => x.GioCham.HasValue && x.GioCham.Value.TimeOfDay >= new TimeSpan(12, 0, 0));
                        }

                        var phep = nghiPhepList.FirstOrDefault(x =>
                            x.MANS == nv.MaNhanVien &&
                            x.NGAY == DateOnly.FromDateTime(ngay));

                        string sangPhep = phep?.SANG ?? ".";
                        string chieuPhep = phep?.CHIEU ?? ".";

                        int loaiViPham = 0;

                        if (!hasCheckIn && !hasCheckOut)
                        {
                            if (sangPhep == "." || chieuPhep == ".")
                                loaiViPham = 10;
                        }
                        else if (!hasCheckIn || !hasCheckOut)
                        {
                            loaiViPham = 9;
                        }
                        else
                        {
                            bool muon = (checkinTime - new TimeSpan(8, 0, 0)).TotalMinutes >= 10 && sangPhep == ".";
                            bool som = (new TimeSpan(17, 30, 0) - checkoutTime).TotalMinutes >= 10 && chieuPhep == ".";

                            if (muon && som)
                            {
                                loaiViPham = 7;
                            }
                            else if (muon)
                            {
                                loaiViPham = 1;
                            }
                            else if (som)
                            {
                                loaiViPham = 2;
                            }
                        }

                        string viPhamId = $"{nv.MaNhanVien}{ngay:ddMMyyyy}";

                        try
                        {
                            var isExist = await _unitOfWork.VPCCTempRepo
                                .GetByConditionAsync(x => x.viphamid == viPhamId);

                            if (!isExist.Any())
                            {
                                _logger.LogInformation($"Insert vpcc_temp: maNV = {nv.MaNhanVien}, ngay = {ngay:yyyy-MM-dd}, viphamid = {viPhamId}, loai = {loaiViPham}");

                                await _unitOfWork.VPCCTempRepo.AddAsync(new tb_vpcc_temp
                                {
                                    maky = maky,
                                    viphamid = viPhamId,
                                    mans = nv.MaNhanVien.ToString(),
                                    ngay = DateOnly.FromDateTime(ngay),
                                    loaiviphamid = loaiViPham,
                                    time_in = checkinTime.ToString(),
                                    time_out = checkoutTime.ToString(),
                                    madv = nv.MaNhanVien,
                                    hoten = nv.FullName ?? "",
                                    chucdanh = nv.ChucVu ?? ""
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Lỗi khi insert vpcc_temp: maNV = {nv.MaNhanVien}, viphamid = {viPhamId}, ngày = {ngay:yyyy-MM-dd}");
                            throw;
                        }
                    }
                }

                try
                {
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi SaveChangesAsync sau khi insert các vi phạm");
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi tổng trong CheckViPhamAsync1");
                throw;
            }
        }




        public async Task<bool> IsWorkingDayAsync(DateTime ngay)
        {
            var dateOnly = DateOnly.FromDateTime(ngay);

            var lich = await _unitOfWork.LichLamViecRepo
                .FirstOrDefaultAsync(x => x.Ngay == dateOnly);

            if (lich != null)
            {
                return lich.LaNgayLamViec; // TRUE hoặc FALSE
            }

            // Nếu không có record trong bảng => Mặc định
            // T2–T6 là làm việc, T7–CN là nghỉ
            var dayOfWeek = ngay.DayOfWeek;
            return dayOfWeek >= DayOfWeek.Monday && dayOfWeek <= DayOfWeek.Friday;
        }

    }
}
