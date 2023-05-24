using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using monitoring.Service;
using monitoring.Models.ViewModel;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace monitoring.Controllers
{
    public class AccountController : Controller
    {
        public AccountController(MonitoringSystemContext context)
        {
            _context = context;
        }
        private readonly MonitoringSystemContext _context;
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.email && u.Password == HashPasswordHelper.HashPassword(model.password));
                if (user != null)
                {
                    await Authenticate($"{user.LastName} {user.FirstName}"); // аутентификация
                    Response.Cookies.Append("UserId", $"{user.UserId}");
                    Response.Cookies.Append("RoleId", $"{user.RoleId}");
                    Response.Cookies.Append("NgduId", $"{user.NgduId}");
                    

                    if ($"{user.RoleId}" == "1")
                    {
                        Response.Cookies.Append("UserRole", "Администратор");
                    }
                    else
                    {
                        Response.Cookies.Append("UserRole", "Пользователь");
                    }

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Некорректные почта и(или) пароль");
            }
            return View(model);
        }

        private async Task Authenticate(string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };

            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}