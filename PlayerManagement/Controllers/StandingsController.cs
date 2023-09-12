using Microsoft.AspNetCore.Mvc;
using PlayerManagement.Models;
using PlayerManagement.ViewModels;

namespace PlayerManagement.Controllers
{
    public class StandingsController : Controller
    {
        public IActionResult Standings()
        {
            // Assuming you have a list of MatchSchedule and Team objects
            List<MatchSchedule> matches = GetMatchSchedules(); // Implement this method to get the matches.
            List<Team> teams = GetTeams(); // Implement this method to get the teams.

            // Create a dictionary to store team statistics
            Dictionary<Team, StandingsVM> teamStats = new Dictionary<Team, StandingsVM>();

            // Initialize team statistics
            foreach (var team in teams)
            {
                teamStats[team] = new StandingsVM
                {
                    TeamName = team.Name,
                    Played = 0,
                    Won = 0,
                    Drawn = 0,
                    Lost = 0,
                    GoalsFavor = 0,
                    GoalsAgainst = 0,
                    GoalsDifference = 0,
                    Points = 0,
                    Form = string.Empty
                };
            }

            // Calculate team statistics based on match results
            foreach (var match in matches)
            {
                if (match.HomeTeamScore.HasValue && match.AwayTeamScore.HasValue)
                {
                    // Update team statistics
                    var homeTeamStat = teamStats[match.HomeTeam];
                    var awayTeamStat = teamStats[match.AwayTeam];

                    homeTeamStat.Played++;
                    awayTeamStat.Played++;

                    homeTeamStat.GoalsFavor += match.HomeTeamScore.Value;
                    homeTeamStat.GoalsAgainst += match.AwayTeamScore.Value;

                    awayTeamStat.GoalsFavor += match.AwayTeamScore.Value;
                    awayTeamStat.GoalsAgainst += match.HomeTeamScore.Value;

                    if (match.HomeTeamScore > match.AwayTeamScore)
                    {
                        homeTeamStat.Won++;
                        awayTeamStat.Lost++;
                        homeTeamStat.Points += 3;
                        homeTeamStat.Form += "W";
                        awayTeamStat.Form += "L";
                    }
                    else if (match.HomeTeamScore < match.AwayTeamScore)
                    {
                        homeTeamStat.Lost++;
                        awayTeamStat.Won++;
                        awayTeamStat.Points += 3;
                        homeTeamStat.Form += "L";
                        awayTeamStat.Form += "W";
                    }
                    else
                    {
                        homeTeamStat.Drawn++;
                        awayTeamStat.Drawn++;
                        homeTeamStat.Points++;
                        awayTeamStat.Points++;
                        homeTeamStat.Form += "D";
                        awayTeamStat.Form += "D";
                    }

                    homeTeamStat.GoalsDifference = homeTeamStat.GoalsFavor - homeTeamStat.GoalsAgainst;
                    awayTeamStat.GoalsDifference = awayTeamStat.GoalsFavor - awayTeamStat.GoalsAgainst;
                }
            }

            // Order the standings by points (and then by goal difference if points are the same)
            var orderedStandings = teamStats.Values.OrderByDescending(x => x.Points)
                                                  .ThenByDescending(x => x.GoalsDifference)
                                                  .ToList();

            // Assign positions
            for (int i = 0; i < orderedStandings.Count; i++)
            {
                orderedStandings[i].Position = i + 1;
            }

            return View(orderedStandings);
        }
    }
}
