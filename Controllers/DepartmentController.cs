using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HTQLNS.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace HTQLNS.Controllers
{
    public class DepartmentViewModel
    {
        public string MaPhongBan { get; set; } = string.Empty;
        public string TenPhongBan { get; set; } = string.Empty;
        public int SoLuongNhanVien { get; set; }
        public string TruongPhong { get; set; } = string.Empty;
        public List<string> ChucVus { get; set; } = new List<string>();
        public List<NhanVien> NhanViens { get; set; } = new List<NhanVien>();
        public string ColorClass { get; set; } = "accent-purple";
    }

    [Authorize(Roles = "Admin,HR")]
    public class DepartmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DepartmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var phongBans = await _context.PhongBans.ToListAsync();
            var nhanViens = await _context.NhanViens.ToListAsync();
            var allChucVus = await _context.ChucVus.ToListAsync();
            var chucVus = allChucVus.ToDictionary(c => c.MaChucVu, c => c.TenChucVu);

            var colors = new[] { "accent-purple", "accent-red", "accent-green", "accent-orange" };

            var viewModels = phongBans.Select((pb, index) =>
            {
                var nvPhongBan = nhanViens.Where(nv => nv.MaPhongBan == pb.MaPhongBan).ToList();

                var rolesInDept = nvPhongBan
                    .Select(nv => nv.MaChucVu)
                    .Distinct()
                    .Select(ma => chucVus.ContainsKey(ma) ? chucVus[ma] : "")
                    .Where(t => !string.IsNullOrEmpty(t))
                    .ToList();

                var truongPhong = nvPhongBan.FirstOrDefault(nv => chucVus.ContainsKey(nv.MaChucVu) && chucVus[nv.MaChucVu].Contains("Quản lý"));
                string truongPhongName = truongPhong?.HoTen ?? (nvPhongBan.FirstOrDefault()?.HoTen ?? "Chưa bổ nhiệm");

                return new DepartmentViewModel
                {
                    MaPhongBan = pb.MaPhongBan,
                    TenPhongBan = pb.TenPhongBan,
                    SoLuongNhanVien = nvPhongBan.Count,
                    TruongPhong = truongPhongName,
                    ChucVus = rolesInDept,
                    NhanViens = nvPhongBan,
                    ColorClass = colors[index % colors.Length]
                };
            }).ToList();

            ViewBag.AllChucVus = allChucVus;
            ViewBag.ChucVuDict = chucVus;
            ViewBag.AllNhanViens = nhanViens;
            return View(viewModels);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(PhongBan phongBan)
        {
            var existingIds = await _context.PhongBans.Select(p => p.MaPhongBan).ToListAsync();
            int nextNum = 1;
            while (existingIds.Contains($"PB{nextNum:D3}"))
            {
                nextNum++;
            }
            phongBan.MaPhongBan = $"PB{nextNum:D3}";
            ModelState.Remove("MaPhongBan");

            if (ModelState.IsValid)
            {
                _context.PhongBans.Add(phongBan);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Đã thêm phòng ban '{phongBan.TenPhongBan}' ({phongBan.MaPhongBan}).";
                return RedirectToAction(nameof(Index));
            }

            return View(phongBan);
        }

        [HttpPost]
        public async Task<IActionResult> EditDepartment(string maPhongBan, string tenPhongBan)
        {
            var pb = await _context.PhongBans.FindAsync(maPhongBan);
            if (pb != null && !string.IsNullOrWhiteSpace(tenPhongBan))
            {
                pb.TenPhongBan = tenPhongBan.Trim();
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Đã cập nhật tên phòng ban thành '{pb.TenPhongBan}'.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> CreatePosition(string tenChucVu, string? moTa)
        {
            if (!string.IsNullOrWhiteSpace(tenChucVu))
            {
                var existingIds = await _context.ChucVus.Select(c => c.MaChucVu).ToListAsync();
                int nextNum = 1;
                while (existingIds.Contains($"CV{nextNum:D3}"))
                {
                    nextNum++;
                }

                var cv = new ChucVu
                {
                    MaChucVu = $"CV{nextNum:D3}",
                    TenChucVu = tenChucVu.Trim(),
                    MoTa = moTa?.Trim()
                };
                _context.ChucVus.Add(cv);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Đã thêm chức vụ '{cv.TenChucVu}' ({cv.MaChucVu}).";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
