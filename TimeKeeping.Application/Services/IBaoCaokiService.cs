using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeKeeping.Application.ViewModels.BaoCaokiViewModel;

namespace TimeKeeping.Application.Services
{
    public interface IBaoCaokiService
    {
        Task<(IEnumerable<BaoCaokiGetViewModel> List, int TotalCount)> GetAllAsync(int pageIndex, int pageSize);
        Task<IEnumerable<BaoCaokiCreateViewModel>> CreateBaoCaoKiAsync(BaoCaokiCreateViewModel baoCaokiCreateViewModel);
    }
}
