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
                var existingTenKi = await _unitOfWork.KyBaoCaoRepo.FirstOrDefaultAsync(a => a.tenky == baoCaokiCreateViewModel.tenky);

                if (existingTenKi != null)
                {
                    _logger.LogWarning("Tên Kì đã tồn tại: " + baoCaokiCreateViewModel.tenky);
                    throw new Exception("Tên Kì đã tồn tại.");
                }
                var newBaoCaoKi = new tb_kybaocao
                {
                    // maky = baoCaokiCreateViewModel.maky,
                    tenky = baoCaokiCreateViewModel.tenky,
                    tungay = baoCaokiCreateViewModel.tungay.HasValue ? DateOnly.FromDateTime(baoCaokiCreateViewModel.tungay.Value) : (DateOnly?)null,
                    denngay = baoCaokiCreateViewModel.denngay.HasValue ? DateOnly.FromDateTime(baoCaokiCreateViewModel.denngay.Value) : (DateOnly?)null,
                    ngaydong_giaitrinh = baoCaokiCreateViewModel.ngaydong_giaitrinh.HasValue ? DateOnly.FromDateTime(baoCaokiCreateViewModel.ngaydong_giaitrinh.Value) : (DateOnly?)null,
                    ngay_duyet = baoCaokiCreateViewModel.ngay_duyet.HasValue ? DateOnly.FromDateTime(baoCaokiCreateViewModel.ngay_duyet.Value) : (DateOnly?)null,
                    ngaydong_auto = baoCaokiCreateViewModel.ngaydong_auto.HasValue ? DateOnly.FromDateTime(baoCaokiCreateViewModel.ngaydong_auto.Value) : (DateOnly?)null,
                };

                await _unitOfWork.KyBaoCaoRepo.AddAsync(newBaoCaoKi);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                var result = new BaoCaokiCreateViewModel
                {
                    //maky = newBaoCaoKi.maky,
                    tenky = newBaoCaoKi.tenky,
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
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
