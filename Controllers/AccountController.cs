using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using HTQLNS.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
namespace HTQLNS.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AccountController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ tài khoản và mật khẩu.";
                return View();
            }

            var user = await _context.TaiKhoans
                .Include(t => t.NhanVien)
                .Include(t => t.VaiTro)
                .FirstOrDefaultAsync(u => u.TenDangNhap.ToLower() == username.Trim().ToLower() && u.MatKhau == password);

            if (user == null)
            {
                ViewBag.Error = "Tài khoản hoặc mật khẩu không chính xác.";
                return View();
            }

            if (user.TrangThai == "Chờ duyệt")
            {
                ViewBag.Error = "Tài khoản của bạn đang chờ quản trị viên phê duyệt. Vui lòng quay lại sau!";
                return View();
            }

            if (user.TrangThai != "Hoạt động")
            {
                ViewBag.Error = "Tài khoản của bạn đã bị khóa hoặc không hợp lệ.";
                return View();
            }

            return await SignInUserAsync(user, returnUrl);
        }

        [HttpPost]
        public async Task<IActionResult> QuickLogin(string role, string? returnUrl = null)
        {
            // Đảm bảo dữ liệu mẫu đã được khởi tạo
            DbSeeder.Seed(_context);

            string targetUsername = role == "Admin" ? "admin" : "nhanvien";
            var user = await _context.TaiKhoans
                .Include(t => t.NhanVien)
                .Include(t => t.VaiTro)
                .FirstOrDefaultAsync(u => u.TenDangNhap == targetUsername);

            if (user == null)
            {
                ViewBag.Error = "Không tìm thấy tài khoản demo.";
                return View("Login");
            }

            return await SignInUserAsync(user, returnUrl);
        }

        private async Task<IActionResult> SignInUserAsync(TaiKhoan user, string? returnUrl = null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.MaTaiKhoan),
                new Claim(ClaimTypes.Name, user.NhanVien?.HoTen ?? user.HoTen ?? user.TenDangNhap),
                new Claim("MaNhanVien", user.MaNhanVien ?? ""),
                new Claim(ClaimTypes.Email, user.NhanVien?.Email ?? user.TenDangNhap),
                new Claim(ClaimTypes.Role, user.VaiTro?.TenVaiTro ?? "Nhân viên"),
                new Claim("HoTen", user.NhanVien?.HoTen ?? user.HoTen ?? "")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { IsPersistent = true };

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

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
        
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ExternalLogin(string provider, string? returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, provider);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            if (remoteError != null)
            {
                ViewBag.Error = $"Lỗi từ dịch vụ bên ngoài: {remoteError}";
                return View("Login");
            }

            var info = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!info.Succeeded)
            {
                info = await HttpContext.AuthenticateAsync("Google");
            }

            if (info?.Principal == null)
            {
                return RedirectToAction("Login");
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var name = info.Principal.FindFirstValue(ClaimTypes.Name);
            
            if (string.IsNullOrEmpty(email))
            {
                ViewBag.Error = "Không thể lấy thông tin email từ Google.";
                return View("Login");
            }

            var user = await _context.TaiKhoans
                .Include(t => t.NhanVien)
                .Include(t => t.VaiTro)
                .FirstOrDefaultAsync(u => u.TenDangNhap == email);

            const string AdminEmail = "trin99832@gmail.com";

            if (user == null)
            {
                if (email.ToLower() == AdminEmail.ToLower())
                {
                    // --- Admin tự động: tạo và đăng nhập luôn ---
                    var adminRole = await _context.VaiTros.FirstOrDefaultAsync(r => r.TenVaiTro == "Admin");
                    if (adminRole == null)
                    {
                        adminRole = new VaiTro { MaVaiTro = "VT001", TenVaiTro = "Admin" };
                        _context.VaiTros.Add(adminRole);
                        await _context.SaveChangesAsync();
                    }

                    var dept = await _context.PhongBans.FirstOrDefaultAsync();
                    var pos  = await _context.ChucVus.FirstOrDefaultAsync(c => c.TenChucVu == "Quản lý")
                               ?? await _context.ChucVus.FirstOrDefaultAsync();

                    var adminNv = await _context.NhanViens.FirstOrDefaultAsync(n => n.Email == email);
                    if (adminNv == null)
                    {
                        adminNv = new NhanVien
                        {
                            MaNhanVien = "NV001",
                            HoTen      = name ?? "Admin StarZone",
                            Email      = email,
                            MaPhongBan = dept?.MaPhongBan ?? "PB001",
                            MaChucVu   = pos?.MaChucVu ?? "CV003"
                        };
                        _context.NhanViens.Add(adminNv);
                        await _context.SaveChangesAsync();
                    }

                    user = new TaiKhoan
                    {
                        MaTaiKhoan  = "TK001",
                        TenDangNhap = email,
                        HoTen       = name ?? "Admin StarZone",
                        MatKhau     = null,
                        TrangThai   = "Hoạt động",
                        MaNhanVien  = adminNv.MaNhanVien,
                        MaVaiTro    = adminRole.MaVaiTro,
                        VaiTro      = adminRole,
                        NhanVien    = adminNv
                    };
                    _context.TaiKhoans.Add(user);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    // Người dùng mới -> bổ sung thông tin
                    TempData["GoogleEmail"] = email;
                    TempData["GoogleName"]  = name;
                    return RedirectToAction("CompleteProfile");
                }
            }

            if (user.TrangThai == "Chờ duyệt")
            {
                ViewBag.Error = "Tài khoản của bạn đang chờ quản trị viên phê duyệt. Vui lòng quay lại sau!";
                return View("Login");
            }
            
            if (user.TrangThai != "Hoạt động")
            {
                ViewBag.Error = "Tài khoản của bạn đã bị khóa hoặc không hợp lệ.";
                return View("Login");
            }

            // Đăng nhập thành công
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.MaTaiKhoan),
                new Claim(ClaimTypes.Name, user.NhanVien?.HoTen ?? user.HoTen ?? user.TenDangNhap),
                new Claim("MaNhanVien", user.MaNhanVien ?? ""),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, user.VaiTro?.TenVaiTro ?? "User"),
                new Claim("HoTen", user.NhanVien?.HoTen ?? user.HoTen ?? "")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { IsPersistent = true };

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

        [HttpGet]
        public IActionResult CompleteProfile()
        {
            if (TempData["GoogleEmail"] == null)
            {
                return RedirectToAction("Login");
            }
            
            ViewBag.Email = TempData["GoogleEmail"];
            ViewBag.Name = TempData["GoogleName"];
            
            // Re-assign TempData so it persists for POST in case of validation error
            TempData.Keep("GoogleEmail");
            TempData.Keep("GoogleName");
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CompleteProfile(string hoTen, DateTime ngaySinh, string matKhau)
        {
            var email = TempData["GoogleEmail"]?.ToString();
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login");
            }

            // Tạo tài khoản mới với trạng thái Chờ duyệt
            var newAccount = new TaiKhoan
            {
                MaTaiKhoan = "TK" + DateTime.Now.Ticks.ToString().Substring(10, 4),
                TenDangNhap = email,
                HoTen = hoTen,
                NgaySinh = ngaySinh,
                MatKhau = matKhau,
                TrangThai = "Chờ duyệt"
            };

            // Có thể tự động gán vai trò User mặc định nếu cần
            var userRole = await _context.VaiTros.FirstOrDefaultAsync(r => r.TenVaiTro == "Nhân viên");
            if (userRole != null)
            {
                newAccount.MaVaiTro = userRole.MaVaiTro;
            }

            _context.TaiKhoans.Add(newAccount);
            await _context.SaveChangesAsync();

            TempData.Remove("GoogleEmail");
            TempData.Remove("GoogleName");

            TempData["Success"] = "Đăng ký thành công! Vui lòng chờ quản trị viên phê duyệt tài khoản và cấp mã nhân viên.";
            return RedirectToAction("Login");
        }
    }
}
