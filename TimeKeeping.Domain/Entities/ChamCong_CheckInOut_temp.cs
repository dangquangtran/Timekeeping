using System;
using System.Collections.Generic;

namespace TimeKeeping.Domain.Entities;

public partial class ChamCong_CheckInOut_temp
{
    public int ID { get; set; }

    public string? manhanvien { get; set; }

    public DateTime? NgayCham { get; set; }

    public DateTime? GioCham { get; set; }

    public int? KieuCham { get; set; }

    public int? NguonCham { get; set; }

    public int? MaSoMay { get; set; }

    public string? TenMay { get; set; }
}
