using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeKeeping.Application.ViewModels.ChamCongCheckInOutV1ViewModel;
using TimeKeeping.Application.ViewModels.XetDuyetViewModel;

namespace TimeKeeping.Application.Services
{
    public interface IXetDuyetGiaiTrinhService
    {
        Task<(IEnumerable<XetDuyetGetViewModel> List, int TotalPages)> GetListGiaiTrinhAsync(int pageIndex, int pageSize, XetDuyetGiaiTrinhFilter filter);
        Task<int> ApproveAllChuaXetDuyetAsync();
        Task<int> ApproveSelectedAsync(IEnumerable<(string viphamid, int? maky)> ids);
        Task<IEnumerable<ListKyGetViewModel>> GetAllKyAsync();
        Task<IEnumerable<ListDonViGetViewModel>> GetAllDonViAsync();
    }
}
