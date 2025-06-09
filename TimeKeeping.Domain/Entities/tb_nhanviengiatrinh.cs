using System;
using System.Collections.Generic;

namespace TimeKeeping.Domain.Entities;

public partial class tb_nhanviengiatrinh
{
    public string EmpID { get; set; } = null!;

    public string EmpCode { get; set; } = null!;

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public string? Level1ID { get; set; }

    public string? Level1Name { get; set; }

    public string? Level2ID { get; set; }

    public string? Level2Name { get; set; }

    public string? Level3ID { get; set; }

    public string? Level3Name { get; set; }

    public bool? IsManager { get; set; }

    public string? LSJobGroupID { get; set; }

    public string? PositionID { get; set; }

    public string? Title { get; set; }

    public string? WorkingDate { get; set; }

    public string? DirectEmpID { get; set; }

    public string? DirectEmpManager { get; set; }

    public string? ThuongTru { get; set; }

    public string? TamTru { get; set; }

    public string? Birthday { get; set; }

    public string? Gender { get; set; }

    public string? PIN { get; set; }

    public string? PINDate { get; set; }

    public string? PINPlace { get; set; }

    public string? FilePhoto { get; set; }

    public string? Mobifone { get; set; }

    public string? LSPosGroupID { get; set; }

    public string? LastName { get; set; }

    public string? FirstName { get; set; }
}
