using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PlayerManagement.CustomControllers;
using PlayerManagement.Data;
using PlayerManagement.Models;
using PlayerManagement.Utilities;
using PlayerManagement.ViewModels;
using Team = PlayerManagement.Models.Team;

namespace PlayerManagement.Controllers
{
    [Authorize(Roles = "Admin, Captain")]
    public class TeamsController : CognizantController
    {
        private readonly PlayerManagementContext _context;

        //for sending email
        private readonly IMyEmailSender _emailSender;



        public TeamsController(PlayerManagementContext context, IMyEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
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

            var teams = _context.Teams
                .Include(p => p.League)
                .Include(p => p.Players)
                .Include(d => d.TeamDocuments)//Just if we want to show documents on Index view
                .AsNoTracking();

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
                .Include(l => l.League)
                .Include(p => p.Players)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (team == null)
            {
                return NotFound();
            }

            PopulateAssignedPlayerData(team);
            return View(team);
        } 

        public PartialViewResult ListOfPlayersDetails(int id)
        {
            var query = from p in _context.Players
                        where p.TeamId == id
                        orderby p.LastName, p.FirstName
                        select p;
            return PartialView("_ListOfPlayers", query.ToList());
        }

        public PartialViewResult ListOfDocumentsDetails(int id)
        {
            var query = from p in _context.TeamDocuments
                        where p.TeamId == id
                        orderby p.FileName
                        select p;
            return PartialView("_ListOfDocuments", query.ToList());
        }

        // GET: Teams/Create
        public IActionResult Create()
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            PopulateDropDownLists();
            Team team = new Team();
            PopulateAssignedPlayerData(team);
            return View();
        }

