using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CCM_Website.Data;
using CCM_Website.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CCM_Website.Controllers
{
    public class MyWorkbooksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MyWorkbooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Courses
        public async Task<IActionResult> Index()
        {
            return View(await _context.Workbooks.ToListAsync());
        }

        // POST: Courses/Search
        public async Task<IActionResult> Search(string SearchPhrase)
        {
            // Check if the SearchPhrase is not null or empty
            if (string.IsNullOrEmpty(SearchPhrase))
            {
                // If no search phrase, return an empty list or all courses
                return View("Search", await _context.Workbooks.ToListAsync());
            }

            // Perform search filtering across multiple fields
            var searchResults = await _context.Workbooks
                .Where(c => c.CourseName.Contains(SearchPhrase) ||
                            c.CourseLead.Contains(SearchPhrase))
                .ToListAsync();

            // Return the search form view with search results and the search phrase
            ViewData["SearchPhrase"] = SearchPhrase;
            return View("Search", searchResults);
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workbook = await _context.Workbooks
                .Include(w => w.Weeks)
                .FirstOrDefaultAsync(m => m.WorkbookId == id);
            if (workbook == null)
            {
                return NotFound();
            }

            return View(workbook);
        }

        // GET: Courses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CourseName,CourseLead")] Workbook workbook)
        {
            if (ModelState.IsValid)
            {
                _context.Add(workbook);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(workbook);
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Workbooks.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CourseName,CourseLead")] Workbook workbook)
        {
            if (id != workbook.WorkbookId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workbook);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(workbook.WorkbookId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(workbook);
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workbook = await _context.Workbooks
                .FirstOrDefaultAsync(m => m.WorkbookId == id);
            if (workbook == null)
            {
                return NotFound();
            }

            return View(workbook);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Workbooks.FindAsync(id);
            if (course != null)
            {
                _context.Workbooks.Remove(course);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        // GET: Courses
        public async Task<IActionResult> Week(int id)
        {
            var week = await _context.Weeks
                .Include(w => w.Workbook) 
                .Include(w => w.WeekActivities) 
                .Include(w => w.WeekGraduateAttributes)
                .FirstOrDefaultAsync(w => w.WeekId == id);

            if (week == null)
            {
                return NotFound();
            }

            return View(week);
        }
        
        private bool CourseExists(int id)
        {
            return _context.Workbooks.Any(e => e.WorkbookId == id);
        }
    }
    
}
