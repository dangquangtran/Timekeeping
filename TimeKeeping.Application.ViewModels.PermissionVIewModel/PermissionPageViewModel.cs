using Microsoft.AspNetCore.Mvc.ModelBinding;

public class PermissionPageViewModel
{
    [BindNever]
    public IEnumerable<AccountGetViewModel> Accounts { get; set; }
    [BindNever]
    public IEnumerable<AccountGetViewModel> AllAccounts { get; set; }
    [BindNever]
    public IEnumerable<PermissionGetViewModel> Permissions { get; set; }

    public PermissionUpdateViewModel PermissionUpdate { get; set; } = new();
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}