using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeKeeping.Application.ViewModels.LyDoViPhamViewModel
{
    public class LyDoViPhamGetViewModel
    {
        public int lydoid { get; set; }

        public string tenlydo { get; set; } = null!;

        public int ghinhan { get; set; }

        public string? ghichu { get; set; }

        public int kemfile { get; set; }
    }
}
