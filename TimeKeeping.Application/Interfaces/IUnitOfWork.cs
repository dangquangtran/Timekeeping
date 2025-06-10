using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeKeeping.Domain.Entities;

namespace TimeKeeping.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<ChamCong_CheckInOut_temp> ChamCong_CheckInOut_tempRepo { get; }
        IGenericRepository<ChamCong_CheckInOut_v1> ChamCong_CheckInOut_v1Repo { get; }
        IGenericRepository<DS_HN_temp> DS_HN_tempRepo { get; }
        IGenericRepository<tb_ChucNang> ChucNangRepo { get; }
        IGenericRepository<tb_donvi> DonViRepo { get; }
        IGenericRepository<tb_giaitrinh_chamcong> GiaiTrinhChamCongRepo { get; }
        IGenericRepository<tb_GroupFunction> GroupFunctionRepo { get; }
        IGenericRepository<tb_GroupFunctionDetail> GroupFunctionDetailRepo { get; }
        IGenericRepository<tb_kybaocao> KyBaoCaoRepo { get; }
        IGenericRepository<tb_lydovipham> LyDoViPhamRepo { get; }
        IGenericRepository<TB_NGHIPHEP> NghiPhepRepo { get; }
        IGenericRepository<tb_nhanviengiatrinh> NhanVienGiaiTrinhRepo { get; }
        IGenericRepository<tb_trangthaiky> TrangThaiKyRepo { get; }
        IGenericRepository<tb_viphamchamcongvantay> ViPhamChamCongVanTayRepo { get; }
        IGenericRepository<tb_vpcc_temp> VPCCTempRepo { get; }
        IGenericRepository<tb_Account> AccountRepo { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }

}
