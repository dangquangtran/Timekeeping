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
    }

}
