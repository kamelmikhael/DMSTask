using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DMSTask.Data;
using DMSTask.Models;
using Microsoft.AspNetCore.Authorization;
using DMSTask.Consts;

namespace DMSTask.Controllers
{
    [Authorize(Roles = DmsRoles.ADMIN)]
    public class UnitOfMeasuresController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UnitOfMeasuresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UnitOfMeasures
        public async Task<IActionResult> Index(string filter)
        {
            ViewData["Filter"] = filter;

            var data = _context.UnitOfMeasures.AsQueryable();

            if(!string.IsNullOrWhiteSpace(filter))
            {
                data = data.Where(x => 
                    x.Description!.Contains(filter) ||
                    x.UOM!.Contains(filter)
                );
            }

            return View(await data.ToListAsync());
        }

        // GET: UnitOfMeasures/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var unitOfMeasure = await _context.UnitOfMeasures
                .FirstOrDefaultAsync(m => m.Id == id);
            if (unitOfMeasure == null)
            {
                return NotFound();
            }

            return View(unitOfMeasure);
        }

        // GET: UnitOfMeasures/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UnitOfMeasures/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UOM,Description")] UnitOfMeasure unitOfMeasure)
        {
            if (ModelState.IsValid)
            {
                _context.Add(unitOfMeasure);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(unitOfMeasure);
        }

        // GET: UnitOfMeasures/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var unitOfMeasure = await _context.UnitOfMeasures.FindAsync(id);
            if (unitOfMeasure == null)
            {
                return NotFound();
            }
            return View(unitOfMeasure);
        }

        // POST: UnitOfMeasures/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UOM,Description")] UnitOfMeasure unitOfMeasure)
        {
            if (id != unitOfMeasure.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(unitOfMeasure);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UnitOfMeasureExists(unitOfMeasure.Id))
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
            return View(unitOfMeasure);
        }

        // GET: UnitOfMeasures/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var unitOfMeasure = await _context.UnitOfMeasures
                .FirstOrDefaultAsync(m => m.Id == id);
            if (unitOfMeasure == null)
            {
                return NotFound();
            }

            return View(unitOfMeasure);
        }

        // POST: UnitOfMeasures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var unitOfMeasure = await _context.UnitOfMeasures.FindAsync(id);
            _context.UnitOfMeasures.Remove(unitOfMeasure);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UnitOfMeasureExists(int id)
        {
            return _context.UnitOfMeasures.Any(e => e.Id == id);
        }
    }
}
