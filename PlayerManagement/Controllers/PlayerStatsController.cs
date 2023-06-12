using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PlayerManagement.CustomControllers;
using PlayerManagement.Data;
using PlayerManagement.Models;
using PlayerManagement.Utilities;

namespace PlayerManagement.Controllers
{
    public class PlayerStatsController : CognizantController
    {
        private readonly PlayerManagementContext _context;

        public PlayerStatsController(PlayerManagementContext context)
        {
            _context = context;
        }

        // GET: PlayerStats
        public async Task<IActionResult> Index()
        {
            var playerManagementContext = _context.PlayerMatchs.Include(p => p.Match).Include(p => p.Player);
            return View(await playerManagementContext.ToListAsync());
        }

        // GET: PlayerStats/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.PlayerMatchs == null)
            {
                return NotFound();
            }

            var playerMatch = await _context.PlayerMatchs
                .Include(p => p.Match)
                .Include(p => p.Player)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (playerMatch == null)
            {
                return NotFound();
            }

            return View(playerMatch);
        }

        // GET: PlayerStats/Create
        public IActionResult Create()
        {
            ViewData["MatchId"] = new SelectList(_context.MatchSchedules, "Id", "Time");
            ViewData["PlayerId"] = new SelectList(_context.Players, "Id", "Email");
            return View();
        }

        // POST: PlayerStats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Goals,RedCards,YellowCards,MatchId,PlayerId")] PlayerMatch playerMatch)
        {
            if (ModelState.IsValid)
            {
                _context.Add(playerMatch);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MatchId"] = new SelectList(_context.MatchSchedules, "Id", "Time", playerMatch.MatchId);
            ViewData["PlayerId"] = new SelectList(_context.Players, "Id", "Email", playerMatch.PlayerId);
            return View(playerMatch);
        }

        // GET: PlayerStats/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.PlayerMatchs == null)
            {
                return NotFound();
            }

            var playerMatch = await _context.PlayerMatchs.FindAsync(id);
            if (playerMatch == null)
            {
                return NotFound();
            }
            ViewData["MatchId"] = new SelectList(_context.MatchSchedules, "Id", "Time", playerMatch.MatchId);
            ViewData["PlayerId"] = new SelectList(_context.Players, "Id", "Email", playerMatch.PlayerId);
            return View(playerMatch);
        }

        // POST: PlayerStats/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Goals,RedCards,YellowCards,MatchId,PlayerId")] PlayerMatch playerMatch)
        {
            if (id != playerMatch.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(playerMatch);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlayerMatchExists(playerMatch.Id))
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
            ViewData["MatchId"] = new SelectList(_context.MatchSchedules, "Id", "Time", playerMatch.MatchId);
            ViewData["PlayerId"] = new SelectList(_context.Players, "Id", "Email", playerMatch.PlayerId);
            return View(playerMatch);
        }

        // GET: PlayerStats/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.PlayerMatchs == null)
            {
                return NotFound();
            }

            var playerMatch = await _context.PlayerMatchs
                .Include(p => p.Match)
                .Include(p => p.Player)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (playerMatch == null)
            {
                return NotFound();
            }

            return View(playerMatch);
        }

        // POST: PlayerStats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.PlayerMatchs == null)
            {
                return Problem("Entity set 'PlayerManagementContext.PlayerMatchs'  is null.");
            }
            var playerMatch = await _context.PlayerMatchs.FindAsync(id);
            if (playerMatch != null)
            {
                _context.PlayerMatchs.Remove(playerMatch);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private SelectList MatchSchedulesSelectList(int? id)
        {
            var dQuery = from d in _context.MatchSchedules
                         orderby d.Summary
                         select d;
            return new SelectList(dQuery, "Id", "Summary", id);
        }
        private void PopulateDropDownLists(PlayerMatch playerMatch = null)
        {
            ViewData["MatchId"] = MatchSchedulesSelectList(playerMatch?.MatchId);
        }
        private void ViewDataReturnURL()
        {
            ViewData["returnURL"] = MaintainURL.ReturnURL(HttpContext, ControllerName());
        }

        private bool PlayerMatchExists(int id)
        {
          return _context.PlayerMatchs.Any(e => e.Id == id);
        }
    }
}
