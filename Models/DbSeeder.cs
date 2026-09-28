using Microsoft.EntityFrameworkCore;

namespace HTQLNS.Models
{
    public static class DbSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (!context.PhongBans.Any())
            {
                var barLounge = new PhongBan { MaPhongBan = "PB001", TenPhongBan = "Bar & Lounge" };
                var sanKhau = new PhongBan { MaPhongBan = "PB002", TenPhongBan = "Sân khấu & Biểu diễn" };
                var leTan = new PhongBan { MaPhongBan = "PB003", TenPhongBan = "Lễ tân" };
                var amThanh = new PhongBan { MaPhongBan = "PB004", TenPhongBan = "Âm thanh ánh sáng" };
                
                context.PhongBans.AddRange(barLounge, sanKhau, leTan, amThanh);
                
                var phaChe = new ChucVu { MaChucVu = "CV001", TenChucVu = "Pha chế", MoTa = "Bartender" };
                var tiepTan = new ChucVu { MaChucVu = "CV002", TenChucVu = "Lễ tân viên", MoTa = "Đón khách" };
                var quanLy = new ChucVu { MaChucVu = "CV003", TenChucVu = "Quản lý", MoTa = "Quản lý chung" };
                
                context.ChucVus.AddRange(phaChe, tiepTan, quanLy);
                context.SaveChanges();

                var nv1 = new NhanVien { MaNhanVien = "NV001", HoTen = "Trần Thị Bích", SDT = "0987654321", Email = "bich.tran@starzone.com", MaPhongBan = barLounge.MaPhongBan, MaChucVu = phaChe.MaChucVu };
                var nv2 = new NhanVien { MaNhanVien = "NV002", HoTen = "Nguyễn Văn An", SDT = "0912345678", Email = "an.nguyen@starzone.com", MaPhongBan = barLounge.MaPhongBan, MaChucVu = phaChe.MaChucVu };
                var nv3 = new NhanVien { MaNhanVien = "NV003", HoTen = "Phạm Quỳnh Anh", SDT = "0909090909", Email = "anh.pham@starzone.com", MaPhongBan = leTan.MaPhongBan, MaChucVu = tiepTan.MaChucVu };
                var nv4 = new NhanVien { MaNhanVien = "NV004", HoTen = "Lê Minh Hoàng", SDT = "0933333333", Email = "hoang.le@starzone.com", MaPhongBan = sanKhau.MaPhongBan, MaChucVu = quanLy.MaChucVu };
                
                context.NhanViens.AddRange(nv1, nv2, nv3, nv4);
                context.SaveChanges();
                
                // LoaiCaLam
                var caSang = new LoaiCaLam { MaLoaiCa = "CA001", TenCa = "Ca sáng", GioBatDau = new TimeSpan(6, 0, 0), GioKetThuc = new TimeSpan(14, 0, 0) };
                var caChieu = new LoaiCaLam { MaLoaiCa = "CA002", TenCa = "Ca chiều", GioBatDau = new TimeSpan(14, 0, 0), GioKetThuc = new TimeSpan(22, 0, 0) };
                var caToi = new LoaiCaLam { MaLoaiCa = "CA003", TenCa = "Ca tối", GioBatDau = new TimeSpan(22, 0, 0), GioKetThuc = new TimeSpan(6, 0, 0) };
                context.LoaiCaLams.AddRange(caSang, caChieu, caToi);
                
                // LichPhanCong
                var lich1 = new LichPhanCong { MaLich = "LC001", NgayLam = DateTime.Today, MaNhanVien = nv1.MaNhanVien, MaLoaiCa = caSang.MaLoaiCa, TrangThai = "Sắp tới" };
                var lich2 = new LichPhanCong { MaLich = "LC002", NgayLam = DateTime.Today, MaNhanVien = nv2.MaNhanVien, MaLoaiCa = caChieu.MaLoaiCa, TrangThai = "Đang làm" };
                var lich3 = new LichPhanCong { MaLich = "LC003", NgayLam = DateTime.Today, MaNhanVien = nv3.MaNhanVien, MaLoaiCa = caSang.MaLoaiCa, TrangThai = "Hoàn thành" };
                var lich4 = new LichPhanCong { MaLich = "LC004", NgayLam = DateTime.Today.AddDays(1), MaNhanVien = nv4.MaNhanVien, MaLoaiCa = caToi.MaLoaiCa, TrangThai = "Sắp tới" };
                context.LichPhanCongs.AddRange(lich1, lich2, lich3, lich4);
                
                // ChamCong
                var cc1 = new ChamCong { MaChamCong = "CC001", MaLich = lich3.MaLich, ThoiGianChekin = DateTime.Today.AddHours(5).AddMinutes(58), ThoiGianCheckout = DateTime.Today.AddHours(14), TrangThai = "Đúng giờ" };
                var cc2 = new ChamCong { MaChamCong = "CC002", MaLich = lich2.MaLich, ThoiGianChekin = DateTime.Today.AddHours(14).AddMinutes(10), TrangThai = "Trễ giờ" };
                var cc3 = new ChamCong { MaChamCong = "CC003", MaLich = lich1.MaLich, TrangThai = "Vắng mặt" };
                context.ChamCongs.AddRange(cc1, cc2, cc3);
                
                // DonXinNghi
                var don1 = new DonXinNghi { MaDon = "DN001", MaNhanVien = nv3.MaNhanVien, TuNgay = DateTime.Today.AddDays(1), DenNgay = DateTime.Today.AddDays(3), LyDo = "Việc gia đình", TrangThai = "Chờ duyệt" };
                var don2 = new DonXinNghi { MaDon = "DN002", MaNhanVien = nv2.MaNhanVien, TuNgay = DateTime.Today.AddDays(5), DenNgay = DateTime.Today.AddDays(5), LyDo = "Khám sức khỏe", TrangThai = "Chờ duyệt" };
                context.DonXinNghis.AddRange(don1, don2);
                
                // YeuCauDoiCa
                var dc1 = new YeuCauDoiCa { MaYeuCau = "DC001", MaNhanVien = nv1.MaNhanVien, MaLichCu = lich1.MaLich, MaLichMoi = lich2.MaLich, NgayGui = DateTime.Today.AddDays(-1), LyDo = "Lịch cá nhân", TrangThai = "Đã duyệt" };
                context.YeuCauDoiCas.Add(dc1);
                
                context.SaveChanges();
            }
        }
    }
}
