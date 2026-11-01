using Microsoft.EntityFrameworkCore;

namespace HTQLNS.Models
{
    public static class DbSeeder
    {
        private const string AdminEmail = "trin99832@gmail.com";

        public static void Seed(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // =========================================================
            // 1. PHÒNG BAN
            // =========================================================
            var phongBanSeeds = new List<PhongBan>
            {
                new PhongBan { MaPhongBan = "PB001", TenPhongBan = "Bar & Lounge" },
                new PhongBan { MaPhongBan = "PB002", TenPhongBan = "Sân khấu & Biểu diễn" },
                new PhongBan { MaPhongBan = "PB003", TenPhongBan = "Lễ tân & CSKH" },
                new PhongBan { MaPhongBan = "PB004", TenPhongBan = "Âm thanh ánh sáng" },
                new PhongBan { MaPhongBan = "PB005", TenPhongBan = "An ninh & Nội vụ" }
            };

            foreach (var pb in phongBanSeeds)
            {
                if (!context.PhongBans.Any(p => p.MaPhongBan == pb.MaPhongBan))
                {
                    context.PhongBans.Add(pb);
                }
            }
            context.SaveChanges();

            // =========================================================
            // 2. CHỨC VỤ
            // =========================================================
            var chucVuSeeds = new List<ChucVu>
            {
                new ChucVu { MaChucVu = "CV001", TenChucVu = "Pha chế (Bartender)",   MoTa = "Pha chế đồ uống tại quầy Bar" },
                new ChucVu { MaChucVu = "CV002", TenChucVu = "Lễ tân viên",           MoTa = "Đón tiếp khách và đặt bàn" },
                new ChucVu { MaChucVu = "CV003", TenChucVu = "Quản lý",               MoTa = "Quản lý điều hành bộ phận" },
                new ChucVu { MaChucVu = "CV004", TenChucVu = "Kỹ thuật viên Âm thanh", MoTa = "Vận hành hệ thống âm thanh, ánh sáng" },
                new ChucVu { MaChucVu = "CV005", TenChucVu = "DJ & Biểu diễn",        MoTa = "Biểu diễn âm nhạc sân khấu" },
                new ChucVu { MaChucVu = "CV006", TenChucVu = "Phục vụ bàn",           MoTa = "Phục vụ khách khu vực Lounge" },
                new ChucVu { MaChucVu = "CV007", TenChucVu = "Giám sát An ninh",      MoTa = "Đảm bảo trật tự và an toàn" }
            };

            foreach (var cv in chucVuSeeds)
            {
                if (!context.ChucVus.Any(c => c.MaChucVu == cv.MaChucVu))
                {
                    context.ChucVus.Add(cv);
                }
            }
            context.SaveChanges();

            // =========================================================
            // 3. VAI TRÒ
            // =========================================================
            var vaiTroSeeds = new List<VaiTro>
            {
                new VaiTro { MaVaiTro = "VT001", TenVaiTro = "Admin" },
                new VaiTro { MaVaiTro = "VT002", TenVaiTro = "Nhân viên" }
            };

            foreach (var vt in vaiTroSeeds)
            {
                if (!context.VaiTros.Any(v => v.MaVaiTro == vt.MaVaiTro))
                {
                    context.VaiTros.Add(vt);
                }
            }
            context.SaveChanges();

            var adminVaiTro = context.VaiTros.FirstOrDefault(v => v.TenVaiTro == "Admin");
            var userVaiTro  = context.VaiTros.FirstOrDefault(v => v.TenVaiTro == "Nhân viên");

            // =========================================================
            // 4. NHÂN VIÊN (12 Nhân sự ở các phòng ban)
            // =========================================================
            var nhanVienSeeds = new List<NhanVien>
            {
                new NhanVien { MaNhanVien = "NV001", HoTen = "Trần Minh Quân (Admin)", Email = AdminEmail,                SDT = "0901234567", NgaySinh = new DateTime(1995, 5, 15),  MaPhongBan = "PB001", MaChucVu = "CV003" },
                new NhanVien { MaNhanVien = "NV002", HoTen = "Nguyễn Văn An",          Email = "nhanvien@starzone.vn",    SDT = "0987654321", NgaySinh = new DateTime(2001, 8, 20),  MaPhongBan = "PB001", MaChucVu = "CV001" },
                new NhanVien { MaNhanVien = "NV003", HoTen = "Lê Thị Bảo Ngọc",        Email = "baongoc@starzone.vn",     SDT = "0912345678", NgaySinh = new DateTime(2000, 3, 12),  MaPhongBan = "PB003", MaChucVu = "CV003" },
                new NhanVien { MaNhanVien = "NV004", HoTen = "Phạm Hoàng Nam",         Email = "hoangnam@starzone.vn",    SDT = "0934567890", NgaySinh = new DateTime(1999, 11, 5),  MaPhongBan = "PB002", MaChucVu = "CV005" },
                new NhanVien { MaNhanVien = "NV005", HoTen = "Võ Thị Thanh Tâm",       Email = "thanhtam@starzone.vn",    SDT = "0978123456", NgaySinh = new DateTime(2002, 7, 25),  MaPhongBan = "PB003", MaChucVu = "CV002" },
                new NhanVien { MaNhanVien = "NV006", HoTen = "Đặng Quốc Khánh",        Email = "quockhanh@starzone.vn",   SDT = "0965432109", NgaySinh = new DateTime(1997, 1, 30),  MaPhongBan = "PB004", MaChucVu = "CV003" },
                new NhanVien { MaNhanVien = "NV007", HoTen = "Bùi Tuấn Kiệt",          Email = "tuankiet@starzone.vn",    SDT = "0943216789", NgaySinh = new DateTime(2001, 4, 18),  MaPhongBan = "PB004", MaChucVu = "CV004" },
                new NhanVien { MaNhanVien = "NV008", HoTen = "Hoàng Mai Phương",       Email = "maiphuong@starzone.vn",   SDT = "0923456781", NgaySinh = new DateTime(2003, 9, 10),  MaPhongBan = "PB001", MaChucVu = "CV006" },
                new NhanVien { MaNhanVien = "NV009", HoTen = "Ngô Gia Huy",            Email = "giahuy@starzone.vn",      SDT = "0956781234", NgaySinh = new DateTime(1998, 12, 2),  MaPhongBan = "PB002", MaChucVu = "CV003" },
                new NhanVien { MaNhanVien = "NV010", HoTen = "Đỗ Thùy Linh",           Email = "thuylinh@starzone.vn",    SDT = "0918273645", NgaySinh = new DateTime(2002, 2, 14),  MaPhongBan = "PB003", MaChucVu = "CV002" },
                new NhanVien { MaNhanVien = "NV011", HoTen = "Trương Văn Hùng",        Email = "vanhung@starzone.vn",     SDT = "0909876543", NgaySinh = new DateTime(1994, 6, 8),   MaPhongBan = "PB005", MaChucVu = "CV003" },
                new NhanVien { MaNhanVien = "NV012", HoTen = "Phan Thanh Tùng",        Email = "thanhtung@starzone.vn",   SDT = "0938475610", NgaySinh = new DateTime(2000, 10, 22), MaPhongBan = "PB001", MaChucVu = "CV001" }
            };

            foreach (var nv in nhanVienSeeds)
            {
                if (!context.NhanViens.Any(n => n.MaNhanVien == nv.MaNhanVien))
                {
                    context.NhanViens.Add(nv);
                }
            }
            context.SaveChanges();

            // =========================================================
            // 5. TÀI KHOẢN (Admin, Nhân viên, Đang hoạt động & Chờ duyệt)
            // =========================================================
            var adminRoleId = adminVaiTro?.MaVaiTro ?? "VT001";
            var userRoleId  = userVaiTro?.MaVaiTro  ?? "VT002";

            var taiKhoanSeeds = new List<TaiKhoan>
            {
                new TaiKhoan { MaTaiKhoan = "TK001",    TenDangNhap = AdminEmail,               HoTen = "Trần Minh Quân (Admin)",     MatKhau = "123456", TrangThai = "Hoạt động", MaNhanVien = "NV001", MaVaiTro = adminRoleId, NgaySinh = new DateTime(1995, 5, 15) },
                new TaiKhoan { MaTaiKhoan = "TK_ADMIN", TenDangNhap = "admin",                  HoTen = "Quản trị viên (Admin)",      MatKhau = "123456", TrangThai = "Hoạt động", MaNhanVien = "NV001", MaVaiTro = adminRoleId, NgaySinh = new DateTime(1995, 5, 15) },
                new TaiKhoan { MaTaiKhoan = "TK_STAFF", TenDangNhap = "nhanvien",               HoTen = "Nguyễn Văn An (Nhân viên)",  MatKhau = "123456", TrangThai = "Hoạt động", MaNhanVien = "NV002", MaVaiTro = userRoleId,  NgaySinh = new DateTime(2001, 8, 20) },
                new TaiKhoan { MaTaiKhoan = "TK003",    TenDangNhap = "baongoc@starzone.vn",    HoTen = "Lê Thị Bảo Ngọc",            MatKhau = "123456", TrangThai = "Hoạt động", MaNhanVien = "NV003", MaVaiTro = userRoleId,  NgaySinh = new DateTime(2000, 3, 12) },
                new TaiKhoan { MaTaiKhoan = "TK004",    TenDangNhap = "hoangnam@starzone.vn",   HoTen = "Phạm Hoàng Nam",             MatKhau = "123456", TrangThai = "Hoạt động", MaNhanVien = "NV004", MaVaiTro = userRoleId,  NgaySinh = new DateTime(1999, 11, 5) },
                new TaiKhoan { MaTaiKhoan = "TK005",    TenDangNhap = "thanhtam@starzone.vn",   HoTen = "Võ Thị Thanh Tâm",           MatKhau = "123456", TrangThai = "Hoạt động", MaNhanVien = "NV005", MaVaiTro = userRoleId,  NgaySinh = new DateTime(2002, 7, 25) },
                new TaiKhoan { MaTaiKhoan = "TK006",    TenDangNhap = "quockhanh@starzone.vn",  HoTen = "Đặng Quốc Khánh",            MatKhau = "123456", TrangThai = "Bị khóa",   MaNhanVien = "NV006", MaVaiTro = userRoleId,  NgaySinh = new DateTime(1997, 1, 30) },
                // Tài khoản Chờ duyệt để demo chức năng Duyệt tài khoản
                new TaiKhoan { MaTaiKhoan = "TK_PEND1", TenDangNhap = "minhkhoi.work@gmail.com", HoTen = "Lý Minh Khôi",              MatKhau = "123456", TrangThai = "Chờ duyệt", MaNhanVien = null,    MaVaiTro = userRoleId,  NgaySinh = new DateTime(2003, 4, 11) },
                new TaiKhoan { MaTaiKhoan = "TK_PEND2", TenDangNhap = "khanhvy.le@gmail.com",    HoTen = "Lê Khánh Vy",               MatKhau = "123456", TrangThai = "Chờ duyệt", MaNhanVien = null,    MaVaiTro = userRoleId,  NgaySinh = new DateTime(2004, 9, 28) }
            };

            foreach (var tk in taiKhoanSeeds)
            {
                var existing = context.TaiKhoans.FirstOrDefault(t => t.MaTaiKhoan == tk.MaTaiKhoan || t.TenDangNhap == tk.TenDangNhap);
                if (existing == null)
                {
                    context.TaiKhoans.Add(tk);
                }
                else if (tk.TenDangNhap == "admin" || tk.TenDangNhap == "nhanvien")
                {
                    existing.MatKhau    = "123456";
                    existing.TrangThai  = "Hoạt động";
                    existing.MaVaiTro   = tk.MaVaiTro;
                    existing.MaNhanVien = tk.MaNhanVien;
                }
            }
            context.SaveChanges();

            // =========================================================
            // 6. LOẠI CA LÀM
            // =========================================================
            var loaiCaSeeds = new List<LoaiCaLam>
            {
                new LoaiCaLam { MaLoaiCa = "CA001", TenCa = "Ca Sáng",  GioBatDau = new TimeSpan(8, 0, 0),  GioKetThuc = new TimeSpan(14, 0, 0) },
                new LoaiCaLam { MaLoaiCa = "CA002", TenCa = "Ca Chiều", GioBatDau = new TimeSpan(14, 0, 0), GioKetThuc = new TimeSpan(20, 0, 0) },
                new LoaiCaLam { MaLoaiCa = "CA003", TenCa = "Ca Tối",   GioBatDau = new TimeSpan(18, 0, 0), GioKetThuc = new TimeSpan(23, 59, 0) }
            };

            foreach (var ca in loaiCaSeeds)
            {
                if (!context.LoaiCaLams.Any(c => c.MaLoaiCa == ca.MaLoaiCa))
                {
                    context.LoaiCaLams.Add(ca);
                }
            }
            context.SaveChanges();

            // =========================================================
            // 7. LỊCH PHÂN CÔNG TRONG TUẦN HIỆN TẠI & CHẤM CÔNG
            // =========================================================
            DateTime today = DateTime.Today;
            int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
            DateTime monday = today.AddDays(-1 * diff);

            // Tạo lịch phân công phong phú cho cả tuần (Thứ 2 -> Chủ nhật)
            var weeklyAssignments = new List<(string MaLich, string MaNV, string MaCa, int DayOffset, string TrangThai)>
            {
                // Thứ 2 (offset 0)
                ("LC_W01", "NV012", "CA001", 0, "Hoàn thành"),
                ("LC_W02", "NV005", "CA001", 0, "Hoàn thành"),
                ("LC_W03", "NV003", "CA002", 0, "Hoàn thành"),
                ("LC_W04", "NV008", "CA002", 0, "Hoàn thành"),
                ("LC_W05", "NV004", "CA003", 0, "Hoàn thành"),
                ("LC_W06", "NV007", "CA003", 0, "Hoàn thành"),
                ("LC_W07", "NV009", "CA003", 0, "Hoàn thành"),

                // Thứ 3 (offset 1)
                ("LC_W08", "NV005", "CA001", 1, "Hoàn thành"),
                ("LC_W09", "NV010", "CA001", 1, "Hoàn thành"),
                ("LC_W10", "NV008", "CA002", 1, "Hoàn thành"),
                ("LC_W11", "NV006", "CA002", 1, "Hoàn thành"),
                ("LC_W12", "NV004", "CA003", 1, "Hoàn thành"),
                ("LC_W13", "NV009", "CA003", 1, "Hoàn thành"),

                // Thứ 4 (offset 2)
                ("LC_W14", "NV012", "CA001", 2, "Hoàn thành"),
                ("LC_W15", "NV003", "CA001", 2, "Hoàn thành"),
                ("LC_W16", "NV008", "CA002", 2, "Hoàn thành"),
                ("LC_W17", "NV010", "CA002", 2, "Hoàn thành"),
                ("LC_W18", "NV007", "CA003", 2, "Hoàn thành"),
                ("LC_W19", "NV011", "CA003", 2, "Hoàn thành"),

                // Thứ 5 (offset 3)
                ("LC_W20", "NV005", "CA001", 3, "Hoàn thành"),
                ("LC_W21", "NV012", "CA001", 3, "Hoàn thành"),
                ("LC_W22", "NV006", "CA002", 3, "Hoàn thành"),
                ("LC_W23", "NV003", "CA002", 3, "Hoàn thành"),
                ("LC_W24", "NV004", "CA003", 3, "Hoàn thành"),
                ("LC_W25", "NV009", "CA003", 3, "Hoàn thành"),

                // Thứ 6 (offset 4)
                ("LC_W26", "NV003", "CA001", 4, "Sắp tới"),
                ("LC_W27", "NV010", "CA001", 4, "Sắp tới"),
                ("LC_W28", "NV005", "CA002", 4, "Sắp tới"),
                ("LC_W29", "NV008", "CA002", 4, "Sắp tới"),
                ("LC_W30", "NV004", "CA003", 4, "Sắp tới"),
                ("LC_W31", "NV007", "CA003", 4, "Sắp tới"),
                ("LC_W32", "NV012", "CA003", 4, "Sắp tới"),

                // Thứ 7 (offset 5)
                ("LC_W33", "NV003", "CA001", 5, "Sắp tới"),
                ("LC_W34", "NV005", "CA002", 5, "Sắp tới"),
                ("LC_W35", "NV008", "CA002", 5, "Sắp tới"),
                ("LC_W36", "NV012", "CA003", 5, "Sắp tới"),
                ("LC_W37", "NV004", "CA003", 5, "Sắp tới"),
                ("LC_W38", "NV009", "CA003", 5, "Sắp tới"),
                ("LC_W39", "NV011", "CA003", 5, "Sắp tới"),

                // Chủ Nhật (offset 6)
                ("LC_W40", "NV010", "CA001", 6, "Sắp tới"),
                ("LC_W41", "NV012", "CA002", 6, "Sắp tới"),
                ("LC_W42", "NV004", "CA003", 6, "Sắp tới"),
                ("LC_W43", "NV007", "CA003", 6, "Sắp tới"),
                ("LC_W44", "NV008", "CA003", 6, "Sắp tới"),

                // Lịch riêng cho NV002 (Nguyễn Văn An - tài khoản nhanvien) cả 7 ngày trong tuần
                ("LC_NV2_D0", "NV002", "CA001", 0, "Sắp tới"),
                ("LC_NV2_D1", "NV002", "CA002", 1, "Sắp tới"),
                ("LC_NV2_D2", "NV002", "CA001", 2, "Sắp tới"),
                ("LC_NV2_D3", "NV002", "CA002", 3, "Sắp tới"),
                ("LC_NV2_D4", "NV002", "CA001", 4, "Sắp tới"),
                ("LC_NV2_D5", "NV002", "CA003", 5, "Sắp tới"),
                ("LC_NV2_D6", "NV002", "CA002", 6, "Sắp tới"),

                // Lịch quá khứ cho NV002 (tuần trước) để có lịch sử chấm công khi xem ở tài khoản Nhân viên
                ("LC_NV2_P1", "NV002", "CA001", -3, "Hoàn thành"),
                ("LC_NV2_P2", "NV002", "CA002", -2, "Hoàn thành"),
                ("LC_NV2_P3", "NV002", "CA001", -1, "Hoàn thành")
            };

            foreach (var item in weeklyAssignments)
            {
                var shiftDate = monday.AddDays(item.DayOffset);
                var existingLich = context.LichPhanCongs.FirstOrDefault(l => l.MaLich == item.MaLich);
                if (existingLich == null)
                {
                    context.LichPhanCongs.Add(new LichPhanCong
                    {
                        MaLich     = item.MaLich,
                        MaNhanVien = item.MaNV,
                        MaLoaiCa   = item.MaCa,
                        NgayLam    = shiftDate,
                        TrangThai  = item.TrangThai
                    });
                }
                else
                {
                    existingLich.NgayLam    = shiftDate;
                    existingLich.MaNhanVien = item.MaNV;
                }
            }
            context.SaveChanges();

            // =========================================================
            // 8. DỮ LIỆU CHẤM CÔNG MẪU (Đúng giờ, Trễ giờ, Vắng mặt)
            // =========================================================
            var chamCongSeeds = new List<ChamCong>
            {
                new ChamCong { MaChamCong = "CC001",    MaLich = "LC_W01",    ThoiGianChekin = today.AddHours(7).AddMinutes(55),  ThoiGianCheckout = today.AddHours(14).AddMinutes(5),  TrangThai = "Đúng giờ" },
                new ChamCong { MaChamCong = "CC002",    MaLich = "LC_W02",    ThoiGianChekin = today.AddHours(7).AddMinutes(58),  ThoiGianCheckout = today.AddHours(14).AddMinutes(2),  TrangThai = "Đúng giờ" },
                new ChamCong { MaChamCong = "CC003",    MaLich = "LC_W03",    ThoiGianChekin = today.AddHours(13).AddMinutes(50), ThoiGianCheckout = today.AddHours(20).AddMinutes(10), TrangThai = "Đúng giờ" },
                new ChamCong { MaChamCong = "CC004",    MaLich = "LC_W04",    ThoiGianChekin = today.AddHours(14).AddMinutes(18), ThoiGianCheckout = today.AddHours(20).AddMinutes(5),  TrangThai = "Trễ giờ" },
                new ChamCong { MaChamCong = "CC005",    MaLich = "LC_W05",    ThoiGianChekin = today.AddHours(17).AddMinutes(55), ThoiGianCheckout = today.AddHours(23).AddMinutes(58), TrangThai = "Đúng giờ" },
                new ChamCong { MaChamCong = "CC006",    MaLich = "LC_W06",    ThoiGianChekin = today.AddHours(17).AddMinutes(50), ThoiGianCheckout = today.AddHours(23).AddMinutes(59), TrangThai = "Đúng giờ" },
                new ChamCong { MaChamCong = "CC007",    MaLich = "LC_W07",    ThoiGianChekin = today.AddHours(18).AddMinutes(25), ThoiGianCheckout = today.AddHours(23).AddMinutes(59), TrangThai = "Trễ giờ" },
                new ChamCong { MaChamCong = "CC008",    MaLich = "LC_W08",    ThoiGianChekin = today.AddHours(7).AddMinutes(52),  ThoiGianCheckout = today.AddHours(14).AddMinutes(0),  TrangThai = "Đúng giờ" },
                new ChamCong { MaChamCong = "CC009",    MaLich = "LC_W09",    ThoiGianChekin = null,                              ThoiGianCheckout = null,                              TrangThai = "Vắng mặt" },
                new ChamCong { MaChamCong = "CC010",    MaLich = "LC_W10",    ThoiGianChekin = today.AddHours(13).AddMinutes(55), ThoiGianCheckout = today.AddHours(20).AddMinutes(15), TrangThai = "Đúng giờ" },
                // Lịch sử chấm công của NV002 (Nguyễn Văn An)
                new ChamCong { MaChamCong = "CC_NV2_1", MaLich = "LC_NV2_P1", ThoiGianChekin = today.AddDays(-3).AddHours(7).AddMinutes(54),  ThoiGianCheckout = today.AddDays(-3).AddHours(14).AddMinutes(3),  TrangThai = "Đúng giờ" },
                new ChamCong { MaChamCong = "CC_NV2_2", MaLich = "LC_NV2_P2", ThoiGianChekin = today.AddDays(-2).AddHours(14).AddMinutes(12), ThoiGianCheckout = today.AddDays(-2).AddHours(20).AddMinutes(5),  TrangThai = "Trễ giờ" },
                new ChamCong { MaChamCong = "CC_NV2_3", MaLich = "LC_NV2_P3", ThoiGianChekin = today.AddDays(-1).AddHours(7).AddMinutes(50),  ThoiGianCheckout = today.AddDays(-1).AddHours(14).AddMinutes(10), TrangThai = "Đúng giờ" }
            };

            foreach (var cc in chamCongSeeds)
            {
                if (!context.ChamCongs.Any(c => c.MaChamCong == cc.MaChamCong))
                {
                    context.ChamCongs.Add(cc);
                }
            }
            context.SaveChanges();

            // =========================================================
            // 9. ĐƠN XIN NGHỈ & YÊU CẦU ĐỔI CA
            // =========================================================
            var donXinNghiSeeds = new List<DonXinNghi>
            {
                new DonXinNghi { MaDon = "DN001", MaNhanVien = "NV002", TuNgay = today.AddDays(2),  DenNgay = today.AddDays(3),  LyDo = "Giải quyết việc gia đình ở quê",          TrangThai = "Chờ duyệt" },
                new DonXinNghi { MaDon = "DN002", MaNhanVien = "NV005", TuNgay = today.AddDays(1),  DenNgay = today.AddDays(1),  LyDo = "Khám sức khỏe định kỳ tại bệnh viện",     TrangThai = "Chờ duyệt" },
                new DonXinNghi { MaDon = "DN003", MaNhanVien = "NV008", TuNgay = today.AddDays(4),  DenNgay = today.AddDays(5),  LyDo = "Tham dự lễ tốt nghiệp đại học",           TrangThai = "Chờ duyệt" },
                new DonXinNghi { MaDon = "DN004", MaNhanVien = "NV004", TuNgay = today.AddDays(-5), DenNgay = today.AddDays(-4), LyDo = "Nghỉ phép năm theo chế độ",               TrangThai = "Đã duyệt" },
                new DonXinNghi { MaDon = "DN005", MaNhanVien = "NV007", TuNgay = today.AddDays(-3), DenNgay = today.AddDays(-3), LyDo = "Bị cảm sốt, xin nghỉ 1 ca tối",           TrangThai = "Đã duyệt" },
                new DonXinNghi { MaDon = "DN006", MaNhanVien = "NV010", TuNgay = today.AddDays(-7), DenNgay = today.AddDays(-6), LyDo = "Đi đám cưới anh ruột tại Đà Nẵng",        TrangThai = "Đã duyệt" }
            };

            foreach (var don in donXinNghiSeeds)
            {
                if (!context.DonXinNghis.Any(d => d.MaDon == don.MaDon))
                {
                    context.DonXinNghis.Add(don);
                }
            }
            context.SaveChanges();

            var yeuCauDoiCaSeeds = new List<YeuCauDoiCa>
            {
                new YeuCauDoiCa { MaYeuCau = "DC001", MaNhanVien = "NV002", MaLichCu = "LC_W26", MaLichMoi = "LC_W30", NgayGui = today,            LyDo = "Bận lịch thi học kỳ buổi sáng, xin đổi sang ca tối", TrangThai = "Chờ duyệt" },
                new YeuCauDoiCa { MaYeuCau = "DC002", MaNhanVien = "NV005", MaLichCu = "LC_W28", MaLichMoi = "LC_W33", NgayGui = today.AddDays(-1), LyDo = "Đổi ca trực quầy lễ tân với Bảo Ngọc",               TrangThai = "Chờ duyệt" },
                new YeuCauDoiCa { MaYeuCau = "DC003", MaNhanVien = "NV012", MaLichCu = "LC_W21", MaLichMoi = "LC_W32", NgayGui = today.AddDays(-2), LyDo = "Có lịch hẹn nha khoa buổi sáng",                     TrangThai = "Đã duyệt" }
            };

            foreach (var dc in yeuCauDoiCaSeeds)
            {
                if (!context.YeuCauDoiCas.Any(y => y.MaYeuCau == dc.MaYeuCau))
                {
                    context.YeuCauDoiCas.Add(dc);
                }
            }
            context.SaveChanges();
        }
    }
}
