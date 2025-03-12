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
    public class WeekGraduateAttributesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WeekGraduateAttributesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: WeekGraduateAttributes
        public Task<IActionResult> Index(string searchString, int? page)
        {
            var weekGraduateAttributes = _context
                .WeekGraduateAttributes.Include(wga => wga.GraduateAttribute)
                .Include(wga => wga.Week)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                weekGraduateAttributes = weekGraduateAttributes.Where(wga =>
                    wga.GraduateAttribute.AttributeName.ToLower().Contains(searchString.ToLower())
                );
            }

            int pageSize = 10;
            int pageNumber = page ?? 1;

            ViewData["SearchString"] = searchString;
            ViewData["DefaultWeekId"] = _context.Weeks.Select(w => w.WeekId).FirstOrDefault();

            var pagedWeekGraduateAttributes = weekGraduateAttributes.ToPagedList(
                pageNumber,
                pageSize
            );

            return Task.FromResult<IActionResult>(View(pagedWeekGraduateAttributes));
        }

        // GET: WeekGraduateAttributes/Create
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

            ViewBag.WeekId = new SelectList(_context.Weeks, "WeekId", "WeekNumber", weekId);
            ViewBag.GraduateAttributeId = new SelectList(
                _context.GraduateAttributes,
                "AttributeId",
                "AttributeName"
            );

            return View();
        }

        // POST: WeekGraduateAttributes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("WeekId,GraduateAttributeId")] WeekGraduateAttributes weekGraduateAttributes
        )
        {
            var week = await _context
                .Weeks.Include(w => w.Workbook)
                .ThenInclude(wb => wb.LearningPlatform)
                .FirstOrDefaultAsync(w => w.WeekId == weekGraduateAttributes.WeekId);

            if (week == null)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(weekGraduateAttributes.Week));
            ModelState.Remove(nameof(weekGraduateAttributes.GraduateAttribute));

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(weekGraduateAttributes);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception exp)
                {
                    Console.WriteLine($"Error creating WeekGraduateAttributes: {exp.Message}");
                    ModelState.AddModelError("", "An error occurred while saving.");
                }
            }
            else
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Validation Error - {state.Key}: {error.ErrorMessage}");
                    }
                }
            }

            ViewBag.WeekId = new SelectList(
                _context.Weeks,
                "WeekId",
                "WeekNumber",
                weekGraduateAttributes.WeekId
            );
            ViewBag.GraduateAttributeId = new SelectList(
                _context.GraduateAttributes,
                "AttributeId",
                "AttributeName",
                weekGraduateAttributes.GraduateAttributeId
            );
            return View(weekGraduateAttributes);
        }

        // GET: WeekGraduateAttributes/Delete
        public async Task<IActionResult> Delete(int weekId, int graduateAttributeId)
        {
            if (weekId == 0 || graduateAttributeId == 0)
            {
                return NotFound();
            }

            var weekGraduateAttributes = await _context
                .WeekGraduateAttributes.Include(wga => wga.GraduateAttribute)
                .Include(wga => wga.Week)
                .FirstOrDefaultAsync(m =>
                    m.WeekId == weekId && m.GraduateAttributeId == graduateAttributeId
                );

            if (weekGraduateAttributes == null)
            {
                return NotFound();
            }

            return View(weekGraduateAttributes);
        }

        // POST: WeekGraduateAttributes/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int weekId, int graduateAttributeId)
        {
            Console.WriteLine(
                $"Attempting to delete: WeekId={weekId}, GraduateAttributeId={graduateAttributeId}"
            );

            var weekGraduateAttributes = await _context
                .WeekGraduateAttributes.Where(wga =>
                    wga.WeekId == weekId && wga.GraduateAttributeId == graduateAttributeId
                )
                .FirstOrDefaultAsync();

            if (weekGraduateAttributes == null)
            {
                Console.WriteLine(
                    $"ERROR: Record with WeekId={weekId} and GraduateAttributeId={graduateAttributeId} NOT FOUND."
                );
                ModelState.AddModelError("", "Error: Unable to find and delete the record.");
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.WeekGraduateAttributes.Remove(weekGraduateAttributes);
                await _context.SaveChangesAsync();
                Console.WriteLine(
                    $"SUCCESS: Deleted record with WeekId={weekId} and GraduateAttributeId={graduateAttributeId}"
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: Failed to delete record - {ex.Message}");
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: WeekGraduateAttributes/Details
        public async Task<IActionResult> Details(int weekId, int graduateAttributeId)
        {
            if (weekId == 0 || graduateAttributeId == 0)
            {
                return NotFound();
            }

            var weekGraduateAttributes = await _context
                .WeekGraduateAttributes.Include(wga => wga.GraduateAttribute)
                .Include(wga => wga.Week)
                .FirstOrDefaultAsync(m =>
                    m.WeekId == weekId && m.GraduateAttributeId == graduateAttributeId
                );

            if (weekGraduateAttributes == null)
            {
                return NotFound();
            }

            return View(weekGraduateAttributes);
        }
    }
}
