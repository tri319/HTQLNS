using Microsoft.AspNetCore.Mvc;
using HTQLNS.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace HTQLNS.Controllers
{
    public class AttendanceViewModel
    {
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
    }

    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var chamCongs = await _context.ChamCongs.ToListAsync();
            var lichPhanCongs = await _context.LichPhanCongs.ToDictionaryAsync(l => l.MaLich);
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
                            MaNV = nv.MaNhanVien,
                            NhanVienTen = nv.HoTen,
                            PhongBanTen = phongBans.ContainsKey(nv.MaPhongBan) ? phongBans[nv.MaPhongBan] : "N/A",
                            AvatarChar = string.IsNullOrEmpty(nv.HoTen) ? "U" : nv.HoTen.Substring(nv.HoTen.LastIndexOf(' ') + 1, 1).ToUpper(),
                            AvatarColor = colors[(colorIndex++) % colors.Length],
                            CaLam = loaiCas.ContainsKey(lich.MaLoaiCa) ? loaiCas[lich.MaLoaiCa] : "N/A",
                            GioVao = cc.ThoiGianChekin?.ToString("HH:mm") ?? "—",
                            GioRa = cc.ThoiGianCheckout?.ToString("HH:mm") ?? "—",
                            OT = "—", // Mock OT logic could go here
                            TrangThai = cc.TrangThai ?? "Chưa rõ"
                        });
                    }
                }
            }

            return View(viewModels.OrderBy(v => v.MaNV).ToList());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.LichPhanCongs = await _context.LichPhanCongs
                .Where(l => l.TrangThai != "Hoàn thành")
                .ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ChamCong chamCong)
        {
            chamCong.MaChamCong = "CC" + DateTime.Now.Ticks.ToString().Substring(10, 4);
            
            // Determine status logic simply based on checkin time presence
            if (chamCong.ThoiGianChekin != null)
            {
                chamCong.TrangThai = "Đúng giờ"; // Simplified logic
            }
            else
            {
                chamCong.TrangThai = "Vắng mặt";
            }
            
            if (ModelState.IsValid)
            {
                _context.ChamCongs.Add(chamCong);
                
                // Update schedule status
                var lich = await _context.LichPhanCongs.FindAsync(chamCong.MaLich);
                if (lich != null)
                {
                    lich.TrangThai = chamCong.ThoiGianCheckout != null ? "Hoàn thành" : "Đang làm";
                }
                
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            ViewBag.LichPhanCongs = await _context.LichPhanCongs
                .Where(l => l.TrangThai != "Hoàn thành")
                .ToListAsync();
            return View(chamCong);
        }
    }
}
