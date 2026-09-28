using Microsoft.AspNetCore.Mvc;
using HTQLNS.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace HTQLNS.Controllers
{
    public class LeaveViewModel
    {
        public string MaDon { get; set; } = string.Empty;
        public string NhanVienTen { get; set; } = string.Empty;
        public string PhongBanTen { get; set; } = string.Empty;
        public string AvatarChar { get; set; } = string.Empty;
        public string AvatarColor { get; set; } = string.Empty;
        
        public string LoaiDon { get; set; } = string.Empty; // Nghỉ phép, Đổi ca
        public string ThoiGianText { get; set; } = string.Empty;
        public string LyDo { get; set; } = string.Empty;
        public string GuiNgayText { get; set; } = string.Empty;
        public string TrangThai { get; set; } = string.Empty;
    }

    public class LeaveController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LeaveController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var nhanViens = await _context.NhanViens.ToListAsync();
            var phongBans = await _context.PhongBans.ToDictionaryAsync(p => p.MaPhongBan, p => p.TenPhongBan);
            
            var donXinNghis = await _context.DonXinNghis.ToListAsync();
            var yeuCauDoiCas = await _context.YeuCauDoiCas.ToListAsync();
            var lichPhanCongs = await _context.LichPhanCongs.ToListAsync();
            var loaiCas = await _context.LoaiCaLams.ToDictionaryAsync(c => c.MaLoaiCa, c => c.TenCa);
            
            var viewModels = new List<LeaveViewModel>();
            var colors = new[] { "#a881e6", "#1bb99a", "#f59e0b", "#f34343" };
            int colorIndex = 0;
            
            // Map DonXinNghi
            foreach (var don in donXinNghis)
            {
                var nv = nhanViens.FirstOrDefault(n => n.MaNhanVien == don.MaNhanVien);
                if (nv != null)
                {
                    viewModels.Add(new LeaveViewModel
                    {
                        MaDon = don.MaDon,
                        NhanVienTen = nv.HoTen,
                        PhongBanTen = phongBans.ContainsKey(nv.MaPhongBan) ? phongBans[nv.MaPhongBan] : "N/A",
                        AvatarChar = string.IsNullOrEmpty(nv.HoTen) ? "U" : nv.HoTen.Substring(nv.HoTen.LastIndexOf(' ') + 1, 1).ToUpper(),
                        AvatarColor = colors[(colorIndex++) % colors.Length],
                        LoaiDon = "Nghỉ phép",
                        ThoiGianText = $"{don.TuNgay:dd/MM}<br>→<br>{don.DenNgay:dd/MM/yyyy}",
                        LyDo = don.LyDo ?? "",
                        GuiNgayText = DateTime.Today.ToString("dd/MM/yyyy"), // Mock GuiNgay
                        TrangThai = don.TrangThai ?? "Chờ duyệt"
                    });
                }
            }
            
            // Map YeuCauDoiCa
            foreach (var dc in yeuCauDoiCas)
            {
                var nv = nhanViens.FirstOrDefault(n => n.MaNhanVien == dc.MaNhanVien);
                if (nv != null)
                {
                    var lichCu = lichPhanCongs.FirstOrDefault(l => l.MaLich == dc.MaLichCu);
                    var lichMoi = lichPhanCongs.FirstOrDefault(l => l.MaLich == dc.MaLichMoi);
                    
                    string tenCaCu = lichCu != null && loaiCas.ContainsKey(lichCu.MaLoaiCa) ? loaiCas[lichCu.MaLoaiCa] : "N/A";
                    string tenCaMoi = lichMoi != null && loaiCas.ContainsKey(lichMoi.MaLoaiCa) ? loaiCas[lichMoi.MaLoaiCa] : "N/A";
                    string ngayCu = lichCu?.NgayLam.ToString("dd/MM") ?? "";
                    string ngayMoi = lichMoi?.NgayLam.ToString("dd/MM") ?? "";
                    
                    viewModels.Add(new LeaveViewModel
                    {
                        MaDon = dc.MaYeuCau,
                        NhanVienTen = nv.HoTen,
                        PhongBanTen = phongBans.ContainsKey(nv.MaPhongBan) ? phongBans[nv.MaPhongBan] : "N/A",
                        AvatarChar = string.IsNullOrEmpty(nv.HoTen) ? "U" : nv.HoTen.Substring(nv.HoTen.LastIndexOf(' ') + 1, 1).ToUpper(),
                        AvatarColor = colors[(colorIndex++) % colors.Length],
                        LoaiDon = "Đổi ca",
                        ThoiGianText = $"{tenCaCu} {ngayCu}<br>→ {tenCaMoi}<br>{ngayMoi}",
                        LyDo = dc.LyDo ?? "",
                        GuiNgayText = dc.NgayGui.ToString("dd/MM/yyyy"),
                        TrangThai = dc.TrangThai ?? "Chờ duyệt"
                    });
                }
            }

            return View(viewModels.OrderBy(v => v.MaDon).ToList());
        }

        [HttpPost]
        public async Task<IActionResult> Approve(string id, string type)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(type)) return NotFound();

            if (type == "Nghỉ phép")
            {
                var don = await _context.DonXinNghis.FindAsync(id);
                if (don != null)
                {
                    don.TrangThai = "Đã duyệt";
                    await _context.SaveChangesAsync();
                }
            }
            else if (type == "Đổi ca")
            {
                var yeuCau = await _context.YeuCauDoiCas.FindAsync(id);
                if (yeuCau != null)
                {
                    yeuCau.TrangThai = "Đã duyệt";
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.NhanViens = await _context.NhanViens.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(DonXinNghi donXinNghi)
        {
            donXinNghi.MaDon = "DN" + DateTime.Now.Ticks.ToString().Substring(10, 4);
            donXinNghi.TrangThai = "Chờ duyệt";
            
            if (ModelState.IsValid)
            {
                _context.DonXinNghis.Add(donXinNghi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            ViewBag.NhanViens = await _context.NhanViens.ToListAsync();
            return View(donXinNghi);
        }
    }
}
