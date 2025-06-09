using System;
using System.Collections.Generic;

namespace TimeKeeping.Domain.Entities;

public partial class tb_ChucNang
{
    public int ChucNangID { get; set; }

    public string? TenChucNang { get; set; }

    public virtual ICollection<tb_GroupFunction> GroupFuncs { get; set; } = new List<tb_GroupFunction>();
    public virtual ICollection<tb_GroupFunctionDetail> tb_GroupFunctionDetails { get; set; } = new List<tb_GroupFunctionDetail>();

}
