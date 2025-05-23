using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CCM_Website.Data;
using CCM_Website.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace CCM_Website.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class WeekController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WeekController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Week
        public Task<IActionResult> Index(string searchString, int? page)
        {
            var week = _context.Weeks.Include(w => w.Workbook).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                week = week.Where(w =>
                    w.Workbook.CourseName.ToLower().Contains(searchString.ToLower())
                );
            }

            int pageSize = 10;
            int pageNumber = page ?? 1;

            ViewData["SearchString"] = searchString;

            var pagedWeeks = week.ToPagedList(pageNumber, pageSize);

            return Task.FromResult<IActionResult>(View(pagedWeeks));
        }

        // GET: Admin/Week/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var week = await _context
                .Weeks.Include(w => w.Workbook)
                .FirstOrDefaultAsync(m => m.WeekId == id);
            if (week == null)
            {
                return NotFound();
            }

            return View(week);
        }

        // GET: Admin/Week/Create
        public IActionResult Create()
        {
            ViewData["WorkbookId"] = new SelectList(_context.Workbooks, "WorkbookId", "WorkbookId");
            return View();
        }

        // POST: Admin/Week/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WeekId,WeekNumber,WorkbookId")] Week week)
        {
            if (ModelState.IsValid)
            {
                _context.Add(week);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["WorkbookId"] = new SelectList(
                _context.Workbooks,
                "WorkbookId",
                "WorkbookId",
                week.WorkbookId
            );
            return View(week);
        }

        // GET: Admin/Week/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var week = await _context
                .Weeks.Include(w => w.Workbook)
                .FirstOrDefaultAsync(m => m.WeekId == id);
            if (week == null)
            {
                return NotFound();
            }

            return View(week);
        }

        // POST: Admin/Week/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var week = await _context.Weeks.FindAsync(id);
            if (week != null)
            {
                _context.Weeks.Remove(week);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WeekExists(int id)
        {
            return _context.Weeks.Any(e => e.WeekId == id);
        }
    }
}
