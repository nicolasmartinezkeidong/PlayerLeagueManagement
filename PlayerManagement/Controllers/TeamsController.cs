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
using Team = PlayerManagement.Models.Team;

namespace PlayerManagement.Controllers
{
    public class TeamsController : Controller
    {
        private readonly PlayerManagementContext _context;

        public TeamsController(PlayerManagementContext context)
        {
            _context = context;
        }

        // GET: Teams
        public async Task<IActionResult> Index(int? page,int? pageSizeID, string SearchString, int? LeagueId,
            string actionButton, string sortDirection = "asc", string sortField = "Team")
        {
            //Clear the sort/filter/paging URL Cookie for Controller
            CookieHelper.CookieSet(HttpContext, ControllerName() + "URL", "", -1);

            //Toggle the Open/Closed state of the collapse depending on if we are filtering
            ViewData["Filtering"] = "btn-outline-secondary"; //Assume not filtering

            PopulateDropDownLists();

            string[] sortOptions = new[] { "Team", "RegistrationDate", "League" };

            var teams = from t in _context.Teams
                .Include(p => p.League)
                .Include(p => p.Players)
                .AsNoTracking()
                        select t;

            #region Filters
            //filters
            if (LeagueId.HasValue)
            {
                teams = teams.Where(t => t.LeagueId == LeagueId);
                ViewData["Filtering"] = "btn-danger";
            }
            if (!String.IsNullOrEmpty(SearchString))
            {

                teams = teams.Where(t => t.Name.ToUpper().Contains(SearchString.ToUpper()));
                ViewData["Filtering"] = "btn-danger";
            }
            #endregion

            #region Sorting
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

            if (sortField == "RegistrationDate")
            {
                if (sortDirection == "asc")
                {
                    teams = teams
                        .OrderBy(p => p.RegistrationDate);
                }
                else
                {
                    teams = teams
                        .OrderByDescending(p => p.RegistrationDate);
                }
            }
            else if (sortField == "League")
            {
                if (sortDirection == "asc")
                {
                    teams = teams
                        .OrderBy(p => p.League.Name);
                }
                else
                {
                    teams = teams
                        .OrderByDescending(p => p.League.Name);
                }
            }
            else //Sorting by Team Name
            {
                if (sortDirection == "asc")
                {
                    teams = teams
                        .OrderBy(p => p.Name);
                }
                else
                {
                    teams = teams
                        .OrderByDescending(p => p.Name);
                }
            }
            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            #endregion

            //Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, "TeamsController");
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Team>.CreateAsync(teams.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        // GET: Teams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            if (id == null || _context.Teams == null)
            {
                return NotFound();
            }

            var team = await _context.Teams
                .Include(t => t.League)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // GET: Teams/Create
        public IActionResult Create()
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            PopulateDropDownLists();
            return View();
        }

        // POST: Teams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,RegistrationDate,LeagueId")] Team team)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(team);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch(DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists contact your system administrator.");
            }
            
            return View(team);
        }

        // GET: Teams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams.FindAsync(id);
            if (team == null)
            {
                return NotFound();
            }
            PopulateDropDownLists(team);
            return View(team);
        }

        // POST: Teams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            //Go get the Team to update
            var teamToUpdate = await _context.Teams.SingleOrDefaultAsync(t => t.Id == id);

            if (teamToUpdate == null)
            {
                return NotFound();
            }

            //Try updating it with the values posted
            if (await TryUpdateModelAsync<Team>(teamToUpdate, "",
                t => t.Name, t => t.RegistrationDate, t => t.LeagueId))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeamExists(teamToUpdate.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists contact your system administrator.");
                }

            }
            return View(teamToUpdate);
        }


        // GET: Teams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // POST: Teams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            if (_context.Teams == null)
            {
                return Problem("Entity set 'PlayerManagementContext.Teams'  is null.");
            }
            var team = await _context.Teams.FindAsync(id);
            try
            {
                if (team != null)
                {
                    _context.Teams.Remove(team);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
                {
                    ModelState.AddModelError("", "Unable to Delete Team. Remember, you cannot delete a Team that has players assigned.");
                }
                else if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: Teams.Name"))
                {
                    ModelState.AddModelError("Name", "Unable to save changes. Remember, you cannot have duplicate Team names.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            return View(team);

        }

        private string ControllerName()
        {
            return this.ControllerContext.RouteData.Values["controller"].ToString();
        }
        private void ViewDataReturnURL()
        {
            ViewData["returnURL"] = MaintainURL.ReturnURL(HttpContext, ControllerName());
        }

        // Order ddl and orderer it by Name
        private void PopulateDropDownLists(Team team = null)
        {
            var lQuery = from l in _context.Leagues
                         orderby l.Name
                         select l;
            ViewData["LeagueId"] = new SelectList(lQuery, "Id", "Name", team?.LeagueId);
        }


        private bool TeamExists(int id)
        {
          return _context.Teams.Any(e => e.Id == id);
        }
    }
}
