using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HTQLNS.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HTQLNS.Controllers
{
    [Authorize(Roles = "Admin,HR,Manager")]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var employees = await _context.NhanViens
                .ToListAsync();
                
            // Load names mapping for view purposes temporarily
            ViewBag.PhongBans = await _context.PhongBans.ToDictionaryAsync(p => p.MaPhongBan, p => p.TenPhongBan);
            ViewBag.ChucVus = await _context.ChucVus.ToDictionaryAsync(c => c.MaChucVu, c => c.TenChucVu);
                
            return View(employees);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.PhongBans = await _context.PhongBans.ToListAsync();
            ViewBag.ChucVus = await _context.ChucVus.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(NhanVien nhanVien)
        {
            nhanVien.MaNhanVien = "NV" + DateTime.Now.Ticks.ToString().Substring(10, 4); // Basic ID generation
            
            if (ModelState.IsValid)
            {
                _context.NhanViens.Add(nhanVien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            ViewBag.PhongBans = await _context.PhongBans.ToListAsync();
            ViewBag.ChucVus = await _context.ChucVus.ToListAsync();
            return View(nhanVien);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var nhanVien = await _context.NhanViens.FindAsync(id);
            if (nhanVien == null) return NotFound();

            ViewBag.PhongBans = await _context.PhongBans.ToListAsync();
            ViewBag.ChucVus = await _context.ChucVus.ToListAsync();
            return View(nhanVien);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, NhanVien nhanVien)
        {
            if (id != nhanVien.MaNhanVien) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(nhanVien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.PhongBans = await _context.PhongBans.ToListAsync();
            ViewBag.ChucVus = await _context.ChucVus.ToListAsync();
            return View(nhanVien);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var nhanVien = await _context.NhanViens.FindAsync(id);
            if (nhanVien != null)
            {
                _context.NhanViens.Remove(nhanVien);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
