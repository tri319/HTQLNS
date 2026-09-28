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

    public class ScheduleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ScheduleController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var lichPhanCongs = await _context.LichPhanCongs.ToListAsync();
            var nhanViens = await _context.NhanViens.ToDictionaryAsync(n => n.MaNhanVien, n => n.HoTen);
            var loaiCas = await _context.LoaiCaLams.OrderBy(c => c.GioBatDau).ToListAsync();
            
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

                // Initialize 7 days
                foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
                {
                    row.Days[day] = new ScheduleCellModel();
                }

                // Map assignments to this shift
                var caLichs = lichPhanCongs.Where(l => l.MaLoaiCa == ca.MaLoaiCa).ToList();
                foreach (var lich in caLichs)
                {
                    if (nhanViens.TryGetValue(lich.MaNhanVien, out var tenNV))
                    {
                        var dayOfWeek = lich.NgayLam.DayOfWeek;
                        row.Days[dayOfWeek].NhanVienNames.Add(tenNV);
                    }
                }
                
                rows.Add(row);
            }

            return View(rows);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.NhanViens = await _context.NhanViens.ToListAsync();
            ViewBag.LoaiCas = await _context.LoaiCaLams.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(LichPhanCong lichPhanCong)
        {
            lichPhanCong.MaLich = "LC" + DateTime.Now.Ticks.ToString().Substring(10, 4);
            lichPhanCong.TrangThai = "Sắp tới";

            if (ModelState.IsValid)
            {
                _context.LichPhanCongs.Add(lichPhanCong);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.NhanViens = await _context.NhanViens.ToListAsync();
            ViewBag.LoaiCas = await _context.LoaiCaLams.ToListAsync();
            return View(lichPhanCong);
        }
    }
}
