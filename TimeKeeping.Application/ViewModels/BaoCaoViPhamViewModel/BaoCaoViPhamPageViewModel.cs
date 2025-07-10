using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeKeeping.Application.ViewModels.BaoCaoViPhamViewModel
{
    public class BaoCaoViPhamPageViewModel
    {
        public IEnumerable<BaoCaoViPhamChamCongVanGetViewModel> List { get; set; } = new List<BaoCaoViPhamChamCongVanGetViewModel>();
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; } = 0;
        public int TotalRecords { get; set; } = 0;
        public string? SelectedDonVi { get; set; }
        public string? SelectedKy { get; set; }
    }
}
