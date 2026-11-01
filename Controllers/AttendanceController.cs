using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HTQLNS.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System;

namespace HTQLNS.Controllers
{
    public class AttendanceViewModel
    {
        public string MaChamCong { get; set; } = string.Empty;
        public string MaNV { get; set; } = string.Empty;
        public string NhanVienTen { get; set; } = string.Empty;
        public string PhongBanTen { get; set; } = string.Empty;
        public string AvatarChar { get; set; } = string.Empty;
        public string AvatarColor { get; set; } = string.Empty;
        
        public string CaLam { get; set; } = string.Empty;
        public string GioVao { get; set; } = string.Empty;
        public string GioRa { get; set; } = string.Empty;
        public string OT { get; set; } = string.Empty;
        public string TrangThai { get; set; } = string.Empty;
        public string MaLich { get; set; } = string.Empty;
    }

    [Authorize]
    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("HR") || User.IsInRole("Quản lý");
            var myMaNhanVien = User.FindFirst("MaNhanVien")?.Value;

            var chamCongsQuery = _context.ChamCongs.Include(c => c.LichPhanCong).AsQueryable();
            var lichQuery = _context.LichPhanCongs.AsQueryable();

            if (!isAdmin)
            {
                if (string.IsNullOrEmpty(myMaNhanVien))
                {
                    // No employee ID mapped
                    return View(new List<AttendanceViewModel>());
                }
                lichQuery = lichQuery.Where(l => l.MaNhanVien == myMaNhanVien);
            }

            var lichIds = await lichQuery.Select(l => l.MaLich).ToListAsync();
            var chamCongs = await chamCongsQuery.Where(c => lichIds.Contains(c.MaLich)).ToListAsync();
            var lichPhanCongs = await _context.LichPhanCongs.Where(l => lichIds.Contains(l.MaLich)).ToDictionaryAsync(l => l.MaLich);
            
            var nhanViens = await _context.NhanViens.ToDictionaryAsync(n => n.MaNhanVien);
            var phongBans = await _context.PhongBans.ToDictionaryAsync(p => p.MaPhongBan, p => p.TenPhongBan);
            var loaiCas = await _context.LoaiCaLams.ToDictionaryAsync(c => c.MaLoaiCa, c => c.TenCa);
            
            var viewModels = new List<AttendanceViewModel>();
            var colors = new[] { "#a881e6", "#1bb99a", "#f59e0b", "#f34343", "#3b82f6" };
            int colorIndex = 0;
            
            foreach (var cc in chamCongs)
            {
                if (lichPhanCongs.TryGetValue(cc.MaLich, out var lich))
                {
                    if (nhanViens.TryGetValue(lich.MaNhanVien, out var nv))
                    {
                        viewModels.Add(new AttendanceViewModel
                        {
                            MaChamCong = cc.MaChamCong,
                            MaNV = nv.MaNhanVien,
                            NhanVienTen = nv.HoTen,
                            PhongBanTen = phongBans.ContainsKey(nv.MaPhongBan) ? phongBans[nv.MaPhongBan] : "N/A",
                            AvatarChar = string.IsNullOrEmpty(nv.HoTen) ? "U" : nv.HoTen.Substring(nv.HoTen.LastIndexOf(' ') + 1, 1).ToUpper(),
                            AvatarColor = colors[(colorIndex++) % colors.Length],
                            CaLam = loaiCas.ContainsKey(lich.MaLoaiCa) ? loaiCas[lich.MaLoaiCa] : "N/A",
                            GioVao = cc.ThoiGianChekin?.ToString("HH:mm") ?? "—",
                            GioRa = cc.ThoiGianCheckout?.ToString("HH:mm") ?? "—",
                            OT = "—",
                            TrangThai = cc.TrangThai ?? "Chưa rõ",
                            MaLich = cc.MaLich
                        });
                    }
                }
            }

            ViewBag.IsAdmin = isAdmin;
            
