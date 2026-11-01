using Microsoft.EntityFrameworkCore;

namespace HTQLNS.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<VaiTro> VaiTros { get; set; }
        public DbSet<PhongBan> PhongBans { get; set; }
        public DbSet<ChucVu> ChucVus { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }
        public DbSet<LoaiCaLam> LoaiCaLams { get; set; }
        public DbSet<LichPhanCong> LichPhanCongs { get; set; }
        public DbSet<ChamCong> ChamCongs { get; set; }
        public DbSet<DonXinNghi> DonXinNghis { get; set; }
        public DbSet<YeuCauDoiCa> YeuCauDoiCas { get; set; }
        public DbSet<NhatKyThaoTac> NhatKyThaoTacs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<YeuCauDoiCa>()
                .HasOne(y => y.LichCu)
                .WithMany()
                .HasForeignKey(y => y.MaLichCu)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<YeuCauDoiCa>()
                .HasOne(y => y.LichMoi)
                .WithMany()
                .HasForeignKey(y => y.MaLichMoi)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
