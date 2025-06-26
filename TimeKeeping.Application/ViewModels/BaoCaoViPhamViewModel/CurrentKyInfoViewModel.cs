using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeKeeping.Application.ViewModels.BaoCaoViPhamViewModel
{
    public class CurrentKyInfoViewModel
    {
        public string? TenKy { get; set; }
        public DateOnly? TuNgay { get; set; }
        public DateOnly? DenNgay { get; set; }
    }
}
