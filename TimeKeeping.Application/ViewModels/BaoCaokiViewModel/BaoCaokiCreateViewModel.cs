using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeKeeping.Application.ViewModels.BaoCaokiViewModel
{
    public class BaoCaokiCreateViewModel
    {
        // public int maky { get; set; }

        public string? tenky { get; set; }
        public DateTime? tungay { get; set; }
        public DateTime? denngay { get; set; }
        public DateTime? ngaydong_auto { get; set; }
        public DateTime? ngaydong_giaitrinh { get; set; }
        public DateTime? ngay_duyet { get; set; }
    }
}
