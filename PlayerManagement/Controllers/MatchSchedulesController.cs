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
    public class MatchSchedulesController : Controller
    {
        private readonly PlayerManagementContext _context;

        public MatchSchedulesController(PlayerManagementContext context)
        {
            _context = context;
        }

        // GET: MatchSchedules
        public async Task<IActionResult> Index()
        {
            var playerManagementContext = _context.MatchSchedules.Include(m => m.AwayTeam).Include(m => m.Field).Include(m => m.HomeTeam);
            return View(await playerManagementContext.ToListAsync());
        }

        // GET: MatchSchedules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MatchSchedules == null)
            {
                return NotFound();
            }

            var matchSchedule = await _context.MatchSchedules
                .Include(m => m.AwayTeam)
                .Include(m => m.Field)
                .Include(m => m.HomeTeam)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (matchSchedule == null)
            {
                return NotFound();
            }

            return View(matchSchedule);
        }

        // GET: MatchSchedules/Create
        public IActionResult Create()
        {
            PopulateDropDownLists();
            return View();
        }

        // POST: MatchSchedules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,Time,FieldId,HomeTeamId,AwayTeamId,HomeTeamScore,AwayTeamScore")] MatchSchedule matchSchedule)
        {
            if (ModelState.IsValid)
            {
                _context.Add(matchSchedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateDropDownLists(matchSchedule);
            return View(matchSchedule);
        }

        // GET: MatchSchedules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.MatchSchedules == null)
            {
                return NotFound();
            }

            var matchSchedule = await _context.MatchSchedules.FindAsync(id);
            if (matchSchedule == null)
            {
                return NotFound();
            }

            PopulateDropDownLists(matchSchedule);
            return View(matchSchedule);
        }

        // POST: MatchSchedules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Date,Time,FieldId,HomeTeamId,AwayTeamId,HomeTeamScore,AwayTeamScore")] MatchSchedule matchSchedule)
        {
            if (id != matchSchedule.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(matchSchedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MatchScheduleExists(matchSchedule.Id))
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
            PopulateDropDownLists(matchSchedule);
            return View(matchSchedule);
        }

        // GET: MatchSchedules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MatchSchedules == null)
            {
                return NotFound();
            }

            var matchSchedule = await _context.MatchSchedules
                .Include(m => m.AwayTeam)
                .Include(m => m.Field)
                .Include(m => m.HomeTeam)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (matchSchedule == null)
            {
                return NotFound();
            }

            return View(matchSchedule);
        }

        // POST: MatchSchedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.MatchSchedules == null)
            {
                return Problem("Entity set 'PlayerManagementContext.MatchSchedules'  is null.");
            }
            var matchSchedule = await _context.MatchSchedules.FindAsync(id);
            if (matchSchedule != null)
            {
                _context.MatchSchedules.Remove(matchSchedule);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private void PopulateDropDownLists(MatchSchedule matchSchedule = null)
        {
            var fQuery = from f in _context.Fields
                         orderby f.Name
                         select f;
            ViewData["FieldId"] = new SelectList(fQuery, "Id", "Name", matchSchedule?.FieldId);

            //Home team
            var htQuery = from t in _context.Teams
                         orderby t.Name
                         select t;
            ViewData["TeamId"] = new SelectList(htQuery, "Id", "Name", matchSchedule?.HomeTeamId);

            //Away team
            var atQuery = from t in _context.Teams
                          orderby t.Name
                          select t;
            ViewData["TeamId"] = new SelectList(htQuery, "Id", "Name", matchSchedule?.AwayTeamId);
        }

        private bool MatchScheduleExists(int id)
        {
          return _context.MatchSchedules.Any(e => e.Id == id);
        }
    }
}
