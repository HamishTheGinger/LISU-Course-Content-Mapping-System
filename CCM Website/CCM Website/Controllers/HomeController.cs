using CCM_Website.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;
using CCM_Website.Data;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace CCM_Website.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(string searchQuery)
        {
            var myWorkbooks = _context.WorkBooks
                .Where(w => (string.IsNullOrEmpty(searchQuery) || w.CourseName.Contains(searchQuery)))
                .OrderByDescending(w => w.LastEdited)
                .ToList();

            var uofgWorkbooks = _context.WorkBooks
                .Where(w => (string.IsNullOrEmpty(searchQuery) || w.CourseName.Contains(searchQuery)))
                .ToList();

            ViewData["SearchPhrase"] = searchQuery;
            ViewData["MyWorkbooks"] = myWorkbooks;
            ViewData["UofGWorkbooks"] = uofgWorkbooks;
            return View();
        }
        

        public IActionResult Settings()
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