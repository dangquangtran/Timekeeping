using System;
using System.Collections.Generic;

namespace TimeKeeping.Domain.Entities;

public partial class ChamCong_CheckInOut_v1
{
    public string? ID { get; set; }

    public string? MaNhanVien { get; set; }

    public DateTime? NgayCham { get; set; }

    public DateTime? GioCham { get; set; }

    public int? KieuCham { get; set; }

    public int? NguonCham { get; set; }

    public int? MaSoMay { get; set; }

    public string? TenMay { get; set; }
}
