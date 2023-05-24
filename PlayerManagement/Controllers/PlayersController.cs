using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PlayerManagement.CustomControllers;
using PlayerManagement.Data;
using PlayerManagement.Models;
using PlayerManagement.Utilities;
using PlayerManagement.ViewModels;

namespace PlayerManagement.Controllers
{
    public class PlayersController : CognizantController
    {
        private readonly PlayerManagementContext _context;

        public PlayersController(PlayerManagementContext context)
        {
            _context = context;
        }

        // GET: Players
        public async Task<IActionResult> Index(int? page,int? pageSizeID,string SearchString, int? TeamId, int? PlayerPositionId,
            string actionButton, string sortDirection = "asc", string sortField = "Player")
        {
            //Clear the sort/filter/paging URL Cookie for Controller
            CookieHelper.CookieSet(HttpContext, ControllerName() + "URL", "", -1);

            //Toggle the Open/Closed state of the collapse depending on if we are filtering
            ViewData["Filtering"] = "btn-outline-secondary";


            // Populate the filter for Team and Position
            PopulateDropDownLists();

            //List of sort options
            string[] sortOptions = new[] { "Player", "Email", "PlayerPosition", "Team" };

            var players = _context.Players
                .Include(p => p.PlayerPosition)
                .Include(p => p.Plays).ThenInclude(p => p.PlayerPosition)
                .Include(p => p.Team)
                .AsNoTracking();
                          
            #region filters
            //Filters
            if (TeamId.HasValue)
            {
                players = players.Where(p => p.TeamId == TeamId);
                ViewData["Filtering"] = "btn-danger";
            }
            if(PlayerPositionId.HasValue)
            {
                players = players.Where(p => p.PlayerPositionId == PlayerPositionId);
                ViewData["Filtering"] = "btn-danger";
            }
            if (!String.IsNullOrEmpty(SearchString))
            {
                page = 1;//Reset page to start

                players = players.Where(p => p.LastName.ToUpper().Contains(SearchString.ToUpper())
                                       || p.FirstName.ToUpper().Contains(SearchString.ToUpper()));
                ViewData["Filtering"] = "btn-danger";
            }
            //Before we sort, see if we have called for a change of filtering or sorting
            if (!String.IsNullOrEmpty(actionButton)) //Form Submitted!
            {
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

            #region sorting
            if (sortField == "Email")
            {
                if (sortDirection == "asc")
                {
                    players = players.OrderBy(p => p.Email);
                }
                else
                {
                    players = players.OrderByDescending(p => p.Email);

                }
            }
            else if (sortField == "PlayerPosition")
            {
                if (sortDirection == "asc")
                {
                    players = players.OrderBy(p => p.PlayerPosition.PlayerPos);

                }
                else
                {
                    players = players
                        .OrderByDescending(p => p.PlayerPosition.PlayerPos);
                }
            }
            else if (sortField == "Team")
            {
                if (sortDirection == "asc")
                {
                    players = players.OrderBy(p => p.Team.Name);
                }
                else
                {
                    players = players
                        .OrderByDescending(p => p.Team.Name);
                }
            }
            else
            {
                if (sortDirection == "asc")
                {
                    players = players
                        .OrderBy(p => p.FirstName)
                        .ThenBy(p => p.LastName);
                }
                else
                {
                    players = players
                        .OrderByDescending(p => p.FirstName)
                        .ThenBy(p => p.LastName);
                }
            }
            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            #endregion

            //Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID,"PlayersController");
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Player>.CreateAsync(players.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        // GET: Players/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            if (id == null || _context.Players == null)
            {
                return NotFound();
            }

            var player = await _context.Players
                .Include(p => p.PlayerPosition)
                .Include(p => p.Plays).ThenInclude(p => p.PlayerPosition)
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
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            Player player = new Player();
            //We call method to populate checkbox
            PopulateAssignedPlayerPositions(player);
            PopulateDropDownLists();
            return View();
        }

        // POST: Players/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Phone,Email,DOB,PlayerPositionId,TeamId")] Player player, 
            string[] selectedOptions)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            try
            {
                UpdatePlayerPositions(selectedOptions, player);
                if (ModelState.IsValid)
                {
                    _context.Add(player);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
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
            
            PopulateDropDownLists(player);
            PopulateAssignedPlayerPositions(player);
            return View(player);
        }

        // GET: Players/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            if (id == null || _context.Players == null)
            {
                return NotFound();
            }

            var player = await _context.Players
                .Include(p => p.Plays).ThenInclude(p => p.PlayerPosition)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (player == null)
            {
                return NotFound();
            }
            
            PopulateDropDownLists(player);
            PopulateAssignedPlayerPositions(player);
            return View(player);
        }

        // POST: Players/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string[] selectedOptions, Byte[] RowVersion)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            //Go get the Player to update
            var playerToUpdate = await _context.Players
                .Include(p => p.Plays).ThenInclude(p => p.PlayerPosition)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (playerToUpdate == null)
            {
                return NotFound();
            }

            UpdatePlayerPositions(selectedOptions, playerToUpdate);

            //Put the original RowVersion value in the OriginalValues collection for the entity
            _context.Entry(playerToUpdate).Property("RowVersion").OriginalValue = RowVersion;

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
                catch (RetryLimitExceededException /* dex */)
                {
                    ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
                }
                catch (DbUpdateConcurrencyException ex)// Added for concurrency
                {
                    var exceptionEntry = ex.Entries.Single();
                    var clientValues = (Player)exceptionEntry.Entity;
                    var databaseEntry = exceptionEntry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError("",
                            "Unable to save changes. The Player was deleted by another user.");
                    }
                    else
                    {
                        var databaseValues = (Player)databaseEntry.ToObject();
                        if (databaseValues.FirstName != clientValues.FirstName)
                            ModelState.AddModelError("FirstName", "Current value: "
                                + databaseValues.FirstName);
                        if (databaseValues.LastName != clientValues.LastName)
                            ModelState.AddModelError("LastName", "Current value: "
                                + databaseValues.LastName);
                        if (databaseValues.DOB != clientValues.DOB)
                            ModelState.AddModelError("DOB", "Current value: "
                                + String.Format("{0:d}", databaseValues.DOB));
                        if (databaseValues.Phone != clientValues.Phone)
                            ModelState.AddModelError("Phone", "Current value: "
                                + databaseValues.PhoneFormatted);
                        if (databaseValues.Email != clientValues.Email)
                            ModelState.AddModelError("Email", "Current value: "
                                + databaseValues.Email);
                        //For the foreign key, we need to go to the database to get the information to show
                        if (databaseValues.TeamId != clientValues.TeamId)
                        {
                            Team databaseTeam = await _context.Teams.FirstOrDefaultAsync(i => i.Id == databaseValues.TeamId);
                            ModelState.AddModelError("TeamId", $"Current value: {databaseTeam?.Name}");
                        }
                        
                        if (databaseValues.PlayerPositionId != clientValues.PlayerPositionId)
                        {
                            PlayerPosition databasePosition = await _context.PlayerPositions.FirstOrDefaultAsync(i => i.Id == databaseValues.PlayerPositionId);
                            ModelState.AddModelError("PlayerPositionId", $"Current value: {databasePosition?.PlayerPos}");
                        }
                        ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                                + "was modified by another user after you received your values. The "
                                + "edit operation was canceled and the current values in the database "
                                + "have been displayed. If you still want to save your version of this record, click "
                                + "the Save button again. Otherwise click the 'Back to Player List' hyperlink.");
                        playerToUpdate.RowVersion = (byte[])databaseValues.RowVersion;
                        ModelState.Remove("RowVersion");
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
                PopulateAssignedPlayerPositions(playerToUpdate);
                return View(playerToUpdate);
        }
        // GET: Players/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            if (id == null)
            {
                return NotFound();
            }

