using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeKeeping.Application.ViewModels.BaoCaokiViewModel
{
    public class KyBaoCaoPageViewModel
    {
        public BaoCaokiCreateViewModel CreateModel { get; set; } = new BaoCaokiCreateViewModel();
        public IEnumerable<BaoCaokiGetViewModel> List { get; set; } = new List<BaoCaokiGetViewModel>();
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; } = 1;
    }

}
