using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeKeeping.Application.Interfaces;
using TimeKeeping.Application.Services;
using TimeKeeping.Application.ViewModels.AccountViewModel;
using TimeKeeping.Application.ViewModels.ChamCongCheckInOutV1ViewModel;
using TimeKeeping.Application.ViewModels.PermissionVIewModel;

namespace TimeKeeping.Infrastructure.ServiceImpls
{
    public class PermissionService : IPermissionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PermissionService> _logger;

        public PermissionService(IUnitOfWork unitOfWork,
                                 ILogger<PermissionService> logger)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<AccountGetViewModel>> GetAllAsync(int pageIndex, int pageSize)
        {
            _logger.LogInformation("Bắt đầu lấy danh sách account");
            try
            { 
                var (pagedData, totalCount) = await _unitOfWork.AccountRepo.GetPagedAsync(pageIndex, pageSize);

                var madonvi = pagedData.Select(c=>c.MaDv).Distinct().ToList();
                var donvi = await _unitOfWork.DonViRepo.GetByConditionAsync(a => madonvi.Contains(a.madv));

                var groupFuncID = pagedData.Select(c => c.GroupFuncID).Distinct().ToList();
                var groupName = await _unitOfWork.GroupFunctionRepo.GetByConditionAsync(a => groupFuncID.Contains(a.GroupFuncID));

                var donViDict = donvi.ToDictionary(a => a.madv);
                var groupNameDict = groupName.ToDictionary(a=> a.GroupFuncID);

                // Ánh xạ dữ liệu từ pagedData vào viewmodel
                var result = pagedData.Select(c =>
                {
                    var tenDonVi = donViDict.GetValueOrDefault(c.MaDv);
                    var role = groupNameDict.GetValueOrDefault(c.GroupFuncID);

                    return new AccountGetViewModel
                    {
                        MaNhanVien = c.MaNhanVien,
                        UserPortal = c.UserName,
                        HoTen = c.FullName,
                        DonVi = tenDonVi?.tendv,
                        ChucDanh = c.ChucVu,
                        ThuocNhomND = role?.GroupName,
                    };
                }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Lỗi khi lấy danh sách account");
                return Enumerable.Empty<AccountGetViewModel>();
            }
        }

        public async Task<IEnumerable<PermissionGetViewModel>> GetAllPermissionAsync()
        {
            _logger.LogInformation("Bắt đầu lấy danh sách quyền");
            try
            {
                var permission = await _unitOfWork.GroupFunctionRepo.GetAllAsync();

                var result = permission.Select(c => new PermissionGetViewModel
                {
                    Id = c.GroupFuncID,
                    ThuocNhomND = c.GroupName,
                        
                }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Lỗi khi lấy danh sách quyền");
                return Enumerable.Empty<PermissionGetViewModel>();
            }
        }

        public async Task<IEnumerable<PermissionUpdateViewModel>> UpdatePermissionAsync(PermissionUpdateViewModel permissionUpdateViewModel)
        {
            _logger.LogInformation("Bắt đầu update quyền");
            try
            {
                var maNhanVienExisting = await _unitOfWork.AccountRepo.FirstOrDefaultAsync(c => c.MaNhanVien == permissionUpdateViewModel.MaNhanVien);
                if (maNhanVienExisting == null)
                {
                    throw new Exception(" Không tồn tại mã nhân viên này");
                }

                var permission = await _unitOfWork.GroupFunctionRepo.FirstOrDefaultAsync(c=>c.GroupName == permissionUpdateViewModel.ThuocNhomND);
                if (permission == null)
                {
                    throw new Exception(" Không tồn tại quyền này");
                }
                await _unitOfWork.BeginTransactionAsync();

                maNhanVienExisting.GroupFuncID = permission.GroupFuncID;
                _unitOfWork.AccountRepo.Update(maNhanVienExisting);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return new[] { permissionUpdateViewModel };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Lỗi khi lấy danh sách quyền");
                return Enumerable.Empty<PermissionUpdateViewModel>();
            }
        }
    }
}
