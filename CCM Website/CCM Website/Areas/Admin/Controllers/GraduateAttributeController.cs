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
    public class GraduateAttributeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GraduateAttributeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/GraduateAttribute
        public async Task<IActionResult> Index()
        {
            return View(await _context.GraduateAttributes.ToListAsync());
        }

        // GET: Admin/GraduateAttribute/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var graduateAttribute = await _context.GraduateAttributes.FirstOrDefaultAsync(m =>
                m.AttributeId == id
            );
            if (graduateAttribute == null)
            {
                return NotFound();
            }

            return View(graduateAttribute);
        }

        // GET: Admin/GraduateAttribute/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/GraduateAttribute/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("AttributeId,AttributeName")] GraduateAttribute graduateAttribute
        )
        {
            try
            {
                graduateAttribute.WeekGraduateAttributes = new List<WeekGraduateAttributes>();
                _context.Add(graduateAttribute);
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
                return View(graduateAttribute);
            }
        }

        // GET: Admin/GraduateAttribute/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var graduateAttribute = await _context.GraduateAttributes.FindAsync(id);
            if (graduateAttribute == null)
            {
                return NotFound();
            }
            return View(graduateAttribute);
        }

        // POST: Admin/GraduateAttribute/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("AttributeId,AttributeName")] GraduateAttribute graduateAttribute
        )
        {
            if (id != graduateAttribute.AttributeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(graduateAttribute);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GraduateAttributeExists(graduateAttribute.AttributeId))
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
            return View(graduateAttribute);
        }

        // GET: Admin/GraduateAttribute/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var graduateAttribute = await _context.GraduateAttributes.FirstOrDefaultAsync(m =>
                m.AttributeId == id
            );
            if (graduateAttribute == null)
            {
                return NotFound();
            }

            return View(graduateAttribute);
        }

        // POST: Admin/GraduateAttribute/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var graduateAttribute = await _context.GraduateAttributes.FindAsync(id);
            if (graduateAttribute != null)
            {
                _context.GraduateAttributes.Remove(graduateAttribute);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GraduateAttributeExists(int id)
        {
            return _context.GraduateAttributes.Any(e => e.AttributeId == id);
        }
    }
}
