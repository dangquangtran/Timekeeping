using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeKeeping.Application.ViewModels.ChamCongCheckInOutV1ViewModel
{
    public class ChamCongExportRequestViewModel
    {
        public int Thang { get; set; }
        public int Nam { get; set; }
        public string TenDonVi { get; set; } = string.Empty;
    }
}
