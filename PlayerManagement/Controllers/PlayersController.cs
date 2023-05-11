using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PlayerManagement.Data;
using PlayerManagement.Models;

namespace PlayerManagement.Controllers
{
    public class PlayersController : Controller
    {
        private readonly PlayerManagementContext _context;

        public PlayersController(PlayerManagementContext context)
        {
            _context = context;
        }

        // GET: Players
        public async Task<IActionResult> Index()
        {
            var playerManagementContext = _context.Players.Include(p => p.PlayerPosition).Include(p => p.Team);
            return View(await playerManagementContext.ToListAsync());
        }

        // GET: Players/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Players == null)
            {
                return NotFound();
            }

            var player = await _context.Players
                .Include(p => p.PlayerPosition)
                .Include(p => p.Team)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (player == null)
            {
                return NotFound();
            }

            return View(player);
        }

        // GET: Players/Create
        public IActionResult Create()
        {
            ViewData["PlayerPositionId"] = new SelectList(_context.PlayerPositions, "Id", "PlayerPos");
            PopulateDropDownLists();
            return View();
        }

        // POST: Players/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Phone,Email,DOB,PlayerPositionId,TeamId")] Player player)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(player);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Players.Email"))
                {
                    ModelState.AddModelError("Email", "Unable to save changes. Remember, you cannot have duplicate emails.");
                }
                else if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Players.Phone"))
                {
                    ModelState.AddModelError("Phone", "Unable to save changes. Remember, you cannot have duplicate phones.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            ViewData["PlayerPositionId"] = new SelectList(_context.PlayerPositions, "Id", "PlayerPos", player.PlayerPositionId);
            PopulateDropDownLists(player);
            return View(player);
        }

        // GET: Players/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Players == null)
            {
                return NotFound();
            }

            var player = await _context.Players.FindAsync(id);
            if (player == null)
            {
                return NotFound();
            }
            ViewData["PlayerPositionId"] = new SelectList(_context.PlayerPositions, "Id", "PlayerPos", player.PlayerPositionId);
            PopulateDropDownLists(player);
            return View(player);
        }

        // POST: Players/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {

            //Go get the patient to update
            var playerToUpdate = await _context.Players.FirstOrDefaultAsync(p => p.Id == id);

            if (playerToUpdate == null)
            {
                return NotFound();
            }

            //Try updating it with the values posted
            if (await TryUpdateModelAsync<Player>(playerToUpdate, "",
                p => p.FirstName, p => p.LastName, p => p.Phone, p => p.Email, p => p.DOB,
                p => p.TeamId, p => p.PlayerPositionId))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlayerExists(playerToUpdate.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException dex)
                {
                    if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Players.Email"))
                    {
                        ModelState.AddModelError("Email", "Unable to save changes. Remember, you cannot have duplicate emails.");
                    }
                    else if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Players.Phone"))
                    {
                        ModelState.AddModelError("Phone", "Unable to save changes. Remember, you cannot have duplicate phones.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                    }
                }
            }
                ViewData["PlayerPositionId"] = new SelectList(_context.PlayerPositions, "Id", "PlayerPos", playerToUpdate.PlayerPositionId);
                PopulateDropDownLists(playerToUpdate);
                return View(playerToUpdate);
        }
        // GET: Players/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var player = await _context.Players
                .Include(p => p.Team)
                .Include(p => p.PlayerPosition)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (player == null)
            {
                return NotFound();
            }

            return View(player);
        }

        // POST: Players/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Players == null)
            {
                return Problem("Entity set 'PlayerManagementContext.Players'  is null.");
            }
            var player = await _context.Players.FindAsync(id);
            try
            {
                if (player != null)
                {
                    _context.Players.Remove(player);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to delete record. Try again, and if the problem persists contact your system administrator.");
            }
            return View(player);
        }


        private void PopulateDropDownLists(Player player = null)
        {
            var tQuery = from t in _context.Teams
                         orderby t.Name
                         select t;
            ViewData["TeamId"] = new SelectList(tQuery, "Id", "Name", player?.TeamId);
        }


        private bool PlayerExists(int id)
        {
          return _context.Players.Any(e => e.Id == id);
        }
    }
}
