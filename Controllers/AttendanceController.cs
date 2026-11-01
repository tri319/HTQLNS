using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HTQLNS.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Text;

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
                        string otText = "—";
                        if (cc.ThoiGianChekin.HasValue && cc.ThoiGianCheckout.HasValue)
                        {
                            var hours = (cc.ThoiGianCheckout.Value - cc.ThoiGianChekin.Value).TotalHours;
                            if (hours > 6.2)
                            {
                                otText = $"+{(hours - 6.0):F1}h";
                            }
                        }

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
                            OT = otText,
                            TrangThai = cc.TrangThai ?? "Chưa rõ",
                            MaLich = cc.MaLich
                        });
                    }
                }
            }

            ViewBag.IsAdmin = isAdmin;

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

            var newCC = new ChamCong
            {
                MaChamCong = "CC" + DateTime.Now.Ticks.ToString().Substring(10, 4),
                MaLich = maLich,
                ThoiGianChekin = DateTime.Now,
                TrangThai = "Đúng giờ"
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
                .OrderByDescending(l => l.NgayLam)
                .ToListAsync();
            ViewBag.NhanVienDict = await _context.NhanViens.ToDictionaryAsync(n => n.MaNhanVien, n => n.HoTen);
            ViewBag.LoaiCaDict = await _context.LoaiCaLams.ToDictionaryAsync(c => c.MaLoaiCa, c => c.TenCa);
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Create(ChamCong chamCong)
        {
            chamCong.MaChamCong = "CC" + DateTime.Now.Ticks.ToString().Substring(10, 4);
            ModelState.Remove("MaChamCong");

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
            ViewBag.NhanVienDict = await _context.NhanViens.ToDictionaryAsync(n => n.MaNhanVien, n => n.HoTen);
            ViewBag.LoaiCaDict = await _context.LoaiCaLams.ToDictionaryAsync(c => c.MaLoaiCa, c => c.TenCa);
            return View(chamCong);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> EditAttendance(string maChamCong, string trangThai)
        {
            var cc = await _context.ChamCongs.FindAsync(maChamCong);
            if (cc != null)
            {
                cc.TrangThai = trangThai;
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Đã cập nhật trạng thái chấm công ({maChamCong}) thành '{trangThai}'.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> ExportCsv()
        {
            var chamCongs = await _context.ChamCongs.ToListAsync();
            var lichs = await _context.LichPhanCongs.ToDictionaryAsync(l => l.MaLich);
            var nhanViens = await _context.NhanViens.ToDictionaryAsync(n => n.MaNhanVien);
            var phongBans = await _context.PhongBans.ToDictionaryAsync(p => p.MaPhongBan, p => p.TenPhongBan);
            var loaiCas = await _context.LoaiCaLams.ToDictionaryAsync(c => c.MaLoaiCa, c => c.TenCa);

            var sb = new StringBuilder();
            sb.AppendLine("Mã Chấm Công,Mã NV,Họ Tên,Phòng Ban,Ca Làm,Ngày Làm,Giờ Vào,Giờ Ra,Trạng Thái");

            foreach (var cc in chamCongs)
            {
                if (lichs.TryGetValue(cc.MaLich, out var lich) && nhanViens.TryGetValue(lich.MaNhanVien, out var nv))
                {
                    string pb = phongBans.ContainsKey(nv.MaPhongBan) ? phongBans[nv.MaPhongBan] : "";
                    string ca = loaiCas.ContainsKey(lich.MaLoaiCa) ? loaiCas[lich.MaLoaiCa] : "";
                    string gioVao = cc.ThoiGianChekin?.ToString("HH:mm") ?? "";
                    string gioRa = cc.ThoiGianCheckout?.ToString("HH:mm") ?? "";
                    sb.AppendLine($"{cc.MaChamCong},{nv.MaNhanVien},\"{nv.HoTen}\",\"{pb}\",\"{ca}\",{lich.NgayLam:dd/MM/yyyy},{gioVao},{gioRa},\"{cc.TrangThai}\"");
                }
            }

            var preamble = Encoding.UTF8.GetPreamble();
            var body = Encoding.UTF8.GetBytes(sb.ToString());
            var result = preamble.Concat(body).ToArray();

            return File(result, "text/csv; charset=utf-8", $"BangChamCong_StarZone_{DateTime.Now:yyyyMMdd}.csv");
        }
    }
}
