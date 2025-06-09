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
            _logger.LogInformation("Bắt đầu lấy danh sách lý do vi phạm");
            try
            {
                var (pagedData, totalCount) = await _unitOfWork.ChamCong_CheckInOut_v1Repo.GetPagedAsync(pageIndex, pageSize);

                var result = pagedData.Select(c => new ChamCongCheckInOutV1GetViewModel
                {
                    ID = c.ID,
                    MaNhanVien = c.MaNhanVien,
                    NgayCham = c.NgayCham,
                    GioCham = c.GioCham,
                    KieuCham = c.KieuCham,
                    NguonCham = c.NguonCham,
                    MaSoMay = c.MaSoMay,
                    TenMay = c.TenMay,
                }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Lỗi khi lấy danh sách lý do vi phạm");
                return Enumerable.Empty<ChamCongCheckInOutV1GetViewModel>();
            }
        }
    }
}
