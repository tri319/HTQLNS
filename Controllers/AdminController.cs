using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HTQLNS.Models;
using Microsoft.EntityFrameworkCore;

namespace HTQLNS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =============================================
        // QUẢN LÝ TÀI KHOẢN CHỜ DUYỆT
        // =============================================

        public async Task<IActionResult> PendingAccounts()
        {
            var pending = await _context.TaiKhoans
                .Include(t => t.NhanVien)
                .Include(t => t.VaiTro)
                .Where(t => t.TrangThai == "Chờ duyệt")
                .OrderByDescending(t => t.NgaySinh)
                .ToListAsync();

            ViewBag.NhanViens = await _context.NhanViens.ToListAsync();
            ViewBag.VaiTros   = await _context.VaiTros.ToListAsync();
            ViewBag.PhongBans = await _context.PhongBans.ToListAsync();
            ViewBag.ChucVus   = await _context.ChucVus.ToListAsync();

            return View(pending);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveAccount(string maTaiKhoan, string hoTen, string maNhanVienMoi, string maPhongBan, string maChucVu, string maVaiTro)
        {
            var tk = await _context.TaiKhoans
                .Include(t => t.NhanVien)
                .FirstOrDefaultAsync(t => t.MaTaiKhoan == maTaiKhoan);

            if (tk == null)
            {
                TempData["Error"] = "Không tìm thấy tài khoản!";
                return RedirectToAction("PendingAccounts");
            }

            maNhanVienMoi = maNhanVienMoi?.Trim().ToUpper() ?? "";

            // Kiểm tra trùng mã nhân viên để tránh lỗi Primary Key
            if (string.IsNullOrEmpty(maNhanVienMoi) || await _context.NhanViens.AnyAsync(n => n.MaNhanVien == maNhanVienMoi))
            {
                var existingIds = await _context.NhanViens.Select(n => n.MaNhanVien).ToListAsync();
                int nextNum = 1;
                while (existingIds.Contains($"NV{nextNum:D3}"))
                {
                    nextNum++;
                }
                string suggestedId = $"NV{nextNum:D3}";

                TempData["Error"] = $"Mã nhân viên '{maNhanVienMoi}' đã tồn tại trong hệ thống! Vui lòng sử dụng mã chưa có (ví dụ: {suggestedId}).";
                return RedirectToAction("PendingAccounts");
            }

            // Tạo nhân viên mới
            var newNv = new NhanVien
            {
                MaNhanVien = maNhanVienMoi,
                HoTen      = hoTen,
                Email      = tk.TenDangNhap,
                NgaySinh   = tk.NgaySinh,
                MaPhongBan = maPhongBan,
                MaChucVu   = maChucVu
            };
            _context.NhanViens.Add(newNv);
            await _context.SaveChangesAsync();

            // Cập nhật tài khoản
            tk.MaNhanVien = newNv.MaNhanVien;
            tk.HoTen      = hoTen;
            tk.MaVaiTro   = maVaiTro;
            tk.TrangThai  = "Hoạt động";
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Đã duyệt tài khoản {tk.TenDangNhap} và gán mã nhân viên {maNhanVienMoi}.";
            return RedirectToAction("PendingAccounts");
        }

        [HttpPost]
        public async Task<IActionResult> RejectAccount(string maTaiKhoan, string lyDo)
        {
            var tk = await _context.TaiKhoans.FindAsync(maTaiKhoan);
            if (tk != null)
            {
                tk.TrangThai = "Từ chối";
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Đã từ chối tài khoản {tk.TenDangNhap}.";
            }
            return RedirectToAction("PendingAccounts");
        }

        // =============================================
        // QUẢN LÝ TẤT CẢ TÀI KHOẢN
        // =============================================

        public async Task<IActionResult> AllAccounts()
        {
            var accounts = await _context.TaiKhoans
                .Include(t => t.NhanVien)
                .Include(t => t.VaiTro)
                .OrderBy(t => t.TrangThai)
                .ToListAsync();

            ViewBag.VaiTros = await _context.VaiTros.ToListAsync();
            return View(accounts);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleAccountStatus(string maTaiKhoan)
        {
            var tk = await _context.TaiKhoans.FindAsync(maTaiKhoan);
            if (tk != null)
            {
                tk.TrangThai = tk.TrangThai == "Hoạt động" ? "Bị khóa" : "Hoạt động";
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Đã cập nhật trạng thái tài khoản {tk.TenDangNhap}.";
            }
            return RedirectToAction("AllAccounts");
        }

        [HttpPost]
        public async Task<IActionResult> ChangeRole(string maTaiKhoan, string maVaiTro)
        {
            var tk = await _context.TaiKhoans.FindAsync(maTaiKhoan);
            if (tk != null)
            {
                tk.MaVaiTro = maVaiTro;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã cập nhật vai trò tài khoản.";
            }
            return RedirectToAction("AllAccounts");
        }
    }
}
