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
    public class LearningTypeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LearningTypeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/LearningType
        public async Task<IActionResult> Index()
        {
            return View(await _context.LearningType.ToListAsync());
        }

        // GET: Admin/LearningType/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var learningType = await _context.LearningType.FirstOrDefaultAsync(m =>
                m.LearningTypeId == id
            );
            if (learningType == null)
            {
                return NotFound();
            }

            return View(learningType);
        }

        // GET: Admin/LearningType/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/LearningType/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("LearningTypeId,LearningTypeName,LearningTypeColour,LearningTypeTextColour")]
                LearningType learningType
        )
        {
            if (ModelState.IsValid)
            {
                _context.Add(learningType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(learningType);
        }

        // GET: Admin/LearningType/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var learningType = await _context.LearningType.FindAsync(id);
            if (learningType == null)
            {
                return NotFound();
            }
            return View(learningType);
        }

        // POST: Admin/LearningType/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("LearningTypeId,LearningTypeName,LearningTypeColour,LearningTypeTextColour")]
                LearningType learningType
        )
        {
            if (id != learningType.LearningTypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(learningType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LearningTypeExists(learningType.LearningTypeId))
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
            return View(learningType);
        }

        // GET: Admin/LearningType/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var learningType = await _context.LearningType.FirstOrDefaultAsync(m =>
                m.LearningTypeId == id
            );
            if (learningType == null)
            {
                return NotFound();
            }

            return View(learningType);
        }

        // POST: Admin/LearningType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var learningType = await _context.LearningType.FindAsync(id);
            if (learningType != null)
            {
                _context.LearningType.Remove(learningType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LearningTypeExists(int id)
        {
            return _context.LearningType.Any(e => e.LearningTypeId == id);
        }
    }
}
