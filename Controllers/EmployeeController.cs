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

        public async Task<IActionResult> Index(string? phongBan = null, string? search = null)
        {
            var query = _context.NhanViens.AsQueryable();

            if (!string.IsNullOrEmpty(phongBan))
            {
                query = query.Where(n => n.MaPhongBan == phongBan);
            }

            if (!string.IsNullOrEmpty(search))
            {
                string kw = search.Trim().ToLower();
                query = query.Where(n =>
                    n.HoTen.ToLower().Contains(kw) ||
                    n.MaNhanVien.ToLower().Contains(kw) ||
                    (n.Email != null && n.Email.ToLower().Contains(kw)) ||
                    (n.SDT != null && n.SDT.Contains(kw)));
            }

            var employees = await query.OrderBy(n => n.MaNhanVien).ToListAsync();

            ViewBag.PhongBans = await _context.PhongBans.ToDictionaryAsync(p => p.MaPhongBan, p => p.TenPhongBan);
            ViewBag.ChucVus = await _context.ChucVus.ToDictionaryAsync(c => c.MaChucVu, c => c.TenChucVu);
            ViewBag.SelectedPhongBan = phongBan;
            ViewBag.Search = search;

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
            var existingIds = await _context.NhanViens.Select(n => n.MaNhanVien).ToListAsync();
            int nextNum = 1;
            while (existingIds.Contains($"NV{nextNum:D3}"))
            {
                nextNum++;
            }
            nhanVien.MaNhanVien = $"NV{nextNum:D3}";
            ModelState.Remove("MaNhanVien");

            if (ModelState.IsValid)
            {
                _context.NhanViens.Add(nhanVien);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Đã thêm nhân viên {nhanVien.HoTen} ({nhanVien.MaNhanVien}) thành công.";
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
                TempData["Success"] = $"Đã cập nhật thông tin nhân viên {nhanVien.HoTen}.";
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
                // Gỡ liên kết tài khoản
                var taiKhoans = await _context.TaiKhoans.Where(t => t.MaNhanVien == id).ToListAsync();
                foreach (var tk in taiKhoans)
                {
                    tk.MaNhanVien = null;
                }

                // Xóa các yêu cầu đổi ca, đơn xin nghỉ, chấm công, lịch phân công liên quan để tránh lỗi khóa ngoại
                var lichIds = await _context.LichPhanCongs.Where(l => l.MaNhanVien == id).Select(l => l.MaLich).ToListAsync();
                var doiCas = await _context.YeuCauDoiCas.Where(y => y.MaNhanVien == id || lichIds.Contains(y.MaLichCu) || lichIds.Contains(y.MaLichMoi)).ToListAsync();
                _context.YeuCauDoiCas.RemoveRange(doiCas);

                var chamCongs = await _context.ChamCongs.Where(c => lichIds.Contains(c.MaLich)).ToListAsync();
                _context.ChamCongs.RemoveRange(chamCongs);

                var lichs = await _context.LichPhanCongs.Where(l => l.MaNhanVien == id).ToListAsync();
                _context.LichPhanCongs.RemoveRange(lichs);

                var dons = await _context.DonXinNghis.Where(d => d.MaNhanVien == id).ToListAsync();
                _context.DonXinNghis.RemoveRange(dons);

                _context.NhanViens.Remove(nhanVien);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Đã xóa nhân viên {nhanVien.HoTen} ({id}).";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
