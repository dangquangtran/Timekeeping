using System;

namespace TimeKeeping.Application.ViewModels.XetDuyetViewModel
{
    public class XetDuyetGiaiTrinhFilter
    {
        public string? Status { get; set; } // "Ch?a Xe?t Duyê?t" ho?c "?a? Xe?t Duyê?t"
        public string? DonVi { get; set; }  // Tên ??n v?
        public string? Ky { get; set; }     // VD: "Tháng 5/2025"
    }
}
