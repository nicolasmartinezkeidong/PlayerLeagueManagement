using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PlayerManagement.CustomControllers;
using PlayerManagement.Data;
using PlayerManagement.Models;
using PlayerManagement.Utilities;

namespace PlayerManagement.Controllers
{
    [Authorize(Roles = "Admin, Captain")]
    public class PlayerStatsController : CognizantController
    {
        private readonly PlayerManagementContext _context;

        public PlayerStatsController(PlayerManagementContext context)
        {
            _context = context;
        }

        // GET: PlayerStats
        public async Task<IActionResult> Index(int? page, int? pageSizeID, int? MatchId, string actionButton,
            string SearchString, string sortDirection = "desc", string sortField = "Match")
        {
            //Clear the sort/filter/paging URL Cookie for Controller
            CookieHelper.CookieSet(HttpContext, ControllerName() + "URL", "", -1);

            PopulateDropDownLists();

            //Toggle the Open/Closed state of the collapse depending on if we are filtering
            ViewData["Filtering"] = "btn-outline-dark"; //Asume not filtering
            //Then in each "test" for filtering, add ViewData["Filtering"] = "btn-danger" if true;

            //NOTE: make sure this array has matching values to the column headings
            string[] sortOptions = new[] { "Match", "Player"};

            var stats = from s in _context.PlayerMatchs
                .Include(p => p.Match)
                .Include(p => p.Player)
                select s;
            
            if (MatchId.HasValue)
            {
                stats = stats.Where(p => p.MatchId == MatchId);
                ViewData["Filtering"] = "btn-danger";
            }
            if (!String.IsNullOrEmpty(SearchString))
            {
                stats = stats.Where(p => p.Player.LastName.ToUpper().Contains(SearchString.ToUpper())
                                       || p.Player.FirstName.ToUpper().Contains(SearchString.ToUpper()));
                ViewData["Filtering"] = "btn-danger";
            }
            //Before we sort, see if we have called for a change of filtering or sorting
            if (!String.IsNullOrEmpty(actionButton)) //Form Submitted so lets sort!
            {
                page = 1;//Reset back to first page when sorting or filtering

                if (sortOptions.Contains(actionButton))//Change of sort is requested
                {
                    if (actionButton == sortField) //Reverse order on same field
                    {
                        sortDirection = sortDirection == "asc" ? "desc" : "asc";
                    }
                    sortField = actionButton;//Sort by the button clicked
                }
            }
            //Now we know which field and direction to sort by.
            if (sortField == "Match")
            {
                if (sortDirection == "asc")
                {
                    stats = stats
                        .OrderBy(p => p.Match.Id);
                }
                else
                {
                    stats = stats
                        .OrderByDescending(p => p.Match.Id);
                }
            }
            else //Player
            {
                if (sortDirection == "asc")
                {
                    stats = stats
                        .OrderByDescending(p => p.Player.FirstName);
                }
                else
                {
                    stats = stats
                        .OrderBy(p => p.Player.FirstName);
                }
            }
            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;


            //Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);

            var pagedData = await PaginatedList<PlayerMatch>.CreateAsync(stats.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
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

            var playerMatch = await _context.PlayerMatchs
                .FindAsync(id);
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
                         orderby d.Id
                         select d;
            return new SelectList(dQuery, "Id", "MatchDay", id);
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
