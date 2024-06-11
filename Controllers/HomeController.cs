﻿using email_alerts.Data.Repositories;
using email_alerts.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;


namespace email_alerts.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly EmailAlertRepository _emailLogRepository;

        public HomeController(ILogger<HomeController> logger, EmailAlertRepository emailLogRepository)
        {
            _logger = logger;
            _emailLogRepository = emailLogRepository;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            string username = HttpContext.Request.Headers["X-Forwarded-Preferred-Username"].ToString() ?? "Not signed in";
            if (Request.Headers.ContainsKey("X-Forwarded-Preferred-Username"))
            {
                username = Request.Headers["X-Forwarded-Preferred-Username"].ToString();
            }
            ViewData["Username"] = username;
        }

        public IActionResult Index()
        {
            var displayQueries = _emailLogRepository.GetAllQueriesToDisplay().ToList();
            return View(displayQueries);
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
