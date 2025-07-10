using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeKeeping.Application.ViewModels.XetDuyetViewModel
{
    public class XetDuyetGetViewModel
    {
        public string? MaNhanVien { get; set; }
        public string? HoTen { get; set; }
        public string Thu { get; set; }
        public string Ngay { get; set; }
        public string GioVao { get; set; }
        public string GioRa { get; set; }
        public int? GiaiTrinh { get; set; }
        public int? File { get; set; }

        public string? viphamid { get; set; }

        public string? Status { get; set; } // Đổi sang string

        public int? maky { get; set; }
    }
}
