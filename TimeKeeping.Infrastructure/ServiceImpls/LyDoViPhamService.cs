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
    public class LyDoViPhamService: ILyDoViPhamService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<LyDoViPhamService> _logger;

        public LyDoViPhamService(IUnitOfWork unitOfWork,
                            ILogger<LyDoViPhamService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<LyDoViPhamGetViewModel>> GetAllAsync(int pageIndex, int pageSize)
        {
            _logger.LogInformation("Bắt đầu lấy danh sách lý do vi phạm");
            try
            {
                var (pagedData, totalCount) = await _unitOfWork.LyDoViPhamRepo.GetPagedAsync(pageIndex, pageSize);

                var result = pagedData.Select(c=> new LyDoViPhamGetViewModel
                {
                    lydoid = c.lydoid,
                    tenlydo = c.tenlydo,
                    ghinhan = c.ghinhan,
                    ghichu = c.ghichu,
                    kemfile = c.kemfile,

                }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Lỗi khi lấy danh sách lý do vi phạm");
                return Enumerable.Empty<LyDoViPhamGetViewModel>();
            }
        }
    }
}
