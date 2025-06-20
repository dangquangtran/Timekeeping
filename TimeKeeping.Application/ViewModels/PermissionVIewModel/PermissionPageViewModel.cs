using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeKeeping.Application.ViewModels.AccountViewModel;

namespace TimeKeeping.Application.ViewModels.PermissionVIewModel
{
    public class PermissionPageViewModel
    {
        [BindNever]
        public IEnumerable<AccountGetViewModel> Accounts { get; set; }
        [BindNever]
        public IEnumerable<AccountGetViewModel> AllAccounts { get; set; }
        [BindNever]
        public IEnumerable<PermissionGetViewModel> Permissions { get; set; }

        public PermissionUpdateViewModel PermissionUpdate { get; set; } = new PermissionUpdateViewModel();
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
