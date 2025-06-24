using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeKeeping.Application.ViewModels.BaoCaoViPhamViewModel;
using TimeKeeping.Application.ViewModels.GiaiTrinhViewModel;

namespace TimeKeeping.Application.Services
{
    public interface IBaoCaoViPhamService
    {
        Task<(IEnumerable<BaoCaoViPhamChamCongVanGetViewModel> List, int TotalPages)> GetListBaoCaoViPhamChamCongVanTayAsync(int pageIndex, int pageSize);
        Task<(IEnumerable<BaoCaoViPhamChamCongVanGetViewModel> List, int TotalPages)> GetListBaoCaoViPhamChamCongVanTayFilterAsync(int pageIndex, int pageSize, string? maDonVi = null, string? maKy = null);
        Task<byte[]> ExportBaoCaoViPhamChamCongVanTayToExcelAsync(BaoCaoViPhamExportRequestViewModel request);
    }
}