            var player = await _context.Players
                .Include(p => p.Team)
                .Include(p => p.PlayerPosition)
                .Include(p => p.Plays).ThenInclude(p => p.PlayerPosition)
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
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

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

        //Not needed since we are in a CognizantController
        //private string ControllerName()
        //{
        //    return this.ControllerContext.RouteData.Values["controller"].ToString();
        //}
        private void ViewDataReturnURL()
        {
            ViewData["returnURL"] = MaintainURL.ReturnURL(HttpContext, ControllerName());
        }

        private void PopulateDropDownLists(Player player = null)
        {
            var tQuery = from t in _context.Teams
                         orderby t.Name
                         select t;
            ViewData["TeamId"] = new SelectList(tQuery, "Id", "Name", player?.TeamId);
            ViewData["PlayerPositionId"] = new SelectList(_context.PlayerPositions, "Id", "PlayerPos", player?.PlayerPositionId);
        }

        private void PopulateAssignedPlayerPositions(Player player)
        {
            //Included the child collection in the parent object
            var allOptions = _context.PlayerPositions;
            var currentOptionsHS = new HashSet<int>(player.Plays.Select(p => p.PlayerPositionId));
            //Create two lists
            var selected = new List<ListOptionVM>();
            var available = new List<ListOptionVM>();
            foreach (var p in allOptions)
            {
                if (currentOptionsHS.Contains(p.Id))
                {
                    selected.Add(new ListOptionVM
                    {
                        Id = p.Id,
                        DisplayText = p.PlayerPos,
                    });
                }
                else
                {
                    available.Add(new ListOptionVM
                    {
                        Id = p.Id,
                        DisplayText = p.PlayerPos
                    });
                }
            }

            ViewData["selOpts"] = new MultiSelectList(selected.OrderBy(s => s.DisplayText), "Id", "DisplayText");
            ViewData["availOpts"] = new MultiSelectList(available.OrderBy(s => s.DisplayText), "Id", "DisplayText");
        }
        private void UpdatePlayerPositions(string[] selectedOptions, Player playerToUpdate)
        {
            if (selectedOptions == null)
            {
                playerToUpdate.Plays = new List<PlayPosition>();
                return;
            }

            var selectedOptionsHS = new HashSet<string>(selectedOptions);
            var currentOptionsHS = new HashSet<int>(playerToUpdate.Plays.Select(b => b.PlayerPositionId));
            foreach (var p in _context.PlayerPositions)
            {
                if (selectedOptionsHS.Contains(p.Id.ToString()))//it is selected
                {
                    if (!currentOptionsHS.Contains(p.Id))//but not currently in the PlayerPosition's collection, we add it
                    {
                        playerToUpdate.Plays.Add(new PlayPosition
                        {
                            PlayerPositionId = p.Id,
                            PlayerId = playerToUpdate.Id
                        });
                    }
                }
                else //not selected
                {
                    if (currentOptionsHS.Contains(p.Id))//but is currently in the PlayerPosition's collection - we remove it
                    {
                        PlayPosition positionToRemove = playerToUpdate.Plays.FirstOrDefault(i => i.PlayerPositionId == p.Id);
                        _context.Remove(positionToRemove);
                    }
                }
            }
        }

        private bool PlayerExists(int id)
        {
          return _context.Players.Any(e => e.Id == id);
        }
    }
}
