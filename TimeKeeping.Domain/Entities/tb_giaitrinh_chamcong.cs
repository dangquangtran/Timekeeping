using System;
using System.Collections.Generic;

namespace TimeKeeping.Domain.Entities;

public partial class tb_giaitrinh_chamcong
{
    public string? viphamid { get; set; }

    public int? giaitrinhid { get; set; }

    public int? maky { get; set; }

    public virtual tb_lydovipham? giaitrinh { get; set; }

    public virtual tb_viphamchamcongvantay? vipham { get; set; }
}
