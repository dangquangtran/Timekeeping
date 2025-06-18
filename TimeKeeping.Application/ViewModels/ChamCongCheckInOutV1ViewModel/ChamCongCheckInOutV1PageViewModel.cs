using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeKeeping.Application.ViewModels.ChamCongCheckInOutV1ViewModel
{
    public class ChamCongCheckInOutV1PageViewModel
    {
        public IEnumerable<ChamCongCheckInOutV1GetViewModel> List { get; set; } = new List<ChamCongCheckInOutV1GetViewModel>();
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int TotalPages { get; set; } = 1;
    }

}
