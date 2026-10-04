using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using HTQLNS.Models;
using Microsoft.EntityFrameworkCore;

namespace HTQLNS.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
        {
            var user = await _context.TaiKhoans
                .Include(t => t.VaiTro)
                .Include(t => t.NhanVien)
                .FirstOrDefaultAsync(u => u.TenDangNhap == username && u.MatKhau == password);

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.MaTaiKhoan),
                    new Claim(ClaimTypes.Name, user.TenDangNhap),
                    new Claim("MaNhanVien", user.MaNhanVien),
                    new Claim(ClaimTypes.Role, user.VaiTro?.TenVaiTro ?? "User")
                };
                
                if (user.NhanVien != null)
                {
                    claims.Add(new Claim("HoTen", user.NhanVien.HoTen));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng!";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string username, string password, string confirmPassword, string hoTen, string email)
        {
            if (password != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp!";
                return View();
            }

            var existingUser = await _context.TaiKhoans.FirstOrDefaultAsync(u => u.TenDangNhap == username);
            if (existingUser != null)
            {
                ViewBag.Error = "Tên đăng nhập đã tồn tại!";
                return View();
            }

            // Create a default role if not exists
            var userRole = await _context.VaiTros.FirstOrDefaultAsync(r => r.TenVaiTro == "User");
            if (userRole == null)
            {
                userRole = new VaiTro { TenVaiTro = "User" };
                _context.VaiTros.Add(userRole);
                await _context.SaveChangesAsync();
            }

            // Create employee record
            var newEmployee = new NhanVien
            {
                MaNhanVien = "NV" + DateTime.Now.Ticks.ToString().Substring(10, 4),
                HoTen = hoTen,
                Email = email,
                MaPhongBan = "PB01", // Default department, should be adjusted based on logic
                MaChucVu = "CV01" // Default position
            };
            
            // Check if default dept and position exist to prevent FK errors (simplified for brevity)
            var dept = await _context.PhongBans.FirstOrDefaultAsync();
            if (dept != null) newEmployee.MaPhongBan = dept.MaPhongBan;
            
            var pos = await _context.ChucVus.FirstOrDefaultAsync();
            if (pos != null) newEmployee.MaChucVu = pos.MaChucVu;

            _context.NhanViens.Add(newEmployee);
            await _context.SaveChangesAsync();

            // Create account
            var newAccount = new TaiKhoan
            {
                TenDangNhap = username,
                MatKhau = password,
                TrangThai = "Active",
                MaNhanVien = newEmployee.MaNhanVien,
                MaVaiTro = userRole.MaVaiTro
            };

            _context.TaiKhoans.Add(newAccount);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
            return RedirectToAction("Login");
        }
        
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
