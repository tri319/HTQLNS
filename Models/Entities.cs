using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HTQLNS.Models
{
    public class TaiKhoan
    {
        [Key]
        public string MaTaiKhoan { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        [StringLength(50)]
        public string TenDangNhap { get; set; } = string.Empty;
        
        [Required]
        [StringLength(255)]
        public string MatKhau { get; set; } = string.Empty;
        
        public string? TrangThai { get; set; }
        
        public string MaNhanVien { get; set; } = string.Empty;
        [ForeignKey("MaNhanVien")]
        public virtual NhanVien? NhanVien { get; set; }

        public string MaVaiTro { get; set; } = string.Empty;
        [ForeignKey("MaVaiTro")]
        public virtual VaiTro? VaiTro { get; set; }
    }

    public class VaiTro
    {
        [Key]
        public string MaVaiTro { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        [StringLength(100)]
        public string TenVaiTro { get; set; } = string.Empty;
    }

    public class PhongBan
    {
        [Key]
        public string MaPhongBan { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        [StringLength(100)]
        public string TenPhongBan { get; set; } = string.Empty;
    }

    public class ChucVu
    {
        [Key]
        public string MaChucVu { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        [StringLength(100)]
        public string TenChucVu { get; set; } = string.Empty;
        
        public string? MoTa { get; set; }
    }

    public class NhanVien
    {
        [Key]
        public string MaNhanVien { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        [StringLength(100)]
        public string HoTen { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string? SDT { get; set; }
        
        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }
        
        public string MaPhongBan { get; set; } = string.Empty;
        [ForeignKey("MaPhongBan")]
        public virtual PhongBan? PhongBan { get; set; }

        public string MaChucVu { get; set; } = string.Empty;
        [ForeignKey("MaChucVu")]
        public virtual ChucVu? ChucVu { get; set; }
    }

    public class LoaiCaLam
    {
        [Key]
        public string MaLoaiCa { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        [StringLength(100)]
        public string TenCa { get; set; } = string.Empty;
        
        public TimeSpan GioBatDau { get; set; }
        public TimeSpan GioKetThuc { get; set; }
    }

    public class LichPhanCong
    {
        [Key]
        public string MaLich { get; set; } = Guid.NewGuid().ToString();
        
        public DateTime NgayLam { get; set; }
        
        [StringLength(50)]
        public string? TrangThai { get; set; }
        
        public string MaNhanVien { get; set; } = string.Empty;
        [ForeignKey("MaNhanVien")]
        public virtual NhanVien? NhanVien { get; set; }

        public string MaLoaiCa { get; set; } = string.Empty;
        [ForeignKey("MaLoaiCa")]
        public virtual LoaiCaLam? LoaiCaLam { get; set; }
    }

    public class ChamCong
    {
        [Key]
        public string MaChamCong { get; set; } = Guid.NewGuid().ToString();
        
        public DateTime? ThoiGianChekin { get; set; }
        public DateTime? ThoiGianCheckout { get; set; }
        
        [StringLength(50)]
        public string? TrangThai { get; set; }
        
        public string MaLich { get; set; } = string.Empty;
        [ForeignKey("MaLich")]
        public virtual LichPhanCong? LichPhanCong { get; set; }
    }

    public class DonXinNghi
    {
        [Key]
        public string MaDon { get; set; } = Guid.NewGuid().ToString();
        
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        
        public string? LyDo { get; set; }
        
        [StringLength(50)]
        public string? TrangThai { get; set; } // Chờ duyệt, Đã duyệt, Từ chối
        
        public string MaNhanVien { get; set; } = string.Empty;
        [ForeignKey("MaNhanVien")]
        public virtual NhanVien? NhanVien { get; set; }
    }

    public class YeuCauDoiCa
    {
        [Key]
        public string MaYeuCau { get; set; } = Guid.NewGuid().ToString();
        
        public DateTime NgayGui { get; set; }
        
        public string? LyDo { get; set; }
        
        [StringLength(50)]
        public string? TrangThai { get; set; }
        
        public string MaNhanVien { get; set; } = string.Empty;
        [ForeignKey("MaNhanVien")]
        public virtual NhanVien? NhanVien { get; set; }

        public string MaLichCu { get; set; } = string.Empty;
        [ForeignKey("MaLichCu")]
        public virtual LichPhanCong? LichCu { get; set; }

        public string MaLichMoi { get; set; } = string.Empty;
        [ForeignKey("MaLichMoi")]
        public virtual LichPhanCong? LichMoi { get; set; }
    }
    
    public class NhatKyThaoTac
    {
        [Key]
        public string MaLog { get; set; } = Guid.NewGuid().ToString();
        
        public DateTime ThoiGian { get; set; } = DateTime.Now;
        
        [StringLength(100)]
        public string? HanhDong { get; set; }
        
        public string? NoiDung { get; set; }
        
        public string MaTaiKhoan { get; set; } = string.Empty;
        [ForeignKey("MaTaiKhoan")]
        public virtual TaiKhoan? TaiKhoan { get; set; }
    }
}
