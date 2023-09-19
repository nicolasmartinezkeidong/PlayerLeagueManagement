using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayerManagement.Data;
using PlayerManagement.Models;
using PlayerManagement.ViewModels;
using System.Text;

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
            var teams = await _context.Teams
                .OrderBy(t => t.Name)
                .ToListAsync();

            var standings = teams.Select(s => new StandingsVM
            {
                TeamName = s.Name
            }).ToList();

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

            // Filter matches for the given team
            var teamMatches = matches
                .Where(match => match.HomeTeam.Name == teamName || match.AwayTeam.Name == teamName)
                .OrderByDescending(match => match.Date) // Order by date to get the most recent matches first
                .Take(3) // Take the last 3 matches
                .ToList();

            // Initialize an empty string to store the form
            var form = new StringBuilder();

            foreach (var match in teamMatches)
            {
                if (match.HomeTeam.Name == teamName)
                {
                    if (match.HomeTeamScore > match.AwayTeamScore)
                    {
                        form.Append("W"); // Team won
                    }
                    else if (match.HomeTeamScore < match.AwayTeamScore)
                    {
                        form.Append("L"); // Team lost
                    }
                    else
                    {
                        form.Append("D"); // Match was drawn
                    }
                }
                else if (match.AwayTeam.Name == teamName)
                {
                    if (match.AwayTeamScore > match.HomeTeamScore)
                    {
                        form.Append("W"); // Team won
                    }
                    else if (match.AwayTeamScore < match.HomeTeamScore)
                    {
                        form.Append("L"); // Team lost
                    }
                    else
                    {
                        form.Append("D"); // Match was drawn
                    }
                }
            }

            return form.ToString();
        }

        private List<StandingsVM> CalculateStandings(List<MatchSchedule> matches)
        {
            var standings = new List<StandingsVM>();

            // Retrieve a list of unique team names from the matches
            var teamNames = matches.SelectMany(match => new[] { match.HomeTeam.Name, match.AwayTeam.Name }).Distinct();

            // Calculate team statistics for each team
            foreach (var teamName in teamNames)
            {
                var teamMatches = matches.Where(match =>
                    match.HomeTeam.Name == teamName || match.AwayTeam.Name == teamName);

                var standing = new StandingsVM
                {
                    TeamName = teamName,
                    Played = teamMatches.Count(),
                    // Add other statistics calculation logic here
                };

                // Add the standing to the list
                standings.Add(standing);
            }

            // Sort and assign positions
            standings = standings.OrderByDescending(s => s.Points).ThenByDescending(s => s.GoalsDifference).ToList();
            for (var i = 0; i < standings.Count; i++)
            {
                standings[i].Position = i + 1;
            }

            return standings;
        }


    }
}
