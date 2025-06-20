using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeKeeping.Application.ViewModels.PermissionVIewModel
{
    public class PermissionUpdateViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn mã nhân viên")]
        public string? MaNhanVien { get; set; }
        
        [Required(ErrorMessage = "Vui lòng chọn nhóm người dùng")]
        public string? ThuocNhomND { get; set; }
    }
}
