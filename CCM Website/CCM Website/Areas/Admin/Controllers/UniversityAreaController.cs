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
    public class UniversityAreaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UniversityAreaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/UniversityArea
        public Task<IActionResult> Index(string searchString, int? page)
        {
            var universityArea = _context.UniversityArea.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                universityArea = universityArea.Where(uA =>
                    uA.AreaName.ToLower().Contains(searchString.ToLower())
                );
            }

            int pageSize = 10;
            int pageNumber = page ?? 1;

            ViewData["SearchString"] = searchString;

            var pagedUniversityArea = universityArea.ToPagedList(pageNumber, pageSize);
            return Task.FromResult<IActionResult>(View(pagedUniversityArea));
        }

        // GET: Admin/UniversityArea/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var universityArea = await _context.UniversityArea.FirstOrDefaultAsync(m =>
                m.AreaId == id
            );
            if (universityArea == null)
            {
                return NotFound();
            }

            return View(universityArea);
        }

        // GET: Admin/UniversityArea/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/UniversityArea/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("AreaId,AreaName")] UniversityArea universityArea
        )
        {
            if (ModelState.IsValid)
            {
                _context.Add(universityArea);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(universityArea);
        }

        // GET: Admin/UniversityArea/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var universityArea = await _context.UniversityArea.FindAsync(id);
            if (universityArea == null)
            {
                return NotFound();
            }
            return View(universityArea);
        }

        // POST: Admin/UniversityArea/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("AreaId,AreaName")] UniversityArea universityArea
        )
        {
            if (id != universityArea.AreaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(universityArea);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UniversityAreaExists(universityArea.AreaId))
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
            return View(universityArea);
        }

        // GET: Admin/UniversityArea/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var universityArea = await _context.UniversityArea.FirstOrDefaultAsync(m =>
                m.AreaId == id
            );
            if (universityArea == null)
            {
                return NotFound();
            }

            return View(universityArea);
        }

        // POST: Admin/UniversityArea/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var universityArea = await _context.UniversityArea.FindAsync(id);
            if (universityArea != null)
            {
                _context.UniversityArea.Remove(universityArea);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UniversityAreaExists(int id)
        {
            return _context.UniversityArea.Any(e => e.AreaId == id);
        }
    }
}
