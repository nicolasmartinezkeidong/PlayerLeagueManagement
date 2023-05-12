using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PlayerManagement.Data;
using PlayerManagement.Models;

namespace PlayerManagement.Controllers
{
    public class FieldsController : Controller
    {
        private readonly PlayerManagementContext _context;

        public FieldsController(PlayerManagementContext context)
        {
            _context = context;
        }

        // GET: Fields
        public async Task<IActionResult> Index(string SearchString, string SearchStringAddress)
        {

            ViewData["Filtering"] = "btn-outline-secondary";
            var fields = from f in _context.Fields
                         .AsNoTracking()
                         select f;

            //Filters
            if (!String.IsNullOrEmpty(SearchString))
            {
                fields = fields.Where(f => f.Name.ToUpper().Contains(SearchString.ToUpper()));
                ViewData["Filtering"] = "btn-danger";
            }
            if (!String.IsNullOrEmpty(SearchStringAddress))
            {
                fields = fields.Where(f => f.Address.ToUpper().Contains(SearchString.ToUpper()));
                ViewData["Filtering"] = "btn-danger";
            }
            return View(await fields.ToListAsync());
        }

        // GET: Fields/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Fields == null)
            {
                return NotFound();
            }

            var @field = await _context.Fields
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@field == null)
            {
                return NotFound();
            }

            return View(@field);
        }

        // GET: Fields/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Fields/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Address,Comments,GoogleMapsLink")] Field @field)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@field);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@field);
        }

        // GET: Fields/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Fields == null)
            {
                return NotFound();
            }

            var @field = await _context.Fields.FindAsync(id);
            if (@field == null)
            {
                return NotFound();
            }
            return View(@field);
        }

        // POST: Fields/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,Comments,GoogleMapsLink")] Field @field)
        {
            if (id != @field.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@field);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FieldExists(@field.Id))
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
            return View(@field);
        }

        // GET: Fields/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Fields == null)
            {
                return NotFound();
            }

            var @field = await _context.Fields
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@field == null)
            {
                return NotFound();
            }

            return View(@field);
        }

        // POST: Fields/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Fields == null)
            {
                return Problem("Entity set 'PlayerManagementContext.Fields'  is null.");
            }
            var @field = await _context.Fields.FindAsync(id);
            if (@field != null)
            {
                _context.Fields.Remove(@field);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FieldExists(int id)
        {
          return _context.Fields.Any(e => e.Id == id);
        }
    }
}