            // For employee, find if they have a shift today to check in/out
            if (!isAdmin && !string.IsNullOrEmpty(myMaNhanVien))
            {
                var today = DateTime.Today;
                var todayShift = await _context.LichPhanCongs
                    .FirstOrDefaultAsync(l => l.MaNhanVien == myMaNhanVien && l.NgayLam.Date == today);
                
                if (todayShift != null)
                {
                    ViewBag.TodayShift = todayShift;
                    ViewBag.CaLamToday = loaiCas.ContainsKey(todayShift.MaLoaiCa) ? loaiCas[todayShift.MaLoaiCa] : "Ca làm";
                    
                    var chamCongToday = await _context.ChamCongs.FirstOrDefaultAsync(c => c.MaLich == todayShift.MaLich);
                    ViewBag.ChamCongToday = chamCongToday;
                }
            }

            return View(viewModels.OrderByDescending(v => v.GioVao).ToList());
        }

        [HttpPost]
        public async Task<IActionResult> CheckIn(string maLich)
        {
            var myMaNhanVien = User.FindFirst("MaNhanVien")?.Value;
            if (string.IsNullOrEmpty(myMaNhanVien)) return Unauthorized();

            var lich = await _context.LichPhanCongs.FindAsync(maLich);
            if (lich == null || lich.MaNhanVien != myMaNhanVien) return NotFound();

            var existingCC = await _context.ChamCongs.FirstOrDefaultAsync(c => c.MaLich == maLich);
            if (existingCC != null)
            {
                TempData["Error"] = "Bạn đã check-in cho ca này rồi.";
                return RedirectToAction(nameof(Index));
            }

            // Simplified status logic
            string status = "Đúng giờ"; // We would ideally check LoaiCaLam.GioBatDau

            var newCC = new ChamCong
            {
                MaChamCong = "CC" + DateTime.Now.Ticks.ToString().Substring(10, 4),
                MaLich = maLich,
                ThoiGianChekin = DateTime.Now,
                TrangThai = status
            };

            lich.TrangThai = "Đang làm";

            _context.ChamCongs.Add(newCC);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Check-in thành công lúc " + DateTime.Now.ToString("HH:mm");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> CheckOut(string maLich)
        {
            var myMaNhanVien = User.FindFirst("MaNhanVien")?.Value;
            if (string.IsNullOrEmpty(myMaNhanVien)) return Unauthorized();

            var cc = await _context.ChamCongs.FirstOrDefaultAsync(c => c.MaLich == maLich);
            if (cc == null || cc.ThoiGianCheckout != null)
            {
                TempData["Error"] = "Không tìm thấy phiên check-in hoặc bạn đã check-out.";
                return RedirectToAction(nameof(Index));
            }

            cc.ThoiGianCheckout = DateTime.Now;
            
            var lich = await _context.LichPhanCongs.FindAsync(maLich);
            if (lich != null)
            {
                lich.TrangThai = "Hoàn thành";
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Check-out thành công lúc " + DateTime.Now.ToString("HH:mm");
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Create()
        {
            ViewBag.LichPhanCongs = await _context.LichPhanCongs
                .Where(l => l.TrangThai != "Hoàn thành")
                .ToListAsync();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Create(ChamCong chamCong)
        {
            chamCong.MaChamCong = "CC" + DateTime.Now.Ticks.ToString().Substring(10, 4);
            
            if (chamCong.ThoiGianChekin != null)
                chamCong.TrangThai = "Đúng giờ";
            else
                chamCong.TrangThai = "Vắng mặt";
            
            if (ModelState.IsValid)
            {
                _context.ChamCongs.Add(chamCong);
                
                var lich = await _context.LichPhanCongs.FindAsync(chamCong.MaLich);
                if (lich != null)
                {
                    lich.TrangThai = chamCong.ThoiGianCheckout != null ? "Hoàn thành" : "Đang làm";
                }
                
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm chấm công thủ công thành công.";
                return RedirectToAction(nameof(Index));
            }
            
            ViewBag.LichPhanCongs = await _context.LichPhanCongs
                .Where(l => l.TrangThai != "Hoàn thành")
                .ToListAsync();
            return View(chamCong);
        }
    }
}