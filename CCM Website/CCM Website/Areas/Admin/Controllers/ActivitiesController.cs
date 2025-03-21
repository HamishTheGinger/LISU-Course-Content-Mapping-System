using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CCM_Website.Data;
using CCM_Website.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;
using X.PagedList.Mvc.Core;

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
        public Task<IActionResult> Index(string searchString, int? page)
        {
            var activities = _context.Activities.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                activities = activities.Where(a =>
                    a.ActivityName.ToLower().Contains(searchString.ToLower())
                );
            }

            int pageSize = 10;
            int pageNumber = page ?? 1;

            ViewData["SearchString"] = searchString;

            var pagedActivities = activities.ToPagedList(pageNumber, pageSize);
            return Task.FromResult<IActionResult>(View(pagedActivities));
        }

        // GET: Admin/Activities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activities = await _context
                .Activities.Include(a => a.LearningPlatformActivities!)
                .ThenInclude(lpa => lpa.LearningPlatform)
                .FirstOrDefaultAsync(m => m.ActivityId == id);

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
            [Bind("ActivityId,ActivityName")] Activities activities,
            List<int> LearningPlatformIds
        )
        {
            var platforms = _context.LearningPlatforms.ToList();
            ViewBag.LearningPlatforms = new SelectList(
                platforms,
                "PlatformId",
                "PlatformName",
                LearningPlatformIds
            );

            try
            {
                activities.WeekActivities = new List<WeekActivities>();
                activities.LearningPlatformActivities = new List<LearningPlatformActivities>();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Model Creation Error: {e.Message}");
                ModelState.AddModelError("", "An error occurred while saving the activity.");
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
                    foreach (var platformId in LearningPlatformIds)
                    {
                        var platform = await _context.LearningPlatforms.FindAsync(platformId);
                        if (platform != null)
                        {
                            var link = new LearningPlatformActivities
                            {
                                ActivitiesId = activities.ActivityId,
                                LearningPlatformId = platformId,
                                Activities = activities,
                                LearningPlatform = platform,
                            };
                            _context.LearningPlatformActivities.Add(link);
                        }
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exp)
                {
                    Console.WriteLine($"Model Creation Error: {exp.Message}");
                    ModelState.AddModelError("", "An error occurred while saving the activity.");
                    return View(activities);
                }
            }

            return View(activities);
        }

        // GET: Admin/Activities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activities = await _context
                .Activities.Include(a => a.LearningPlatformActivities)
                .FirstOrDefaultAsync(a => a.ActivityId == id);

            if (activities == null)
            {
                return NotFound();
            }

            var selectedPlatformIds = activities
                .LearningPlatformActivities?.Select(lpa => lpa.LearningPlatformId)
                .ToList();

            var platforms = await _context.LearningPlatforms.ToListAsync();
            ViewBag.LearningPlatforms = new MultiSelectList(
                platforms,
                "PlatformId",
                "PlatformName",
                selectedPlatformIds // This should be an IEnumerable
            );

            return View(activities);
        }

        // POST: Admin/Activities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("ActivityId,ActivityName")] Activities activities,
            List<int> LearningPlatformIds
        )
        {
            if (id != activities.ActivityId)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(activities.WeekActivities));
            ModelState.Remove(nameof(activities.LearningPlatformActivities));

            if (ModelState.IsValid)
            {
                try
                {
                    var existingActivity = await _context
                        .Activities.Include(a => a.LearningPlatformActivities)
                        .FirstOrDefaultAsync(a => a.ActivityId == id);

                    if (existingActivity == null)
                    {
                        return NotFound();
                    }

                    existingActivity.ActivityName = activities.ActivityName;

                    var existingLinks =
                        existingActivity.LearningPlatformActivities?.ToList()
                        ?? new List<LearningPlatformActivities>();
                    _context.LearningPlatformActivities.RemoveRange(existingLinks);

                    foreach (var platformId in LearningPlatformIds)
                    {
                        var platform = await _context.LearningPlatforms.FindAsync(platformId);
                        if (platform != null)
                        {
                            var link = new LearningPlatformActivities
                            {
                                ActivitiesId = id,
                                LearningPlatformId = platformId,
                                Activities = existingActivity,
                                LearningPlatform = platform,
                            };
                            _context.LearningPlatformActivities.Add(link);
                        }
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Edit Error: {ex.Message}");
                    ModelState.AddModelError("", "An error occurred while updating the activity.");
                }
            }

            var platforms = await _context.LearningPlatforms.ToListAsync();
            ViewBag.LearningPlatforms = new SelectList(
                platforms,
                "PlatformId",
                "PlatformName",
                LearningPlatformIds
            );

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
