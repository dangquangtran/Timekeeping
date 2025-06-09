using System;
using System.Collections.Generic;

namespace TimeKeeping.Domain.Entities;

public partial class tb_kybaocao
{
    public int maky { get; set; }

    public string? tenky { get; set; }

    public DateOnly? tungay { get; set; }

    public DateOnly? denngay { get; set; }

    public DateOnly? ngaydong_auto { get; set; }

    public DateOnly? ngaydong_giaitrinh { get; set; }

    public DateOnly? ngay_duyet { get; set; }
}
