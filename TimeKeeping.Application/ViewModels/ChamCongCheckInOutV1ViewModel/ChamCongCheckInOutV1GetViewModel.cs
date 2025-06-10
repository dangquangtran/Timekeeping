using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeKeeping.Application.ViewModels.ChamCongCheckInOutV1ViewModel
{
    public class ChamCongCheckInOutV1GetViewModel
    {
        public string? MaChamCong { get; set; }

        public string? HoTen { get; set; }
        public string? ChucDanh { get; set; }
        public string? DonVi { get; set; }

        public string? NgayCham { get; set; }

        public string? GioCham { get; set; }

        //public int? KieuCham { get; set; }

        //public int? NguonCham { get; set; }

        //public int? MaSoMay { get; set; }

        public string? TenMay { get; set; }
    }
}
