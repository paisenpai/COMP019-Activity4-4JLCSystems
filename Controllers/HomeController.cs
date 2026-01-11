using System.Diagnostics;
using COMP019_Activity4_4JLCSystems.Models;
using Microsoft.AspNetCore.Mvc;

namespace COMP019_Activity4_4JLCSystems.Controllers
{
    public class HomeController : Controller
    {
        [Route("")]
        [Route("landing")]
        public IActionResult Landing()
        {
            // If already logged in, redirect to appropriate dashboard
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                var role = HttpContext.Session.GetString("UserRole");
                if (role == "Admin")
                    return RedirectToAction("Index", "Dashboard");
                else
                    return RedirectToAction("Index", "Shop");
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
