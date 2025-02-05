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
    public class LearningPlatformController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LearningPlatformController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/LearningPlatform
        public async Task<IActionResult> Index()
        {
            return View(await _context.LearningPlatforms.ToListAsync());
        }

        // GET: Admin/LearningPlatform/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var learningPlatform = await _context.LearningPlatforms.FirstOrDefaultAsync(m =>
                m.PlatformId == id
            );
            if (learningPlatform == null)
            {
                return NotFound();
            }

            return View(learningPlatform);
        }

        // GET: Admin/LearningPlatform/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/LearningPlatform/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("PlatformId,PlatformName")] LearningPlatform learningPlatform
        )
        {
            try
            {
                learningPlatform.Workbooks = new List<Workbook>();
                learningPlatform.LearningPlatformActivities =
                    new List<LearningPlatformActivities>();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Model Creation Error: {e.Message}");
                ModelState.AddModelError(
                    "",
                    "An error occurred while saving the workbook. Please try again later."
                );
                return View(learningPlatform);
            }

            ModelState.Remove(nameof(learningPlatform.Workbooks));
            ModelState.Remove(nameof(learningPlatform.LearningPlatformActivities));

            if (!ModelState.IsValid)
            {
                Console.WriteLine("Error creating model");
                return View(learningPlatform);
            }

            try
            {
                _context.Add(learningPlatform);
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
                return View(learningPlatform);
            }
        }

        // GET: Admin/LearningPlatform/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var learningPlatform = await _context.LearningPlatforms.FindAsync(id);
            if (learningPlatform == null)
            {
                return NotFound();
            }
            return View(learningPlatform);
        }

        // POST: Admin/LearningPlatform/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("PlatformId,PlatformName")] LearningPlatform learningPlatform
        )
        {
            if (id != learningPlatform.PlatformId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(learningPlatform);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LearningPlatformExists(learningPlatform.PlatformId))
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
            return View(learningPlatform);
        }

        // GET: Admin/LearningPlatform/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var learningPlatform = await _context.LearningPlatforms.FirstOrDefaultAsync(m =>
                m.PlatformId == id
            );
            if (learningPlatform == null)
            {
                return NotFound();
            }

            return View(learningPlatform);
        }

        // POST: Admin/LearningPlatform/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var learningPlatform = await _context.LearningPlatforms.FindAsync(id);
            if (learningPlatform != null)
            {
                _context.LearningPlatforms.Remove(learningPlatform);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LearningPlatformExists(int id)
        {
            return _context.LearningPlatforms.Any(e => e.PlatformId == id);
        }
    }
}
