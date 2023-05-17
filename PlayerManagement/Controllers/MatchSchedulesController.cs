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
using PlayerManagement.Utilities;

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
        public async Task<IActionResult> Index(int? page, int? HomeTeamId, int? AwayTeamId, int? FieldId, 
            DateTime? startDate, DateTime? endDate, string actionButton, string sortDirection = "asc", 
            string sortField = "Field")
        {
            ViewData["Filtering"] = "btn-outline-secondary";

            PopulateDropDownLists();

            string[] sortOptions = new[] { "Date", "Time", "Field", "HomeTeam", "AwayTeam" };

            var matchSchedules = from m in _context.MatchSchedules
                .Include(m => m.AwayTeam)
                .Include(m => m.Field)
                .Include(m => m.HomeTeam)
                .AsNoTracking()
                select m;

            #region Filters
            //Filters
            if (HomeTeamId.HasValue)
            {
                matchSchedules = matchSchedules.Where(t => t.HomeTeamId == HomeTeamId);
                ViewData["Filtering"] = "btn-danger";
            }
            if (AwayTeamId.HasValue)
            {
                matchSchedules = matchSchedules.Where(t => t.AwayTeamId == AwayTeamId);
                ViewData["Filtering"] = "btn-danger";
            }
            if (FieldId.HasValue)
            {
                matchSchedules = matchSchedules.Where(f => f.FieldId == FieldId);
                ViewData["Filtering"] = "btn-danger";
            }
            if (startDate.HasValue && endDate.HasValue)
            {
                matchSchedules = matchSchedules.Where(d => d.Date >= startDate && d.Date <= endDate);
                ViewData["Filtering"] = "btn-danger";
            }
            //Before we sort, see if we have called for a change of filtering or sorting
            if (!String.IsNullOrEmpty(actionButton)) //Form Submitted!
            {
                page = 1;
                if (sortOptions.Contains(actionButton))//Change of sort is requested
                {
                    if (actionButton == sortField) //Reverse order on same field
                    {
                        sortDirection = sortDirection == "asc" ? "desc" : "asc";
                    }
                    sortField = actionButton;//Sort by the button clicked
                }
            }
            #endregion

            #region Sorting
            //Now we know which field and direction to sort by
            if (sortField == "Time")
            {
                if (sortDirection == "asc")
                {
                    matchSchedules = matchSchedules.OrderBy(p => p.Time);
                }
                else
                {
                    matchSchedules = matchSchedules.OrderByDescending(p => p.Time);
                }
            }
            else if (sortField == "Date")
            {
                if (sortDirection == "asc")
                {
                    matchSchedules = matchSchedules.OrderBy(p => p.Date);
                }
                else
                {
                    matchSchedules = matchSchedules.OrderByDescending(p => p.Date);
                }
            }
            else if (sortField == "HomeTeam")
            {
                if (sortDirection == "asc")
                {
                    matchSchedules = matchSchedules.OrderBy(p => p.HomeTeam.Name);

                }
                else
                {
                    matchSchedules = matchSchedules.OrderByDescending(p => p.HomeTeam.Name);
                }
            }
            else if (sortField == "AwayTeam")
            {
                if (sortDirection == "asc")
                {
                    matchSchedules = matchSchedules.OrderBy(p => p.AwayTeam.Name);

                }
                else
                {
                    matchSchedules = matchSchedules.OrderByDescending(p => p.AwayTeam.Name);
                }
            }
            else 
            {
                if (sortDirection == "asc")
                {
                    matchSchedules = matchSchedules.OrderBy(p => p.Field.Name);
                }
                else
                {
                    matchSchedules = matchSchedules.OrderByDescending(p => p.Field.Name);
                }
            }
            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            #endregion

            int pageSize = 10;//Change as required
            var pagedData = await PaginatedList<MatchSchedule>.CreateAsync(matchSchedules.AsNoTracking(), page ?? 1, pageSize);
            return View(pagedData);
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
                .AsNoTracking()
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
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(matchSchedule);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch(DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            PopulateDropDownLists(matchSchedule);
            return View(matchSchedule);
        }

        // GET: MatchSchedules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
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
            if (id == null)
            {
                return NotFound();
            }

            var matchSchedule = await _context.MatchSchedules
                .Include(m => m.AwayTeam)
                .Include(m => m.Field)
                .Include(m => m.HomeTeam)
                .AsNoTracking()
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
            try
            {
                if (matchSchedule != null)
                {
                    _context.MatchSchedules.Remove(matchSchedule);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to delete record. Try again, and if the problem persists see your system administrator.");
            }
            return View(matchSchedule);
        }

        private void PopulateDropDownLists(MatchSchedule matchSchedule = null)
        {
            //Home team
            var msQuery = from ms in _context.Teams
                         orderby ms.Name
                         select ms;
            ViewData["HomeTeamId"] = new SelectList(msQuery, "Id", "Name", matchSchedule?.HomeTeamId);

            //Away team
            var tQuery = from ms in _context.Teams
                          orderby ms.Name
                          select ms;
            ViewData["AwayTeamId"] = new SelectList(tQuery, "Id", "Name", matchSchedule?.AwayTeamId);

            //Field
            var fQuery = from ms in _context.Fields
                         orderby ms.Name
                         select ms;
            ViewData["FieldId"] = new SelectList(fQuery, "Id", "Name", matchSchedule?.FieldId);
        }


        private bool MatchScheduleExists(int id)
        {
          return _context.MatchSchedules.Any(e => e.Id == id);
        }
    }
}
