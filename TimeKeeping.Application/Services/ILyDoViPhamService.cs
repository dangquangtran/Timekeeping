using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeKeeping.Application.ViewModels.LyDoViPhamViewModel;

namespace TimeKeeping.Application.Services
{
    public interface ILyDoViPhamService
    {
        Task<IEnumerable<LyDoViPhamGetViewModel>> GetAllAsync(int pageIndex, int pageSize);
    }
}
