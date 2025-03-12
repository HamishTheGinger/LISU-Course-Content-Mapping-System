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
    public class TaskProgressStatusController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaskProgressStatusController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/TaskProgressStatus
        public Task<IActionResult> Index(string searchString, int? page)
        {
            var taskProgressStatus = _context.TaskProgressStatus.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                taskProgressStatus = taskProgressStatus.Where(t =>
                    t.StatusName.ToLower().Contains(searchString.ToLower())
                );
            }

            int pageSize = 10;
            int pageNumber = page ?? 1;

            ViewData["SearchString"] = searchString;

            var pagedTaskProgressStatus = taskProgressStatus.ToPagedList(pageNumber, pageSize);
            return Task.FromResult<IActionResult>(View(pagedTaskProgressStatus));
        }

        // GET: Admin/TaskProgressStatus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskProgressStatus = await _context.TaskProgressStatus.FirstOrDefaultAsync(m =>
                m.StatusId == id
            );
            if (taskProgressStatus == null)
            {
                return NotFound();
            }

            return View(taskProgressStatus);
        }

        // GET: Admin/TaskProgressStatus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/TaskProgressStatus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("StatusName")] TaskProgressStatus taskProgressStatus
        )
        {
            if (ModelState.IsValid)
            {
                _context.Add(taskProgressStatus);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(taskProgressStatus);
        }

        // GET: Admin/TaskProgressStatus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskProgressStatus = await _context.TaskProgressStatus.FindAsync(id);
            if (taskProgressStatus == null)
            {
                return NotFound();
            }
            return View(taskProgressStatus);
        }

        // POST: Admin/TaskProgressStatus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("StatusId,StatusName")] TaskProgressStatus taskProgressStatus
        )
        {
            if (id != taskProgressStatus.StatusId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(taskProgressStatus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskProgressStatusExists(taskProgressStatus.StatusId))
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
            return View(taskProgressStatus);
        }

        // GET: Admin/TaskProgressStatus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskProgressStatus = await _context.TaskProgressStatus.FirstOrDefaultAsync(m =>
                m.StatusId == id
            );
            if (taskProgressStatus == null)
            {
                return NotFound();
            }

            return View(taskProgressStatus);
        }

        // POST: Admin/TaskProgressStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var taskProgressStatus = await _context.TaskProgressStatus.FindAsync(id);
            if (taskProgressStatus != null)
            {
                _context.TaskProgressStatus.Remove(taskProgressStatus);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskProgressStatusExists(int id)
        {
            return _context.TaskProgressStatus.Any(e => e.StatusId == id);
        }
    }
}
