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
    public class WorkbooksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkbooksController(ApplicationDbContext context)
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
        public async Task<IActionResult> Create([Bind("WorkbookId,CourseName,CourseCode,CourseLead,CourseLength,LearningPlatformId,Collaborators")] Workbook workbook) {
            try
            {
                var learningPlatform = await _context.LearningPlatforms.FirstOrDefaultAsync(lp => lp.PlatformId == workbook.LearningPlatformId);
                workbook.LearningPlatform = learningPlatform;
                if (workbook.LearningPlatform == null)
                {
                    Console.WriteLine($"ERROR: Failed to link Workbook to Learning Platform");
                    ModelState.AddModelError("", "An error occurred while saving the workbook. Please try again later.");
                    return View(workbook);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Model Creation Error: {e.Message}");
                ModelState.AddModelError("", "An error occurred while saving the workbook. Please try again later.");
                return View(workbook);
            }
            
            ModelState.Remove(nameof(workbook.LearningPlatform));
            ModelState.Remove(nameof(workbook.Weeks));

            if (ModelState.IsValid) {
                try
                {
                    workbook.LastEdited = DateTime.Now;
                    _context.Add(workbook);
                    await _context.SaveChangesAsync();

                    int numberOfWeeks = workbook.CourseLength;

                    var weeks = new List<Week>();

                    for (int i = 1; i <= numberOfWeeks; i++)
                    {
                        weeks.Add(new Week
                        {
                            WeekNumber = i, WorkbookId = workbook.WorkbookId, Workbook = workbook,
                            WeekActivities = new List<WeekActivities>(),
                            WeekGraduateAttributes = new List<WeekGraduateAttributes>()
                        });
                    }

                    workbook.Weeks = weeks;
                    _context.AddRange(weeks);
                    
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex) {
                    Console.WriteLine($"DB Error occurred: {ex.Message}");
                    ModelState.AddModelError("", "An error occurred while saving the workbook. Please try again later.");
                    return View(workbook);
                }
                
            }

            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    // Log each error (you could also store or display them if needed)
                    Console.WriteLine($"Error for field {state.Key}: {error.ErrorMessage}");
                }
            }
            ModelState.AddModelError("",
                "An error occurred while creating the workbook. Please contact an administrator if this problem persists.");
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
        public async Task<IActionResult> Edit(int id, [Bind("WorkbookId,CourseName,CourseCode,CourseLead,CourseLength,LearningPlatformId,Collaborators")] Workbook workbook)
        {
            if (id != workbook.WorkbookId)
            {
                return NotFound();
            }
            
            ModelState.Remove(nameof(workbook.LearningPlatform));
            ModelState.Remove(nameof(workbook.Weeks));

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workbook);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkbookExists(workbook.WorkbookId))
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
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    // Log each error (you could also store or display them if needed)
                    Console.WriteLine($"Error for field {state.Key}: {error.ErrorMessage}");
                }
            }
            ModelState.AddModelError("",
                "An error occurred while creating the workbook. Please contact an administrator if this problem persists.");
            return View(workbook);
        }
        
        // GET: Courses
        public async Task<IActionResult> Week(int id)
        {
            var week = await _context.Weeks
                .Include(w => w.Workbook)
                    .ThenInclude(wb => wb.Weeks)
                .Include(w => w.WeekActivities) 
                .Include(w => w.WeekGraduateAttributes)
                .FirstOrDefaultAsync(w => w.WeekId == id);

            if (week == null)
            {
                return NotFound();
            }

            return View(week);
        }
        
        private bool WorkbookExists(int id)
        {
            return _context.Workbooks.Any(e => e.WorkbookId == id);
        }
    }
    
}
