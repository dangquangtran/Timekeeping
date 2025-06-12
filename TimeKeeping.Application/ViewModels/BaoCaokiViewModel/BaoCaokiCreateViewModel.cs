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

        public DateOnly? tungay { get; set; }

        public DateOnly? denngay { get; set; }

        public DateOnly? ngaydong_auto { get; set; }

        public DateOnly? ngaydong_giaitrinh { get; set; }

        public DateOnly? ngay_duyet { get; set; }
    }
}
