using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeKeeping.Domain.Entities
{
    public partial class tb_Account
    {
        public int ID { get; set; }
        public string MaNhanVien { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; } = null!;
        public string HashPassword { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime? LastLogin { get; set; }
        public int GroupFuncID { get; set; }

        public virtual tb_GroupFunction GroupFunction { get; set; } = null!;
    }
}
