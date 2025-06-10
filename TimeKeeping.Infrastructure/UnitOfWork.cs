using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeKeeping.Application.Interfaces;
using TimeKeeping.Domain.Entities;
using TimeKeeping.Infrastructure.Persistence;
using TimeKeeping.Infrastructure.Repositories;

namespace TimeKeeping.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction _transaction;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        private IGenericRepository<ChamCong_CheckInOut_temp> _chamCongCheckInOutTempRepo;
        public IGenericRepository<ChamCong_CheckInOut_temp> ChamCong_CheckInOut_tempRepo => _chamCongCheckInOutTempRepo ??= new GenericRepository<ChamCong_CheckInOut_temp>(_context);

        private IGenericRepository<ChamCong_CheckInOut_v1> _chamCongCheckInOutV1Repo;
        public IGenericRepository<ChamCong_CheckInOut_v1> ChamCong_CheckInOut_v1Repo => _chamCongCheckInOutV1Repo ??= new GenericRepository<ChamCong_CheckInOut_v1>(_context);

        private IGenericRepository<DS_HN_temp> _dsHNTempRepo;
        public IGenericRepository<DS_HN_temp> DS_HN_tempRepo => _dsHNTempRepo ??= new GenericRepository<DS_HN_temp>(_context);

        private IGenericRepository<tb_ChucNang> _chucNangRepo;
        public IGenericRepository<tb_ChucNang> ChucNangRepo => _chucNangRepo ??= new GenericRepository<tb_ChucNang>(_context);

        private IGenericRepository<tb_donvi> _donViRepo;
        public IGenericRepository<tb_donvi> DonViRepo => _donViRepo ??= new GenericRepository<tb_donvi>(_context);

        private IGenericRepository<tb_giaitrinh_chamcong> _giaiTrinhChamCongRepo;
        public IGenericRepository<tb_giaitrinh_chamcong> GiaiTrinhChamCongRepo => _giaiTrinhChamCongRepo ??= new GenericRepository<tb_giaitrinh_chamcong>(_context);

        private IGenericRepository<tb_GroupFunction> _groupFunctionRepo;
        public IGenericRepository<tb_GroupFunction> GroupFunctionRepo => _groupFunctionRepo ??= new GenericRepository<tb_GroupFunction>(_context);

        private IGenericRepository<tb_GroupFunctionDetail> _groupFunctionDetailRepo;
        public IGenericRepository<tb_GroupFunctionDetail> GroupFunctionDetailRepo => _groupFunctionDetailRepo ??= new GenericRepository<tb_GroupFunctionDetail>(_context);

        private IGenericRepository<tb_kybaocao> _kyBaoCaoRepo;
        public IGenericRepository<tb_kybaocao> KyBaoCaoRepo => _kyBaoCaoRepo ??= new GenericRepository<tb_kybaocao>(_context);

        private IGenericRepository<tb_lydovipham> _lyDoViPhamRepo;
        public IGenericRepository<tb_lydovipham> LyDoViPhamRepo => _lyDoViPhamRepo ??= new GenericRepository<tb_lydovipham>(_context);

        private IGenericRepository<TB_NGHIPHEP> _nghiPhepRepo;
        public IGenericRepository<TB_NGHIPHEP> NghiPhepRepo => _nghiPhepRepo ??= new GenericRepository<TB_NGHIPHEP>(_context);

        private IGenericRepository<tb_nhanviengiatrinh> _nhanVienGiaiTrinhRepo;
        public IGenericRepository<tb_nhanviengiatrinh> NhanVienGiaiTrinhRepo => _nhanVienGiaiTrinhRepo ??= new GenericRepository<tb_nhanviengiatrinh>(_context);

        private IGenericRepository<tb_trangthaiky> _trangThaiKyRepo;
        public IGenericRepository<tb_trangthaiky> TrangThaiKyRepo => _trangThaiKyRepo ??= new GenericRepository<tb_trangthaiky>(_context);

        private IGenericRepository<tb_viphamchamcongvantay> _viPhamChamCongVanTayRepo;
        public IGenericRepository<tb_viphamchamcongvantay> ViPhamChamCongVanTayRepo => _viPhamChamCongVanTayRepo ??= new GenericRepository<tb_viphamchamcongvantay>(_context);

        private IGenericRepository<tb_vpcc_temp> _vpccTempRepo;
        public IGenericRepository<tb_vpcc_temp> VPCCTempRepo => _vpccTempRepo ??= new GenericRepository<tb_vpcc_temp>(_context);

        private IGenericRepository<tb_Account> accountRepo;
        public IGenericRepository<tb_Account> AccountRepo => accountRepo ??= new GenericRepository<tb_Account>(_context);

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        public async Task BeginTransactionAsync()
        {
            if (_transaction == null)
                _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }

}
