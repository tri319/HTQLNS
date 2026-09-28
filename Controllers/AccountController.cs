using Microsoft.AspNetCore.Mvc;

namespace HTQLNS.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
