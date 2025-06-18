using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeKeeping.Application.ViewModels.ChamCongCheckInOutV1ViewModel;
using TimeKeeping.Application.ViewModels.GiaiTrinhViewModel;

namespace TimeKeeping.Application.Services
{
    public interface IGiaiTrinhService
    {
        Task<(IEnumerable<GiaiTrinhBaoCaoGetViewModel> List, int TotalPages)> GetListChamCongAsync(int pageIndex, int pageSize, string userName);
        Task<(IEnumerable<GiaiTrinhGetViewModel> List, int TotalPages)> GetListGiaiTrinhAsync(int pageIndex, int pageSize, string userName);
    }
}
