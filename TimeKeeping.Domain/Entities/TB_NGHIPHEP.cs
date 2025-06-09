using System;
using System.Collections.Generic;

namespace TimeKeeping.Domain.Entities;

public partial class TB_NGHIPHEP
{
    public string MANS { get; set; } = null!;

    public DateOnly NGAY { get; set; }

    public string SANG { get; set; } = null!;

    public string CHIEU { get; set; } = null!;
}
