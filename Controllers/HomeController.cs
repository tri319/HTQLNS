using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using HTQLNS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HTQLNS.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var totalEmployees = await _context.NhanViens.CountAsync();
            var totalPresent = await _context.ChamCongs.CountAsync(c => c.TrangThai == "Đúng giờ" || c.TrangThai == "Trễ giờ");
            var lateOrAbsent = await _context.ChamCongs.CountAsync(c => c.TrangThai == "Trễ giờ" || c.TrangThai == "Vắng mặt");
            var pendingLeaves = await _context.DonXinNghis.CountAsync(d => d.TrangThai == "Chờ duyệt");
            var pendingSwaps = await _context.YeuCauDoiCas.CountAsync(y => y.TrangThai == "Chờ duyệt");

            var phongBans = await _context.PhongBans.ToListAsync();
            var nhanViens = await _context.NhanViens.ToListAsync();

            var deptStats = phongBans
                .Select(pb => new
                {
                    TenPhongBan = pb.TenPhongBan,
                    SoLuong = nhanViens.Count(n => n.MaPhongBan == pb.MaPhongBan)
                })
                .OrderByDescending(d => d.SoLuong)
                .ToList();

            ViewBag.TotalEmployees = totalEmployees;
            ViewBag.TotalPresent = totalPresent;
            ViewBag.LateOrAbsent = lateOrAbsent;
            ViewBag.PendingTotal = pendingLeaves + pendingSwaps;
            ViewBag.PendingLeaves = pendingLeaves;
            ViewBag.PendingSwaps = pendingSwaps;
            ViewBag.DeptStats = deptStats;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
