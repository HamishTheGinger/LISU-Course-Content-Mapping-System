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
    public class TaskApproachController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaskApproachController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/TaskApproach
        public async Task<IActionResult> Index()
        {
            return View(await _context.TaskApproach.ToListAsync());
        }

        // GET: Admin/TaskApproach/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskApproach = await _context.TaskApproach.FirstOrDefaultAsync(m =>
                m.ApproachId == id
            );
            if (taskApproach == null)
            {
                return NotFound();
            }

            return View(taskApproach);
        }

        // GET: Admin/TaskApproach/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/TaskApproach/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("ApproachId,ApproachName")] TaskApproach taskApproach
        )
        {
            if (ModelState.IsValid)
            {
                _context.Add(taskApproach);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(taskApproach);
        }

        // GET: Admin/TaskApproach/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskApproach = await _context.TaskApproach.FindAsync(id);
            if (taskApproach == null)
            {
                return NotFound();
            }
            return View(taskApproach);
        }

        // POST: Admin/TaskApproach/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("ApproachId,ApproachName")] TaskApproach taskApproach
        )
        {
            if (id != taskApproach.ApproachId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(taskApproach);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskApproachExists(taskApproach.ApproachId))
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
            return View(taskApproach);
        }

        // GET: Admin/TaskApproach/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskApproach = await _context.TaskApproach.FirstOrDefaultAsync(m =>
                m.ApproachId == id
            );
            if (taskApproach == null)
            {
                return NotFound();
            }

            return View(taskApproach);
        }

        // POST: Admin/TaskApproach/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var taskApproach = await _context.TaskApproach.FindAsync(id);
            if (taskApproach != null)
            {
                _context.TaskApproach.Remove(taskApproach);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskApproachExists(int id)
        {
            return _context.TaskApproach.Any(e => e.ApproachId == id);
        }
    }
}
