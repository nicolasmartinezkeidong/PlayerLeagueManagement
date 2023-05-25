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
    public class TeamDocumentsController : Controller
    {
        private readonly PlayerManagementContext _context;

        public TeamDocumentsController(PlayerManagementContext context)
        {
            _context = context;
        }

        // GET: TeamDocuments
        public async Task<IActionResult> Index()
        {
            var playerManagementContext = _context.TeamDocuments.Include(t => t.Team);
            return View(await playerManagementContext.ToListAsync());
        }

        // GET: TeamDocuments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TeamDocuments == null)
            {
                return NotFound();
            }

            var teamDocument = await _context.TeamDocuments
                .Include(t => t.Team)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teamDocument == null)
            {
                return NotFound();
            }

            return View(teamDocument);
        }

        // GET: TeamDocuments/Create
        public IActionResult Create()
        {
            ViewData["TeamId"] = new SelectList(_context.Teams, "Id", "Name");
            return View();
        }

        // POST: TeamDocuments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TeamId,Id,FileName,MimeType")] TeamDocument teamDocument)
        {
            if (ModelState.IsValid)
            {
                _context.Add(teamDocument);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TeamId"] = new SelectList(_context.Teams, "Id", "Name", teamDocument.TeamId);
            return View(teamDocument);
        }

        // GET: TeamDocuments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TeamDocuments == null)
            {
                return NotFound();
            }

            var teamDocument = await _context.TeamDocuments.FindAsync(id);
            if (teamDocument == null)
            {
                return NotFound();
            }
            ViewData["TeamId"] = new SelectList(_context.Teams, "Id", "Name", teamDocument.TeamId);
            return View(teamDocument);
        }

        // POST: TeamDocuments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TeamId,Id,FileName,MimeType")] TeamDocument teamDocument)
        {
            if (id != teamDocument.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(teamDocument);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeamDocumentExists(teamDocument.Id))
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
            ViewData["TeamId"] = new SelectList(_context.Teams, "Id", "Name", teamDocument.TeamId);
            return View(teamDocument);
        }

        // GET: TeamDocuments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TeamDocuments == null)
            {
                return NotFound();
            }

            var teamDocument = await _context.TeamDocuments
                .Include(t => t.Team)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teamDocument == null)
            {
                return NotFound();
            }

            return View(teamDocument);
        }

        // POST: TeamDocuments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TeamDocuments == null)
            {
                return Problem("Entity set 'PlayerManagementContext.TeamDocuments'  is null.");
            }
            var teamDocument = await _context.TeamDocuments.FindAsync(id);
            if (teamDocument != null)
            {
                _context.TeamDocuments.Remove(teamDocument);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeamDocumentExists(int id)
        {
          return _context.TeamDocuments.Any(e => e.Id == id);
        }
    }
}
