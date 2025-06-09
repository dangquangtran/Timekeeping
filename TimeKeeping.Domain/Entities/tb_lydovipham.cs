using System;
using System.Collections.Generic;

namespace TimeKeeping.Domain.Entities;

public partial class tb_lydovipham
{
    public int lydoid { get; set; }

    public string tenlydo { get; set; } = null!;

    public int ghinhan { get; set; }

    public string? ghichu { get; set; }

    public int kemfile { get; set; }
}
