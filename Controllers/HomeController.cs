using email_alerts.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace email_alerts.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            string username = HttpContext.Request.Headers["X-Forwarded-Preferred-Username"].ToString() ?? "Not signed in";
            if (Request.Headers.ContainsKey("X-Forwarded-Preferred-Username"))
                username = Request.Headers["X-Forwarded-Preferred-Username"].ToString();
            ViewData["Username"] = username;
            Console.WriteLine(username);
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