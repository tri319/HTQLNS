using Microsoft.AspNetCore.Mvc;
using HTQLNS.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace HTQLNS.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var totalNhanVien = await _context.NhanViens.CountAsync();
            var totalDonXinNghi = await _context.DonXinNghis.CountAsync(d => d.TrangThai == "Đã duyệt");
            var pendingDon = await _context.DonXinNghis.CountAsync(d => d.TrangThai == "Chờ duyệt");
            
            var chamCongs = await _context.ChamCongs.ToListAsync();
            int dungGioCount = chamCongs.Count(c => c.TrangThai == "Đúng giờ");
            int tongChamCong = chamCongs.Count;
            
            double attendanceRate = tongChamCong > 0 ? (double)dungGioCount / tongChamCong * 100 : 0;
            
            // Mock OT for now
            int totalOT = 87; 
            
            ViewBag.AttendanceRate = attendanceRate.ToString("F1");
            ViewBag.TotalOT = totalOT;
            ViewBag.ApprovedLeaves = totalDonXinNghi;
            ViewBag.PendingLeaves = pendingDon;
            ViewBag.TotalEmployees = totalNhanVien;
            
            return View();
        }
    }
}
