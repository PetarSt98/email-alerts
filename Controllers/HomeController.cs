using email_alerts.Data.Repositories;
using email_alerts.Models;
using email_alerts.Models.EmailAlerts;
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
            SetUsername();
        }

        private void SetUsername()
        {
            string username = HttpContext.Request.Headers["X-Forwarded-Preferred-Username"].ToString() ?? "Not signed in";

            if (Request.Headers.ContainsKey("X-Forwarded-Preferred-Username"))
                username = Request.Headers["X-Forwarded-Preferred-Username"].ToString();
            ViewData["Username"] = username;
        }

        public IActionResult Index()
        {
            SetUsername();
            var displayQueries = _emailLogRepository.GetAllQueriesToDisplay().ToList();
            return View(displayQueries);
        }

        public IActionResult Privacy()
        {
            SetUsername();
            return View();
        }

        public IActionResult Details(int id)
        {
            SetUsername();
            var query = _emailLogRepository.GetQueryById(id);
            if (query == null)
            {
                return NotFound();
            }
            return View(query);
        }

        public IActionResult History(int id)
        {
            SetUsername();
            var query = _emailLogRepository.GetQueryById(id);
            if (query == null)
            {
                return NotFound();
            }

            var emailLogs = _emailLogRepository.GetEmailLogsByQueryId(id).ToList();
            var historyViewModel = new HistoryViewModel
            {
                QueryDescription = query.Description,
                EmailLogs = emailLogs
            };

            return View(historyViewModel);
        }

        public IActionResult AlertsToBeSent(int id)
        {
            SetUsername();
            var query = _emailLogRepository.GetQueryById(id);
            if (query == null)
            {
                return NotFound();
            }
            return View(query);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            SetUsername();
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult Edit(Query query)
        {
            if (ModelState.IsValid)
            {
                _emailLogRepository.UpdateQuery(query);
                return RedirectToAction("Details", new { id = query.id });
            }
            return View("Details", query);
        }

        public IActionResult NewAlert()
        {
            SetUsername();
            return View(new Query());
        }

        [HttpPost]
        public IActionResult Create(Query query)
        {
            if (ModelState.IsValid)
            {
                _emailLogRepository.AddQuery(query);
                return RedirectToAction("Index");
            }
            return View("NewAlert", query);
        }
    }
}
