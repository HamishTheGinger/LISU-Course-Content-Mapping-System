using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class ActivitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ActivitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Activities
        public async Task<IActionResult> Index()
        {
            return View(await _context.Activities.ToListAsync());
        }

        // GET: Admin/Activities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activities = await _context.Activities.FirstOrDefaultAsync(m => m.ActivityId == id);
            if (activities == null)
            {
                return NotFound();
            }

            return View(activities);
        }

        // GET: Admin/Activities/Create
        public IActionResult Create()
        {
            var platforms = _context.LearningPlatforms.ToList(); // _context is your DB context

            ViewBag.LearningPlatforms = new SelectList(platforms, "PlatformId", "PlatformName");

            return View();
        }

        // POST: Admin/Activities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("ActivityId,ActivityName")] Activities activities
        )
        {
            var platforms = _context.LearningPlatforms.ToList(); // _context is your DB context
            ViewBag.LearningPlatforms = new SelectList(platforms, "PlatformId", "PlatformName");

            try
            {
                activities.WeekActivities = new List<WeekActivities>();
                activities.LearningPlatformActivities = new List<LearningPlatformActivities>();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Model Creation Error: {e.Message}");
                ModelState.AddModelError(
                    "",
                    "An error occurred while saving the activity. Please try again later."
                );
                return View(activities);
            }

            ModelState.Remove(nameof(activities.WeekActivities));
            ModelState.Remove(nameof(activities.LearningPlatformActivities));

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(activities);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exp)
                {
                    Console.WriteLine($"Model Creation Error: {exp.Message}");
                    ModelState.AddModelError(
                        "",
                        "An error occurred while saving the workbook. Please try again later."
                    );
                    return View(activities);
                }
            }

            Console.WriteLine("Error creating model");
            return View(activities);
        }

        // GET: Admin/Activities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activities = await _context.Activities.FindAsync(id);
            if (activities == null)
            {
                return NotFound();
            }
            return View(activities);
        }

        // POST: Admin/Activities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("ActivityId,ActivityName")] Activities activities
        )
        {
            if (id != activities.ActivityId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(activities);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActivitiesExists(activities.ActivityId))
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
            return View(activities);
        }

        // GET: Admin/Activities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activities = await _context.Activities.FirstOrDefaultAsync(m => m.ActivityId == id);
            if (activities == null)
            {
                return NotFound();
            }

            return View(activities);
        }

        // POST: Admin/Activities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var activities = await _context.Activities.FindAsync(id);
            if (activities != null)
            {
                _context.Activities.Remove(activities);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActivitiesExists(int id)
        {
            return _context.Activities.Any(e => e.ActivityId == id);
        }
    }
}
