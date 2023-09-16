using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayerManagement.Data;
using PlayerManagement.Models;
using PlayerManagement.ViewModels;

namespace PlayerManagement.Controllers
{
    public class StandingsController : Controller
    {
        private readonly PlayerManagementContext _context;

        public StandingsController(PlayerManagementContext context)
        {
            _context = context;
        }

        // GET: Standings
        public async Task<IActionResult> Index()
        {
            var standings = await _context.StandingsVM
                .OrderBy(s => s.Position)
                .ToListAsync();

            // matches results
            var matches = await _context.MatchSchedules.ToListAsync();

            // Calculate team statistics and update the standings
            foreach (var standing in standings)
            {
                var teamName = standing.TeamName;

                // Filter matches for the current team, both home and away
                var teamMatches = matches.Where(match =>match.HomeTeam.Name == teamName || match.AwayTeam.Name == teamName);

                standing.Played = teamMatches.Count();
                standing.Won = teamMatches.Count(match =>(match.HomeTeam.Name == teamName && match.HomeTeamScore > match.AwayTeamScore) || (match.AwayTeam.Name == teamName && match.AwayTeamScore > match.HomeTeamScore));
                standing.Lost = teamMatches.Count(match =>(match.HomeTeam.Name == teamName && match.HomeTeamScore < match.AwayTeamScore) || (match.AwayTeam.Name == teamName && match.AwayTeamScore < match.HomeTeamScore));
                standing.Drawn = teamMatches.Count(match =>match.HomeTeamScore == match.AwayTeamScore);
                standing.GoalsFavor = teamMatches.Sum(match =>match.HomeTeam.Name == teamName ? match.HomeTeamScore ?? 0 : match.AwayTeamScore ?? 0);
                standing.GoalsAgainst = teamMatches.Sum(match =>match.HomeTeam.Name == teamName ? match.AwayTeamScore ?? 0 : match.HomeTeamScore ?? 0);
                standing.GoalsDifference = standing.GoalsFavor - standing.GoalsAgainst;
                standing.Points = (standing.Won * 3) + standing.Drawn;

                standing.Form = CalculateForm(teamName, matches);
            }

            // Sort by points and goal difference
            standings = standings.OrderByDescending(s => s.Points).ThenByDescending(s => s.GoalsDifference).ToList();

            // Assign positions
            for (var i = 0; i < standings.Count; i++)
            {
                standings[i].Position = i + 1;
            }

            return View(standings);
        }

        private string CalculateForm(string teamName, List<MatchSchedule> matches)
        {
           
            return "WDL"; 
        }


    }
}
