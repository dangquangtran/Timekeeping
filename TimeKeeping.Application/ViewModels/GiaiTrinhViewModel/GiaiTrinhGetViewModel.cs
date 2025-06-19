using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeKeeping.Application.ViewModels.GiaiTrinhViewModel
{
    public class GiaiTrinhGetViewModel
    {
        public string Thu { get; set; }
        public string Ngay { get; set;}
        public string GioVao { get; set;}
        public string GioRa { get; set;}
        public int? GiaiTrinh { get; set;}
        public int? File { get; set;}

        public string? viphamid { get; set; }

       // public int? giaitrinhid { get; set; }

        public int? maky { get; set; }
    }
}
