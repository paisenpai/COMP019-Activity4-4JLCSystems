using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using COMP019_Activity4_4JLCSystems.Data;
using COMP019_Activity4_4JLCSystems.Models.ViewModels;
using COMP019_Activity4_4JLCSystems.Models.Entities;

namespace COMP019_Activity4_4JLCSystems.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("login")]
        public IActionResult Login(string? returnUrl = null)
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                var role = HttpContext.Session.GetString("UserRole");
                if (role == "Admin")
                    return Redirect("/admin/dashboard");
                else
                    return Redirect("/shop");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [Route("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == model.Username && 
                                              u.Password == model.Password && 
                                              u.IsActive);

                if (user != null)
                {
                    HttpContext.Session.SetInt32("UserId", user.UserId);
                    HttpContext.Session.SetString("Username", user.Username);
                    HttpContext.Session.SetString("UserRole", user.Role);
                    HttpContext.Session.SetString("FullName", user.FullName ?? user.Username);

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    if (user.IsAdmin)
                    {
                        return Redirect("/admin/dashboard");
                    }
                    else
                    {
                        return Redirect("/shop");
                    }
                }

                ModelState.AddModelError(string.Empty, "Invalid username or password.");
            }

            return View(model);
        }

        [Route("signup")]
        public IActionResult Register()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                var role = HttpContext.Session.GetString("UserRole");
                if (role == "Admin")
                    return Redirect("/admin/dashboard");
                else
                    return Redirect("/shop");
            }

            return View();
        }

        [HttpPost]
        [Route("signup")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Users.AnyAsync(u => u.Username == model.Username))
                {
                    ModelState.AddModelError("Username", "Username is already taken.");
                    return View(model);
                }

                if (!string.IsNullOrEmpty(model.Email) && await _context.Users.AnyAsync(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email is already registered.");
                    return View(model);
                }

                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    Password = model.Password,
                    FullName = model.FullName,
                    Role = "User",
                    DateCreated = DateTime.Now,
                    IsActive = true
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("UserRole", user.Role);
                HttpContext.Session.SetString("FullName", user.FullName ?? user.Username);

                TempData["Success"] = "Account created successfully! Welcome to 4JLC.";
                return Redirect("/shop");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "You have been logged out successfully.";
            return Redirect("/landing");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
