using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeKeeping.Application.ViewModels.ChamCongCheckInOutV1ViewModel
{
    public class ChamCongCheckInOutV1GetViewModel
    {
        public string? ID { get; set; }

        public string? MaNhanVien { get; set; }

        public DateTime? NgayCham { get; set; }

        public DateTime? GioCham { get; set; }

        public int? KieuCham { get; set; }

        public int? NguonCham { get; set; }

        public int? MaSoMay { get; set; }

        public string? TenMay { get; set; }
    }
}
