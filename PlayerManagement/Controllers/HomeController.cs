using Microsoft.AspNetCore.Mvc;
using PlayerManagement.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using PlayerManagement.Data;
using Microsoft.AspNetCore.Identity;
using PlayerManagement.ViewModels;
using PlayerManagement.Utilities;

namespace PlayerManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PlayerManagementContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;

        public HomeController(ILogger<HomeController> logger, SignInManager<IdentityUser> signInManager, PlayerManagementContext context)
        {
            _logger = logger;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<IActionResult> IndexAsync()
        {
            //MatchSchedules Items query
            #region MatchSchedules
            var currentDate = DateTime.Now;
            var startOfWeek = currentDate.Date.AddDays(DayOfWeek.Sunday - currentDate.DayOfWeek);
            var endOfWeek = startOfWeek.AddDays(7);

            // Query the database for all match schedules
            var matchSchedules = from m in _context.MatchSchedules
                .Include(m => m.AwayTeam)
                .Include(m => m.Field)
                .Include(m => m.HomeTeam)
                .AsNoTracking()
                where m.Date >= startOfWeek && m.Date < endOfWeek
                select m;

            int total = matchSchedules.Count();
            ViewBag.MatchSchedules = matchSchedules.ToList();
            ViewBag.TotalMatchSchedules = total;

            // Calculate the matchweek number based on the MatchDay first match
            int matchweekNumber = matchSchedules.FirstOrDefault()?.MatchDay ?? 0;
            ViewBag.MatchweekNumber = matchweekNumber;

            #endregion

            #region Standings
            var standings = await StandingsCalculator.CalculateStandingsAsync(_context);

            // Take only 'x' amount of teams
            standings = standings.Take(10).ToList();
            ViewBag.Standings = standings;
            #endregion


            #region Team Stats
            var topTeamsByGoalsFavor = standings.OrderByDescending(s => s.GoalsFavor).Take(5).ToList();
            var topTeamsByGoalsAgainst = standings.OrderBy(s => s.GoalsAgainst).Take(5).ToList();
            var topTeamsByWins = standings.OrderByDescending(s => s.Won).Take(5).ToList();

            ViewBag.TopTeamsByGoalsFavor = topTeamsByGoalsFavor;
            ViewBag.TopTeamsByGoalsAgainst = topTeamsByGoalsAgainst;
            ViewBag.TopTeamsByWins = topTeamsByWins;
            #endregion

            #region Goal Scorers and Players to Watch
            var playerStats = await _context.PlayerMatchs
                .Include(p => p.Player)
                .OrderByDescending(p => p.Goals)
                .Take(5)
                .ToListAsync();

            // Pass the top goalscorers data to the view
            ViewBag.PlayerStats = playerStats;
            #endregion

            #region Player to Watch 
            //NM: Note, Player to Watch should select random players to update the cards in section, but for this example is not neccesary to include a random method because the
            //DB get updated every time the app is run, what makes to display different players each time

            var randomPlayerStats = _context.PlayerMatchs
                .Include(p => p.Player)
                .Include(m => m.Match)
                .Take(2)
                .AsNoTracking();

            ViewBag.PlayerToWatch = randomPlayerStats.ToList();

            var randomPlayerStatsList = randomPlayerStats.ToList();

            // Retrieve all matches played by the selected players
            var matchesPlayedByPlayers = _context.MatchSchedules
                .Include(match => match.HomeTeam.Players) // Include HomeTeam and its Players
                .Include(match => match.AwayTeam.Players) // Include AwayTeam and its Players
                .AsEnumerable() // Retrieve data from the database
                .Where(match =>
                    match.HomeTeam.Players.Any(player => randomPlayerStatsList.Any(rps => rps.PlayerId == player.Id)) ||
                    match.AwayTeam.Players.Any(player => randomPlayerStatsList.Any(rps => rps.PlayerId == player.Id))
                )
                .ToList();

            int matchCountP1 = matchesPlayedByPlayers.Count(match =>
                match.HomeTeam.Players.Any(player => player.Id == randomPlayerStatsList[0].PlayerId) ||
                match.AwayTeam.Players.Any(player => player.Id == randomPlayerStatsList[0].PlayerId)
            );

            int matchCountP2 = matchesPlayedByPlayers.Count(match =>
                match.HomeTeam.Players.Any(player => player.Id == randomPlayerStatsList[1].PlayerId) ||
                match.AwayTeam.Players.Any(player => player.Id == randomPlayerStatsList[1].PlayerId)
            );

            ViewBag.MatchCountP1 = matchCountP1;
            ViewBag.MatchCountP2 = matchCountP2;



            #endregion

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}