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
            var chucVus = await _context.ChucVus.ToDictionaryAsync(c => c.MaChucVu, c => c.TenChucVu);
            
            var colors = new[] { "accent-purple", "accent-red", "accent-green", "accent-orange" };
            
            var viewModels = phongBans.Select((pb, index) => {
                var nvPhongBan = nhanViens.Where(nv => nv.MaPhongBan == pb.MaPhongBan).ToList();
                
                // Get unique roles in this department
                var rolesInDept = nvPhongBan
                    .Select(nv => nv.MaChucVu)
                    .Distinct()
                    .Select(ma => chucVus.ContainsKey(ma) ? chucVus[ma] : "")
                    .Where(t => !string.IsNullOrEmpty(t))
                    .ToList();
                    
                // Mock a TruongPhong (manager)
                var truongPhong = nvPhongBan.FirstOrDefault(nv => chucVus.ContainsKey(nv.MaChucVu) && chucVus[nv.MaChucVu].Contains("Quản lý"));
                string truongPhongName = truongPhong?.HoTen ?? (nvPhongBan.FirstOrDefault()?.HoTen ?? "Chưa có");

                return new DepartmentViewModel
                {
                    MaPhongBan = pb.MaPhongBan,
                    TenPhongBan = pb.TenPhongBan,
                    SoLuongNhanVien = nvPhongBan.Count,
                    TruongPhong = truongPhongName,
                    ChucVus = rolesInDept,
                    ColorClass = colors[index % colors.Length]
                };
            }).ToList();

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
            phongBan.MaPhongBan = "PB" + DateTime.Now.Ticks.ToString().Substring(10, 3);
            
            if (ModelState.IsValid)
            {
                _context.PhongBans.Add(phongBan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            return View(phongBan);
        }
    }
}
