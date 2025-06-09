using System;
using System.Collections.Generic;

namespace TimeKeeping.Domain.Entities;

public partial class tb_vpcc_temp
{
    public string? viphamid { get; set; }

    public int? maky { get; set; }

    public DateOnly? ngay { get; set; }

    public string? mans { get; set; }

    public string? time_in { get; set; }

    public string? time_out { get; set; }

    public int? loaiviphamid { get; set; }

    public string? madv { get; set; }

    public string? hoten { get; set; }

    public string? chucdanh { get; set; }
}
