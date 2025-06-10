using System;
using System.Collections.Generic;

namespace TimeKeeping.Domain.Entities;

public partial class tb_GroupFunction
{
    public int GroupFuncID { get; set; }

    public string GroupName { get; set; } = null!;

    public string? GroupNote { get; set; }

    public virtual ICollection<tb_ChucNang> Funcs { get; set; } = new List<tb_ChucNang>();
    public virtual ICollection<tb_GroupFunctionDetail> tb_GroupFunctionDetails { get; set; } = new List<tb_GroupFunctionDetail>();
    public virtual ICollection<tb_Account> Accounts { get; set; } = new List<tb_Account>();

}
