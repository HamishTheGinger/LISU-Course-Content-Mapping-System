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
    public class TaskLocationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaskLocationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/TaskLocation
        public Task<IActionResult> Index(string searchString, int? page)
        {
            var taskLocation = _context.TaskLocation.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                taskLocation = taskLocation.Where(t =>
                    t.LocationName.ToLower().Contains(searchString.ToLower())
                );
            }

            int pageSize = 10;
            int pageNumber = page ?? 1;

            ViewData["SearchString"] = searchString;

            var pagedTaskLocation = taskLocation.ToPagedList(pageNumber, pageSize);
            return Task.FromResult<IActionResult>(View(pagedTaskLocation));
        }

        // GET: Admin/TaskLocation/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskLocation = await _context.TaskLocation.FirstOrDefaultAsync(m =>
                m.LocationId == id
            );
            if (taskLocation == null)
            {
                return NotFound();
            }

            return View(taskLocation);
        }

        // GET: Admin/TaskLocation/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/TaskLocation/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LocationName")] TaskLocation taskLocation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(taskLocation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(taskLocation);
        }

        // GET: Admin/TaskLocation/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskLocation = await _context.TaskLocation.FindAsync(id);
            if (taskLocation == null)
            {
                return NotFound();
            }
            return View(taskLocation);
        }

        // POST: Admin/TaskLocation/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("LocationId,LocationName")] TaskLocation taskLocation
        )
        {
            if (id != taskLocation.LocationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(taskLocation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskLocationExists(taskLocation.LocationId))
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
            return View(taskLocation);
        }

        // GET: Admin/TaskLocation/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskLocation = await _context.TaskLocation.FirstOrDefaultAsync(m =>
                m.LocationId == id
            );
            if (taskLocation == null)
            {
                return NotFound();
            }

            return View(taskLocation);
        }

        // POST: Admin/TaskLocation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var taskLocation = await _context.TaskLocation.FindAsync(id);
            if (taskLocation != null)
            {
                _context.TaskLocation.Remove(taskLocation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskLocationExists(int id)
        {
            return _context.TaskLocation.Any(e => e.LocationId == id);
        }
    }
}
