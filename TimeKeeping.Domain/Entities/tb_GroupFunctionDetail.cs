using System;
using System.Collections.Generic;

namespace TimeKeeping.Domain.Entities;

public partial class tb_GroupFunctionDetail
{
    public int GroupFuncID { get; set; }

    public int FuncID { get; set; }
    public virtual tb_GroupFunction GroupFunc { get; set; } = null!;
    public virtual tb_ChucNang Func { get; set; } = null!;
}
