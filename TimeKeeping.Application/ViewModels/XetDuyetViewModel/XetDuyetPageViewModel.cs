using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeKeeping.Application.ViewModels.XetDuyetViewModel
{
    public class XetDuyetPageViewModel
    {
        public IEnumerable<XetDuyetGetViewModel> List { get; set; } = new List<XetDuyetGetViewModel>();
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; } = 0;
        public int TotalRecords { get; set; } = 0;
        public string? SelectedDonVi { get; set; }
        public string? SelectedKy { get; set; }
        public string? SelectedStatus { get; set; }
    }
}