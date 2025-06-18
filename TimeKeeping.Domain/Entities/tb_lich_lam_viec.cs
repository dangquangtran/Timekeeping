using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeKeeping.Domain.Entities
{
    public class tb_lich_lam_viec
    {
        public int Id { get; set; }
        public DateOnly Ngay { get; set; } 
        public bool LaNgayLamViec { get; set; }
        public string? LoaiNgay { get; set; } 
    }
}
