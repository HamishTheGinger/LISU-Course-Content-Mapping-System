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
    public class WeekActivitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WeekActivitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: WeekActivities
        public Task<IActionResult> Index(string searchString, int? page)
        {
            var weekActivities = _context.WeekActivities.Include(wa => wa.Activities).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                weekActivities = weekActivities.Where(wa =>
                    wa.TaskTitle.ToLower().Contains(searchString.ToLower())
                );
            }

            int pageSize = 10;
            int pageNumber = page ?? 1;

            var pagedWeekActivities = weekActivities.ToPagedList(pageNumber, pageSize);

            return Task.FromResult<IActionResult>(View(pagedWeekActivities));
        }

        // GET: WeekActivities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weekActivity = await _context
                .WeekActivities.Include(wa => wa.Week)
                .Include(wa => wa.Activities)
                .FirstOrDefaultAsync(m => m.WeekActivityId == id);
            if (weekActivity == null)
            {
                return NotFound();
            }

            return View(weekActivity);
        }

        // GET: WeekActivities/Create
        public IActionResult Create(int? weekId)
        {
            if (weekId == null)
            {
                return BadRequest("Week ID is required.");
            }

            var week = _context
                .Weeks.Include(w => w.Workbook)
                .ThenInclude(wb => wb.LearningPlatform)
                .FirstOrDefault(w => w.WeekId == weekId);

            if (week == null)
            {
                return NotFound();
            }

            var learningPlatformId = week.Workbook.LearningPlatformId;

            var allowedActivities = _context
                .LearningPlatformActivities.Where(lpa =>
                    lpa.LearningPlatformId == learningPlatformId
                )
                .Select(lpa => lpa.Activities)
                .ToList();

            ViewBag.WeekId = new SelectList(
                _context.Weeks.ToList(),
                "WeekId",
                "WeekNumber",
                weekId
            );
            ViewBag.ActivitiesId = new SelectList(allowedActivities, "ActivityId", "ActivityName");
            ViewBag.LearningApproach = new SelectList(
                _context.LearningType.ToList(),
                "LearningTypeId",
                "LearningTypeName"
            );
            ViewBag.TaskApproach = new SelectList(
                _context.TaskApproach.ToList(),
                "ApproachId",
                "ApproachName"
            );
            ViewBag.TaskLocation = new SelectList(
                _context.TaskLocation.ToList(),
                "LocationId",
                "LocationName"
            );
            ViewBag.TaskStatus = new SelectList(
                _context.TaskProgressStatus.ToList(),
                "StatusId",
                "StatusName"
            );

            return View();
        }

        // POST: WeekActivities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind(
                "TaskOrder,WeekId,ActivitiesId,TaskTitle,TaskStaff,ActivityTime,TasksStatusId,LearningTypeId,TaskLocationId,TaskApproachId"
            )]
                WeekActivities weekActivities
        )
        {
            var week = await _context
                .Weeks.Include(w => w.Workbook)
                .ThenInclude(wb => wb.LearningPlatform)
                .FirstOrDefaultAsync(w => w.WeekId == weekActivities.WeekId);

            if (week == null)
            {
                return NotFound();
            }

            var learningPlatformId = week.Workbook.LearningPlatformId;

            bool isAllowedActivity = await _context.LearningPlatformActivities.AnyAsync(lpa =>
                lpa.LearningPlatformId == learningPlatformId
                && lpa.ActivitiesId == weekActivities.ActivitiesId
            );

            if (!isAllowedActivity)
            {
                ModelState.AddModelError(
                    "ActivitiesId",
                    "The selected activity is not allowed for this learning platform."
                );
            }

            ModelState.Remove(nameof(weekActivities.Week));
            ModelState.Remove(nameof(weekActivities.Activities));
            ModelState.Remove(nameof(weekActivities.LearningType));
            ModelState.Remove(nameof(weekActivities.TaskLocation));
            ModelState.Remove(nameof(weekActivities.TaskApproach));
            ModelState.Remove(nameof(weekActivities.TasksStatus));

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(weekActivities);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exp)
                {
                    Console.WriteLine(exp.Message);
                    ModelState.AddModelError("", $"Error creating Week Activity: {exp.Message}");
                }
            }
            else
            {
                Console.WriteLine("Validation failed. Errors:");
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Field: {state.Key} - Error: {error.ErrorMessage}");
                    }
                }
                ModelState.AddModelError(
                    "",
                    "An error occurred while creating the Weekly Activity. Please check all fields."
                );
            }

            ViewBag.WeekId = new SelectList(
                _context.Weeks.ToList(),
                "WeekId",
                "WeekNumber",
                weekActivities.WeekId
            );
            ViewBag.ActivitiesId = new SelectList(
                _context.Activities.ToList(),
                "ActivityId",
                "ActivityName",
                weekActivities.ActivitiesId
            );
            ViewBag.LearningApproach = new SelectList(
                _context.LearningType.ToList(),
                "LearningTypeId",
                "LearningTypeName"
            );
            ViewBag.TaskApproach = new SelectList(
                _context.TaskApproach.ToList(),
                "ApproachId",
                "ApproachName"
            );
            ViewBag.TaskLocation = new SelectList(
                _context.TaskLocation.ToList(),
                "LocationId",
                "LocationName"
            );
            ViewBag.TaskStatus = new SelectList(
                _context.TaskProgressStatus.ToList(),
                "StatusId",
                "StatusName"
            );

            return View(weekActivities);
        }

        // GET: WeekActivities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weekActivity = await _context.WeekActivities.FindAsync(id);
            if (weekActivity == null)
            {
                return NotFound();
            }

            ViewData["WeekId"] = new SelectList(
                _context.Weeks,
                "WeekId",
                "WeekNumber",
                weekActivity.WeekId
            );
            ViewData["ActivitiesId"] = new SelectList(
                _context.Activities,
                "ActivityId",
                "ActivityName",
                weekActivity.ActivitiesId
            );
            ViewData["LearningApproach"] = new SelectList(
                _context.LearningType,
                "LearningTypeId",
                "LearningTypeName"
            );
            return View(weekActivity);
        }

        // POST: WeekActivities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind(
                "WeekActivityId, TaskOrder,WeekId,ActivitiesId,TaskTitle,TaskStaff,ActivityTime,TasksStatus,LearningTypeId,TaskLocation,TaskApproach"
            )]
                WeekActivities weekActivities
        )
        {
            if (id != weekActivities.WeekActivityId)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(weekActivities.Week));
            ModelState.Remove(nameof(weekActivities.Activities));
            ModelState.Remove(nameof(weekActivities.LearningType));

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(weekActivities);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WeekActivitiesExists(weekActivities.WeekActivityId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    // Log each error (you could also store or display them if needed)
                    Console.WriteLine($"Error for field {state.Key}: {error.ErrorMessage}");
                }
                ModelState.AddModelError(
                    "",
                    "An error occurred while creating the workbook. Please contact an administrator if this problem persists."
                );
            }
            try
            {
                var week = await _context.Weeks.FirstOrDefaultAsync(wk =>
                    wk.WeekId == weekActivities.WeekId
                );
                var activity = await _context.Activities.FirstOrDefaultAsync(a =>
                    a.ActivityId == weekActivities.ActivitiesId
                );
                var learningType = await _context.LearningType.FirstOrDefaultAsync(lt =>
                    lt.LearningTypeId == weekActivities.LearningTypeId
                );
                if (week == null || activity == null || learningType == null)
                {
                    Console.WriteLine($"ERROR: Link Fail");
                    ModelState.AddModelError(
                        "",
                        "An error occurred while saving the weekly activity. Please try again later."
                    );
                    return View(weekActivities);
                }
                weekActivities.Week = week;
                weekActivities.Activities = activity;
                weekActivities.LearningType = learningType;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Model Creation Error: {e.Message}");
                ModelState.AddModelError(
                    "",
                    "An error occurred while saving the weekly activity. Please try again later."
                );
                return View(weekActivities);
            }

            ViewData["WeekId"] = new SelectList(
                _context.Weeks,
                "WeekId",
                "WeekNumber",
                weekActivities.WeekId
            );
            ViewData["ActivitiesId"] = new SelectList(
                _context.Activities,
                "ActivityId",
                "ActivityName",
                weekActivities.ActivitiesId
            );
            ViewData["LearningApproach"] = new SelectList(
                _context.LearningType,
                "LearningTypeId",
                "LearningTypeName"
            );

            return View(weekActivities);
        }

        // GET: WeekActivities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weekActivity = await _context
                .WeekActivities.Include(wa => wa.Week)
                .Include(wa => wa.Activities)
                .FirstOrDefaultAsync(m => m.WeekActivityId == id);
            if (weekActivity == null)
            {
                return NotFound();
            }

            return View(weekActivity);
        }

        // POST: WeekActivities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var weekActivity = await _context.WeekActivities.FindAsync(id);
            if (weekActivity == null)
            {
                Console.WriteLine($"ERROR: WeekActivity with ID {id} not found.");
                return NotFound();
            }
            _context.WeekActivities.Remove(weekActivity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WeekActivitiesExists(int Id)
        {
            return _context.WeekActivities.Any(e => e.WeekActivityId == Id);
        }
    }
}
