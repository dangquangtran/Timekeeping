using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeKeeping.Application.ViewModels.ChamCongCheckInOutV1ViewModel;

namespace TimeKeeping.Application.Services
{
    public interface IChamCongCheckInOutV1Service
    {
        Task<IEnumerable<ChamCongCheckInOutV1GetViewModel>> GetAllAsync(int pageIndex, int pageSize);
        Task<byte[]> ExportChamCongToExcelAsync(ChamCongExportRequestViewModel chamCongExportRequestViewModel);
        Task<IEnumerable<ListKyGetViewModel>> GetAllKyAsync();
        Task<IEnumerable<ListDonViGetViewModel>> GetAllDonViAsync();
        Task<(IEnumerable<ChamCongCheckInOutV1GetViewModel> List, int TotalPages)> GetFilteredAsync(int pageIndex, int pageSize, FilterChamCongViewModel filter);
    }
}
