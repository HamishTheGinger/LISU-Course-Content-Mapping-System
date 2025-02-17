using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CCM_Website.Data;
using CCM_Website.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CCM_Website.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class WorkbookController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkbookController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Workbook
        public async Task<IActionResult> Index()
        {
            return View(await _context.Workbooks.ToListAsync());
        }

        // GET: Admin/Workbook/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workbook = await _context.Workbooks.FirstOrDefaultAsync(m => m.WorkbookId == id);
            if (workbook == null)
            {
                return NotFound();
            }

            return View(workbook);
        }

        // GET: Admin/Workbook/Create
        public IActionResult Create()
        {
            ViewBag.LearningPlatforms = new SelectList(
                _context.LearningPlatforms,
                "PlatformId",
                "PlatformName"
            );
            return View();
        }

        // POST: Admin/Workbook/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind(
                "WorkbookId,CourseName,CourseCode,PipReference,CourseLead,CourseLength,LearningPlatformId,Collaborators"
            )]
                Workbook workbook
        )
        {
            try
            {
                var learningPlatform = await _context.LearningPlatforms.FirstOrDefaultAsync(lp =>
                    lp.PlatformId == workbook.LearningPlatformId
                );

                if (learningPlatform == null)
                {
                    Console.WriteLine("ERROR: Failed to link Workbook to Learning Platform");
                    ModelState.AddModelError(
                        "",
                        "An error occurred while saving the workbook. Please try again later."
                    );

                    ViewBag.LearningPlatforms = new SelectList(
                        _context.LearningPlatforms,
                        "PlatformId",
                        "PlatformName"
                    );

                    return View(workbook);
                }

                workbook.LearningPlatform = learningPlatform;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Model Creation Error: {e.Message}");
                ModelState.AddModelError(
                    "",
                    "An error occurred while saving the workbook. Please try again later."
                );

                ViewBag.LearningPlatforms = new SelectList(
                    _context.LearningPlatforms,
                    "PlatformId",
                    "PlatformName"
                );

                return View(workbook);
            }

            ModelState.Remove(nameof(workbook.LearningPlatform));
            ModelState.Remove(nameof(workbook.Weeks));

            if (ModelState.IsValid)
            {
                try
                {
                    workbook.LastEdited = DateTime.Now;
                    _context.Add(workbook);
                    await _context.SaveChangesAsync();

                    int numberOfWeeks = workbook.CourseLength;
                    var weeks = new List<Week>();

                    for (int i = 1; i <= numberOfWeeks; i++)
                    {
                        weeks.Add(
                            new Week
                            {
                                WeekNumber = i,
                                WorkbookId = workbook.WorkbookId,
                                Workbook = workbook,
                                WeekActivities = new List<WeekActivities>(),
                                WeekGraduateAttributes = new List<WeekGraduateAttributes>(),
                            }
                        );
                    }

                    workbook.Weeks = weeks;
                    _context.AddRange(weeks);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"DB Error occurred: {ex.Message}");
                    ModelState.AddModelError(
                        "",
                        "An error occurred while saving the workbook. Please try again later."
                    );

                    ViewBag.LearningPlatforms = new SelectList(
                        _context.LearningPlatforms,
                        "PlatformId",
                        "PlatformName"
                    );

                    return View(workbook);
                }
            }

            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    Console.WriteLine($"Error for field {state.Key}: {error.ErrorMessage}");
                }
            }

            ModelState.AddModelError(
                "",
                "An error occurred while creating the workbook. Please contact an administrator if this problem persists."
            );

            ViewBag.LearningPlatforms = new SelectList(
                _context.LearningPlatforms,
                "PlatformId",
                "PlatformName"
            );

            return View(workbook);
        }

        // GET: Admin/Workbook/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workbook = await _context
                .Workbooks.Include(w => w.LearningPlatform)
                .FirstOrDefaultAsync(m => m.WorkbookId == id);
            if (workbook == null)
            {
                return NotFound();
            }

            ViewBag.LearningPlatforms = new SelectList(
                _context.LearningPlatforms,
                "PlatformId",
                "PlatformName",
                workbook.LearningPlatformId
            );
            return View(workbook);
        }

        // POST: Admin/Workbook/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind(
                "WorkbookId,CourseName,CourseCode,PipReference,CourseLead,CourseLength,LearningPlatformId,Collaborators"
            )]
                Workbook workbook
        )
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
                    var learningPlatform = await _context.LearningPlatforms.FirstOrDefaultAsync(
                        lp => lp.PlatformId == workbook.LearningPlatformId
                    );
                    if (learningPlatform == null)
                    {
                        ModelState.AddModelError(
                            "LearningPlatformId",
                            "Invalid Learning Platform selected."
                        );
                        ViewBag.LearningPlatforms = new SelectList(
                            _context.LearningPlatforms,
                            "PlatformId",
                            "PlatformName",
                            workbook.LearningPlatformId
                        );
                        return View(workbook);
                    }

                    workbook.LearningPlatform = learningPlatform;
                    workbook.LastEdited = DateTime.Now;
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

            ViewBag.LearningPlatforms = new SelectList(
                _context.LearningPlatforms,
                "PlatformId",
                "PlatformName",
                workbook.LearningPlatformId
            );

            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    // Log each error (you could also store or display them if needed)
                    Console.WriteLine($"Error for field {state.Key}: {error.ErrorMessage}");
                }
            }
            ModelState.AddModelError(
                "",
                "An error occurred while creating the workbook. Please contact an administrator if this problem persists."
            );
            return View(workbook);
        }

        // GET: Admin/Workbook/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workbook = await _context.Workbooks.FirstOrDefaultAsync(m => m.WorkbookId == id);
            if (workbook == null)
            {
                return NotFound();
            }

            return View(workbook);
        }

        // POST: Admin/Workbook/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workbook = await _context.Workbooks.FindAsync(id);
            if (workbook != null)
            {
                _context.Workbooks.Remove(workbook);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkbookExists(int id)
        {
            return _context.Workbooks.Any(e => e.WorkbookId == id);
        }
    }
}
