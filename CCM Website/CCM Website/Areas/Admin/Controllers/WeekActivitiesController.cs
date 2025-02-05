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
    public class WeekActivitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WeekActivitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: WeekActivities
        public async Task<IActionResult> Index()
        {
            var weekActivities = _context
                .WeekActivities.Include(wa => wa.Week)
                .Include(wa => wa.Activities);
            return View(await weekActivities.ToListAsync());
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
        public IActionResult Create()
        {
            ViewBag.WeekId = new SelectList(_context.Weeks, "WeekId", "WeekNumber");
            ViewBag.ActivitiesId = new SelectList(
                _context.Activities,
                "ActivityId",
                "ActivityName"
            );
            ViewBag.LearningApproach = new SelectList(
                _context.LearningType,
                "LearningTypeId",
                "LearningTypeName"
            );
            return View();
        }

        // POST: WeekActivities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind(
                "TaskOrder,WeekId,ActivitiesId,TaskTitle,TaskStaff,ActivityTime,TasksStatus,LearningTypeId,TaskLocation,TaskApproach"
            )]
                WeekActivities weekActivities
        )
        {
            ModelState.Remove(nameof(weekActivities.Week));
            ModelState.Remove(nameof(weekActivities.Activities));
            ModelState.Remove(nameof(weekActivities.LearningType));

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
                    ModelState.AddModelError("", $"Error creating WeekActivity: {exp.Message}");
                }
            }
            else
            {
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
            }

            try
            {
                var week = await _context.Weeks.FirstOrDefaultAsync(wk =>
                    wk.WeekId == weekActivities.WeekId
                );
                weekActivities.Week = week;
                var activity = await _context.Activities.FirstOrDefaultAsync(a =>
                    a.ActivityId == weekActivities.ActivitiesId
                );
                weekActivities.Activities = activity;
                var learingType = await _context.LearningType.FirstOrDefaultAsync(lt =>
                    lt.LearningTypeId == weekActivities.LearningTypeId
                );
                weekActivities.LearningType = learingType;
                if (
                    weekActivities.Week == null
                    || weekActivities.Activities == null
                    || weekActivities.LearningType == null
                )
                {
                    Console.WriteLine($"ERROR: Link Fail");
                    ModelState.AddModelError(
                        "",
                        "An error occurred while saving the weekly activity. Please try again later."
                    );
                    return View(weekActivities);
                }
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
                weekActivities.Week = week;
                var activity = await _context.Activities.FirstOrDefaultAsync(a =>
                    a.ActivityId == weekActivities.ActivitiesId
                );
                weekActivities.Activities = activity;
                var learingType = await _context.LearningType.FirstOrDefaultAsync(lt =>
                    lt.LearningTypeId == weekActivities.LearningTypeId
                );
                weekActivities.LearningType = learingType;
                if (
                    weekActivities.Week == null
                    || weekActivities.Activities == null
                    || weekActivities.LearningType == null
                )
                {
                    Console.WriteLine($"ERROR: Link Fail");
                    ModelState.AddModelError(
                        "",
                        "An error occurred while saving the weekly activity. Please try again later."
                    );
                    return View(weekActivities);
                }
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