        // POST: Teams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,RegistrationDate,LeagueId")] Team team,
            string[] selectedOptions, List<IFormFile> theFiles)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            try
            {
                UpdateTeamPlayers(selectedOptions, team);
                if (ModelState.IsValid)
                {
                    if (_context.Teams.Any(p => p.Name.ToLower() == team.Name.ToLower()))
                    {
                        ModelState.AddModelError("", team.Name + " was not created because is a duplicate value");
                    }
                    else
                    {
                        // Check if any selected players are already associated with a team
                        var playersWithTeam = await _context.Players
                            .Where(p => selectedOptions.Contains(p.Id.ToString()) && p.TeamId != 0)
                            .ToListAsync();

                        if (playersWithTeam.Any())
                        {
                            // Filter out players with existing teams from the team's Players collection
                            team.Players = team.Players.Where(p => !playersWithTeam.Any(pt => pt.Id == p.Id)).ToList();

                            // Display a message about the players not assigned to the team
                            ViewData["PlayerNotAssignedMessage"] = "Note: Some players were not assigned to the team as they are already part of other teams.";
                        }
                        await AddDocumentsAsync(team, theFiles);
                        _context.Add(team);
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));

                }

            }
            catch (RetryLimitExceededException /* dex */)
            {
                ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists contact your system administrator.");
            }
            PopulateAssignedPlayerData(team);
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

            var team = await _context.Teams
                .Include(t => t.Players)
                .Include(d => d.TeamDocuments)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (team == null)
            {
                return NotFound();
            }
            PopulateDropDownLists(team);
            PopulateAssignedPlayerData(team);
            return View(team);
        }

        // POST: Teams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Byte[] RowVersion, string[] selectedOptions
            , List<IFormFile> theFiles)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            //Go get the Team to update
            var teamToUpdate = await _context.Teams
                .Include(t => t.Players)
                .Include(d => d.TeamDocuments)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (teamToUpdate == null)
            {
                return NotFound();
            }

            //Put the original RowVersion value in the OriginalValues collection for the entity
            _context.Entry(teamToUpdate).Property("RowVersion").OriginalValue = RowVersion;

            UpdateTeamPlayers(selectedOptions, teamToUpdate);

            //Try updating it with the values posted
            if (await TryUpdateModelAsync<Team>(teamToUpdate, "",
                t => t.Name, t => t.RegistrationDate, t => t.LeagueId))
            {
                try
                {
                    await AddDocumentsAsync(teamToUpdate, theFiles);
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
                    var clientValues = (Team)exceptionEntry.Entity;
                    var databaseEntry = exceptionEntry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError("",
                            "Unable to save changes. The Team was deleted by another user.");
                    }
                    else
                    {
                        var databaseValues = (Team)databaseEntry.ToObject();
                        if (databaseValues.Name != clientValues.Name)
                            ModelState.AddModelError("Name", "Current value: "
                                + databaseValues.Name);
                        if (databaseValues.RegistrationDate != clientValues.RegistrationDate)
                            ModelState.AddModelError("RegistrationDate", "Current value: "
                                + String.Format("{0:d}", databaseValues.RegistrationDate));
                        //For the foreign key, we need to go to the database to get the information to show
                        if (databaseValues.LeagueId != clientValues.LeagueId)
                        {
                            League databaseLeague = await _context.Leagues.FirstOrDefaultAsync(i => i.Id == databaseValues.LeagueId);
                            ModelState.AddModelError("LeagueId", $"Current value: {databaseLeague?.Name}");
                        }
                        ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                                + "was modified by another user after you received your values. The "
                                + "edit operation was canceled and the current values in the database "
                                + "have been displayed. If you still want to save your version of this record, click "
                                + "the Save button again. Otherwise click the 'Back to Team List' hyperlink.");
                        teamToUpdate.RowVersion = (byte[])databaseValues.RowVersion;
                        ModelState.Remove("RowVersion");
                    }
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists contact your system administrator.");
                }

            }
            PopulateAssignedPlayerData(teamToUpdate);
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

        // GET/POST: Teams/Notification/5
        public async Task<IActionResult> Notification(int? id, string Subject, string emailContent)
        {
            if (id == null)
            {
                return NotFound();
            }
            Team t = await _context.Teams.FindAsync(id);

            ViewData["id"] = id;
            ViewData["name"] = t.Name;

            if (string.IsNullOrEmpty(Subject) || string.IsNullOrEmpty(emailContent))
            {
                ViewData["Message"] = "You must enter both a Subject and a message Content before sending the message.";
            }
            else
            {
                int playersCount = 0;
                try
                {
                    //Send a Notice.
                    List<EmailAddress> players = (from p in _context.Players
                                                where p.TeamId == id
                                                select new EmailAddress
                                                {
                                                    Name = p.FullName,
                                                    Address = p.Email
                                                }).ToList();
                    playersCount = players.Count();
                    if (playersCount > 0)
                    {
                        var msg = new EmailMessage()
                        {
                            ToAddresses = players,
                            Subject = Subject,
                            Content = "<p>" + emailContent + "</p><p>Please access the <strong>Player Management</strong> web site to review.</p>"

                        };
                        await _emailSender.SendToManyAsync(msg);
                        ViewData["Message"] = "Message sent to " + playersCount + " player(s)"
                            + ((playersCount == 1) ? "." : "s.");
                    }
                    else
                    {
                        ViewData["Message"] = "Message not sent.  No players in the Team.";
                    }
                }
                catch (Exception ex)
                {
                    string errMsg = ex.GetBaseException().Message;
                    ViewData["Message"] = "Error: Could not send email message to the " + playersCount + " players"
                        + ((playersCount == 1) ? "" : "s") + " in the team.";
                }
            }
            return View();
        }

        public async Task<FileContentResult> Download(int id)
        {
            var theFile = await _context.UploadedFiles
                .Include(d => d.FileContent)
                .Where(f => f.Id == id)
                .FirstOrDefaultAsync();
            return File(theFile.FileContent.Content, theFile.MimeType, theFile.FileName);
        }

        private async Task AddDocumentsAsync(Team team, List<IFormFile> theFiles)
        {
            foreach (var f in theFiles)
            {
                if (f != null)
                {
                    string mimeType = f.ContentType;
                    string fileName = Path.GetFileName(f.FileName);
                    long fileLength = f.Length;
                    //Note: you could filter for mime types if you only want to allow
                    //certain types of files.  I am allowing everything.
                    if (!(fileName == "" || fileLength == 0))//Looks like we have a file!!!
                    {
                        TeamDocument t = new TeamDocument();
                        using (var memoryStream = new MemoryStream())
                        {
                            await f.CopyToAsync(memoryStream);
                            t.FileContent.Content = memoryStream.ToArray();
                        }
                        t.MimeType = mimeType;
                        t.FileName = fileName;
                        team.TeamDocuments.Add(t);
                    };
                }
            }
        }

        //private string ControllerName()
        //{
        //    return this.ControllerContext.RouteData.Values["controller"].ToString();
        //}
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

        private void PopulateAssignedPlayerData(Team team)
        {
            //For this to work, you must have Included the child collection in the parent object
            var allOptions = _context.Players;
            var currentOptionsHS = new HashSet<int>(team.Players.Select(b => b.Id));
            //Instead of one list with a boolean, we will make two lists
            var selected = new List<ListOptionVM>();
            var available = new List<ListOptionVM>();
            foreach (var p in allOptions)
            {
                if (currentOptionsHS.Contains(p.Id))
                {
                    selected.Add(new ListOptionVM
                    {
                        Id = p.Id,
                        DisplayText = p.FullName
                    });
                }
                else
                {
                    available.Add(new ListOptionVM
                    {
                        Id = p.Id,
                        DisplayText = p.FullName
                    });
                }
            }

            ViewData["selOpts"] = new MultiSelectList(selected.OrderBy(s => s.DisplayText), "Id", "DisplayText");
            ViewData["availOpts"] = new MultiSelectList(available.OrderBy(s => s.DisplayText), "Id", "DisplayText");
        }
        private void UpdateTeamPlayers(string[] selectedOptions, Team teamToUpdate)
        {
            if (selectedOptions == null)
            {
                teamToUpdate.Players = new List<Player>();
                return;
            }

            var selectedOptionsHS = new HashSet<string>(selectedOptions);
            var currentOptionsHS = new HashSet<int>(teamToUpdate.Players.Select(b => b.Id));
            foreach (var s in _context.Players)
            {
                if (selectedOptionsHS.Contains(s.Id.ToString()))//it is selected
                {
                    if (!currentOptionsHS.Contains(s.Id))//but not currently in the Team's collection - Add it!
                    {
                        teamToUpdate.Players.Add(new Player
                        {
                            Id = s.Id,
                            TeamId = teamToUpdate.Id
                        });
                    }
                }
                else //not selected
                {
                    if (currentOptionsHS.Contains(s.Id))//but is currently in the Team's collection - Remove it!
                    {
                        Player playerToRemove = teamToUpdate.Players.FirstOrDefault(d => d.Id == s.Id);
                        _context.Remove(playerToRemove);
                    }
                }
            }
        }

        private bool TeamExists(int id)
        {
          return _context.Teams.Any(e => e.Id == id);
        }

        public async Task<IActionResult> TeamStatsAsync(int? page, int? pageSizeID)
        {
            var teamStats = _context.Teams
                .Select(team => new TeamStatsVM
                {
                    Id = team.Id,
                    TeamName = team.Name,
                    Goals = _context.PlayerMatchs.Where(pm => pm.Match.HomeTeamId == team.Id || pm.Match.AwayTeamId == team.Id).Sum(pm => pm.Goals),
                    RedCards = _context.PlayerMatchs.Where(pm => pm.Match.HomeTeamId == team.Id || pm.Match.AwayTeamId == team.Id).Sum(pm => pm.RedCards ?? 0),
                    YellowCards = _context.PlayerMatchs.Where(pm => pm.Match.HomeTeamId == team.Id || pm.Match.AwayTeamId == team.Id).Sum(pm => pm.YellowCards ?? 0)
                })
                .OrderBy(s => s.TeamName);

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, "TeamStats");
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<TeamStatsVM>.CreateAsync(teamStats.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }
    }
}
