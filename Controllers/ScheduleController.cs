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
    public class ScheduleCellModel
    {
        public List<string> NhanVienNames { get; set; } = new List<string>();
        public bool IsCurrentDay { get; set; }
    }

    public class ScheduleRowModel
    {
        public string TenCa { get; set; } = string.Empty;
        public string ThoiGian { get; set; } = string.Empty;
        public string ColorClass { get; set; } = string.Empty;
        public Dictionary<DayOfWeek, ScheduleCellModel> Days { get; set; } = new Dictionary<DayOfWeek, ScheduleCellModel>();
    }

    public class ScheduleListItem
    {
        public string MaLich { get; set; } = string.Empty;
        public string MaNhanVien { get; set; } = string.Empty;
        public string TenNhanVien { get; set; } = string.Empty;
        public string TenCa { get; set; } = string.Empty;
        public string ThoiGian { get; set; } = string.Empty;
        public DateTime NgayLam { get; set; }
        public string TrangThai { get; set; } = string.Empty;
    }

    [Authorize]
    public class ScheduleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ScheduleController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int weekOffset = 0)
        {
            DateTime today = DateTime.Today;
            int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
            DateTime startOfWeek = today.AddDays(-1 * diff).AddDays(weekOffset * 7);
            DateTime endOfWeek = startOfWeek.AddDays(7);

            var allLichs = await _context.LichPhanCongs.OrderByDescending(l => l.NgayLam).ToListAsync();
            var weekLichs = allLichs.Where(l => l.NgayLam.Date >= startOfWeek && l.NgayLam.Date < endOfWeek).ToList();
            if (!weekLichs.Any() && weekOffset == 0)
            {
                weekLichs = allLichs;
            }

            var nhanViens = await _context.NhanViens.ToDictionaryAsync(n => n.MaNhanVien, n => n.HoTen);
            var loaiCas = await _context.LoaiCaLams.OrderBy(c => c.GioBatDau).ToListAsync();
            var caDict = loaiCas.ToDictionary(c => c.MaLoaiCa);

            var colors = new[] { "accent-orange", "accent-purple", "accent-green" };
            var rows = new List<ScheduleRowModel>();

            for (int i = 0; i < loaiCas.Count; i++)
            {
                var ca = loaiCas[i];
                var row = new ScheduleRowModel
                {
                    TenCa = ca.TenCa,
                    ThoiGian = $"{ca.GioBatDau:hh\\:mm} - {ca.GioKetThuc:hh\\:mm}",
                    ColorClass = colors[i % colors.Length]
                };

                foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
                {
                    row.Days[day] = new ScheduleCellModel();
                }

                var caLichs = weekLichs.Where(l => l.MaLoaiCa == ca.MaLoaiCa).ToList();
                foreach (var lich in caLichs)
                {
                    if (nhanViens.TryGetValue(lich.MaNhanVien, out var tenNV))
                    {
                        var dayOfWeek = lich.NgayLam.DayOfWeek;
                        if (!row.Days[dayOfWeek].NhanVienNames.Contains(tenNV))
                        {
                            row.Days[dayOfWeek].NhanVienNames.Add(tenNV);
                        }
                    }
                }

                rows.Add(row);
            }

            var listItems = weekLichs
                .OrderBy(l => l.NgayLam)
                .Select(l => new ScheduleListItem
                {
                    MaLich = l.MaLich,
                    MaNhanVien = l.MaNhanVien,
                    TenNhanVien = nhanViens.ContainsKey(l.MaNhanVien) ? nhanViens[l.MaNhanVien] : l.MaNhanVien,
                    TenCa = caDict.ContainsKey(l.MaLoaiCa) ? caDict[l.MaLoaiCa].TenCa : l.MaLoaiCa,
                    ThoiGian = caDict.ContainsKey(l.MaLoaiCa) ? $"{caDict[l.MaLoaiCa].GioBatDau:hh\\:mm} - {caDict[l.MaLoaiCa].GioKetThuc:hh\\:mm}" : "",
                    NgayLam = l.NgayLam,
                    TrangThai = l.TrangThai ?? "Sắp tới"
                })
                .ToList();

            ViewBag.StartOfWeek = startOfWeek;
            ViewBag.WeekOffset = weekOffset;
            ViewBag.ScheduleList = listItems;
            ViewBag.MyName = User.FindFirst("HoTen")?.Value ?? "";

            return View(rows);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            ViewBag.NhanViens = await _context.NhanViens.ToListAsync();
            ViewBag.LoaiCas = await _context.LoaiCaLams.ToListAsync();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(LichPhanCong lichPhanCong)
        {
            lichPhanCong.MaLich = "LC" + DateTime.Now.Ticks.ToString().Substring(10, 4);
            lichPhanCong.TrangThai = "Sắp tới";
            ModelState.Remove("MaLich");

            if (ModelState.IsValid)
            {
                _context.LichPhanCongs.Add(lichPhanCong);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã phân công ca làm việc mới thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.NhanViens = await _context.NhanViens.ToListAsync();
            ViewBag.LoaiCas = await _context.LoaiCaLams.ToListAsync();
            return View(lichPhanCong);
        }
    }
}
