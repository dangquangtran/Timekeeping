using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TimeKeeping.Domain.Entities;

namespace TimeKeeping.Infrastructure.Persistence;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChamCong_CheckInOut_temp> ChamCong_CheckInOut_temps { get; set; }

    public virtual DbSet<ChamCong_CheckInOut_v1> ChamCong_CheckInOut_v1s { get; set; }

    public virtual DbSet<DS_HN_temp> DS_HN_temps { get; set; }

    public virtual DbSet<TB_NGHIPHEP> TB_NGHIPHEPs { get; set; }

    public virtual DbSet<tb_ChucNang> tb_ChucNangs { get; set; }

    public virtual DbSet<tb_GroupFunction> tb_GroupFunctions { get; set; }

    public virtual DbSet<tb_donvi> tb_donvis { get; set; }

    public virtual DbSet<tb_giaitrinh_chamcong> tb_giaitrinh_chamcongs { get; set; }

    public virtual DbSet<tb_kybaocao> tb_kybaocaos { get; set; }

    public virtual DbSet<tb_lydovipham> tb_lydoviphams { get; set; }

    public virtual DbSet<tb_nhanviengiatrinh> tb_nhanviengiatrinhs { get; set; }

    public virtual DbSet<tb_trangthaiky> tb_trangthaikies { get; set; }

    public virtual DbSet<tb_viphamchamcongvantay> tb_viphamchamcongvantays { get; set; }

    public virtual DbSet<tb_vpcc_temp> tb_vpcc_temps { get; set; }
    public virtual DbSet<tb_GroupFunctionDetail> tb_GroupFunctionDetails { get; set; }
    public virtual DbSet<tb_Account> tb_Accounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChamCong_CheckInOut_temp>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__ChamCong__3214EC2725DEAECC");

            entity.ToTable("ChamCong_CheckInOut_temp");

            entity.Property(e => e.GioCham).HasColumnType("datetime");
            entity.Property(e => e.NgayCham).HasColumnType("datetime");
            entity.Property(e => e.TenMay).HasMaxLength(50);
            entity.Property(e => e.manhanvien)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ChamCong_CheckInOut_v1>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ChamCong_CheckInOut_v1");

            entity.Property(e => e.GioCham).HasColumnType("datetime");
            entity.Property(e => e.ID)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MaNhanVien)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NgayCham).HasColumnType("datetime");
            entity.Property(e => e.TenMay)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<DS_HN_temp>(entity =>
        {
            entity.HasKey(e => e.mans).HasName("PK__DS_HN_te__7A21B362E1C18E8D");

            entity.ToTable("DS_HN_temp");

            entity.Property(e => e.mans)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TB_NGHIPHEP>(entity =>
        {
            entity.HasKey(e => new { e.MANS, e.NGAY }).HasName("PK__TB_NGHIP__3D30CCB790C87053");

            entity.ToTable("TB_NGHIPHEP");

            entity.Property(e => e.MANS).HasMaxLength(50);
            entity.Property(e => e.CHIEU)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SANG)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<tb_ChucNang>(entity =>
        {
            entity.HasKey(e => e.ChucNangID).HasName("PK__tb_ChucN__2AB8F1DDAA474DC8");

            entity.ToTable("tb_ChucNang");

            entity.Property(e => e.ChucNangID).ValueGeneratedNever();
            entity.Property(e => e.TenChucNang).HasMaxLength(250);
        });

        modelBuilder.Entity<tb_Account>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_tb_Account");

            entity.ToTable("tb_Account");

            entity.Property(e => e.ID)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.MaNhanVien)
                .HasMaxLength(50)
                .IsUnicode(false); 

            entity.Property(e => e.FullName)
                .HasMaxLength(200);

            entity.Property(e => e.UserName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.HashPassword)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);

            entity.Property(e => e.LastLogin)
                .HasColumnType("datetime");

            entity.HasOne(e => e.GroupFunction)
                .WithMany(g => g.Accounts) 
                .HasForeignKey(e => e.GroupFuncID)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_tb_Account_tb_GroupFunction");
        });


        modelBuilder.Entity<tb_GroupFunction>(entity =>
        {
            entity.HasKey(e => e.GroupFuncID).HasName("PK__tb_Group__573A13A1DFFEBEF9");

            entity.ToTable("tb_GroupFunction");

            entity.Property(e => e.GroupFuncID).ValueGeneratedNever();
            entity.Property(e => e.GroupNote)
                .HasMaxLength(200)
                .IsUnicode(false);

            //entity.HasMany(d => d.Funcs).WithMany(p => p.GroupFuncs)
            //    .UsingEntity<Dictionary<string, object>>(
            //        "tb_GroupFunctionDetail",
            //        r => r.HasOne<tb_ChucNang>().WithMany()
            //            .HasForeignKey("FuncID")
            //            .OnDelete(DeleteBehavior.ClientSetNull)
            //            .HasConstraintName("FK__tb_GroupF__FuncI__0C85DE4D"),
            //        l => l.HasOne<tb_GroupFunction>().WithMany()
            //            .HasForeignKey("GroupFuncID")
            //            .OnDelete(DeleteBehavior.ClientSetNull)
            //            .HasConstraintName("FK__tb_GroupF__Group__0B91BA14"),
            //        j =>
            //        {
            //            j.HasKey("GroupFuncID", "FuncID").HasName("PK__tb_Group__7F0ECD8617DB2015");
            //            j.ToTable("tb_GroupFunctionDetail");
            //        });
        });

        modelBuilder.Entity<tb_donvi>(entity =>
        {
            entity.HasKey(e => e.madv).HasName("PK__tb_donvi__7A21E02964655F87");

            entity.ToTable("tb_donvi");

            entity.Property(e => e.madv).HasMaxLength(20);
            entity.Property(e => e.tendv).HasMaxLength(255);
        });

        modelBuilder.Entity<tb_giaitrinh_chamcong>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("tb_giaitrinh_chamcong");

            entity.Property(e => e.viphamid)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.giaitrinh).WithMany()
                .HasForeignKey(d => d.giaitrinhid)
                .HasConstraintName("FK_GTCG_LyDo");

            entity.HasOne(d => d.vipham).WithMany()
                .HasForeignKey(d => d.viphamid)
                .HasConstraintName("FK_GTCG_ViphamID");
        });

        modelBuilder.Entity<tb_kybaocao>(entity =>
        {
            entity.HasKey(e => e.maky).HasName("PK__tb_kybao__7A21BB5B936585F8");

            entity.ToTable("tb_kybaocao");

            entity.Property(e => e.maky).ValueGeneratedOnAdd();
            entity.Property(e => e.tenky).HasMaxLength(500);
        });

        modelBuilder.Entity<tb_lydovipham>(entity =>
        {
            entity.HasKey(e => e.lydoid).HasName("PK__tb_lydov__36CCC97C8A79514D");

            entity.ToTable("tb_lydovipham");

            entity.Property(e => e.lydoid).ValueGeneratedNever();
            entity.Property(e => e.ghichu).HasMaxLength(500);
            entity.Property(e => e.tenlydo).HasMaxLength(500);
        });

        modelBuilder.Entity<tb_nhanviengiatrinh>(entity =>
        {
            entity.HasKey(e => e.EmpID).HasName("PK__tb_nhanv__AF2DBA79CD5A4557");

            entity.ToTable("tb_nhanviengiatrinh");

            entity.Property(e => e.EmpID).HasMaxLength(12);
            entity.Property(e => e.Birthday)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.DirectEmpID).HasMaxLength(12);
            entity.Property(e => e.DirectEmpManager).HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.EmpCode).HasMaxLength(15);
            entity.Property(e => e.FilePhoto).HasMaxLength(200);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(201);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.LSJobGroupID).HasMaxLength(12);
            entity.Property(e => e.LSPosGroupID).HasMaxLength(12);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Level1ID).HasMaxLength(12);
            entity.Property(e => e.Level1Name).HasMaxLength(150);
            entity.Property(e => e.Level2ID).HasMaxLength(12);
            entity.Property(e => e.Level2Name).HasMaxLength(150);
            entity.Property(e => e.Level3ID).HasMaxLength(12);
            entity.Property(e => e.Level3Name).HasMaxLength(150);
            entity.Property(e => e.Mobifone).HasMaxLength(50);
            entity.Property(e => e.PIN)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PINDate)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.PINPlace).HasMaxLength(150);
            entity.Property(e => e.PositionID).HasMaxLength(12);
            entity.Property(e => e.TamTru).HasMaxLength(100);
            entity.Property(e => e.ThuongTru).HasMaxLength(100);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.WorkingDate)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<tb_trangthaiky>(entity =>
        {
            entity.HasKey(e => e.trangthaiid).HasName("PK__tb_trang__4530D32938C5B996");

            entity.ToTable("tb_trangthaiky");

            entity.Property(e => e.trangthaiid).ValueGeneratedNever();
            entity.Property(e => e.trangthaiten).HasMaxLength(50);
        });

        modelBuilder.Entity<tb_GroupFunctionDetail>(entity =>
        {
            entity.HasKey(e => new { e.GroupFuncID, e.FuncID }).HasName("PK__tb_Group__7F0ECD8617DB2015");

            entity.ToTable("tb_GroupFunctionDetail");

            entity.HasOne(d => d.GroupFunc)
                .WithMany(p => p.tb_GroupFunctionDetails)
                .HasForeignKey(d => d.GroupFuncID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_GroupF__Group__0B91BA14");

            entity.HasOne(d => d.Func)
                .WithMany(p => p.tb_GroupFunctionDetails)
                .HasForeignKey(d => d.FuncID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tb_GroupF__FuncI__0C85DE4D");
        });

        modelBuilder.Entity<tb_viphamchamcongvantay>(entity =>
        {
            entity.HasKey(e => e.viphamid).HasName("PK__tb_vipha__624315EC1EADCAFA");

            entity.ToTable("tb_viphamchamcongvantay");

            entity.Property(e => e.viphamid)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.chucdanh).HasMaxLength(500);
            entity.Property(e => e.hoten).HasMaxLength(50);
            entity.Property(e => e.madv)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.mans)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.time_in)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.time_out)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<tb_vpcc_temp>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("tb_vpcc_temp");

            entity.Property(e => e.chucdanh).HasMaxLength(500);
            entity.Property(e => e.hoten).HasMaxLength(50);
            entity.Property(e => e.madv)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.mans)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.time_in)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.time_out)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.viphamid)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
