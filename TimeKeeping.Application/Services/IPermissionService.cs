using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeKeeping.Application.ViewModels.AccountViewModel;
using TimeKeeping.Application.ViewModels.PermissionVIewModel;

namespace TimeKeeping.Application.Services
{
    public interface IPermissionService
    {
        Task<IEnumerable<AccountGetViewModel>> GetAllAsync(int pageIndex, int pageSize);
        Task<IEnumerable<PermissionGetViewModel>> GetAllPermissionAsync();
        Task<IEnumerable<PermissionUpdateViewModel>> UpdatePermissionAsync(PermissionUpdateViewModel permissionUpdateViewModel);
    }
}
