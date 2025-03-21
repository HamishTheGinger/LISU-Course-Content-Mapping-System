using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using CCM_Website.Data;
using CCM_Website.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace CCM_Website.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var myWorkbooks = _context
                .Workbooks.Where(w => w.OwnerId == userId)
                .OrderByDescending(w => w.LastEdited)
                .Take(12)
                .ToList();

            var uofgWorkbooks = _context
                .Workbooks.Take(12)
                .OrderByDescending(w => w.LastEdited)
                .ToList();

            ViewData["MyWorkbooks"] = myWorkbooks;
            ViewData["UofGWorkbooks"] = uofgWorkbooks;
            return View();
        }

        public IActionResult Settings()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(
                new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                }
            );
        }
    }
}
