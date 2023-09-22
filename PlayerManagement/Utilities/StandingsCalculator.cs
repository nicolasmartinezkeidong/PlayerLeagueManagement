using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlayerManagement.Data;
using PlayerManagement.Models;
using PlayerManagement.ViewModels;

namespace PlayerManagement.Utilities
{
    public class StandingsCalculator
    {
        public static async Task<List<StandingsVM>> CalculateStandingsAsync(PlayerManagementContext context)
        {
            var teams = await context.Teams
                .OrderBy(t => t.Name)
                .ToListAsync();

            var standings = teams.Select(s => new StandingsVM
            {
                TeamName = s.Name
            }).ToList();

            // Matches results
            var matches = await context.MatchSchedules.ToListAsync();

            foreach (var standing in standings)
            {
                var teamName = standing.TeamName;

                // Filter matches for the current team, both home and away
                var teamMatches = matches.Where(match => match.HomeTeam.Name == teamName || match.AwayTeam.Name == teamName);

                standing.Played = teamMatches.Count();
                standing.Won = teamMatches.Count(match => (match.HomeTeam.Name == teamName && match.HomeTeamScore > match.AwayTeamScore) || (match.AwayTeam.Name == teamName && match.AwayTeamScore > match.HomeTeamScore));
                standing.Lost = teamMatches.Count(match => (match.HomeTeam.Name == teamName && match.HomeTeamScore < match.AwayTeamScore) || (match.AwayTeam.Name == teamName && match.AwayTeamScore < match.HomeTeamScore));
                standing.Drawn = teamMatches.Count(match => match.HomeTeamScore == match.AwayTeamScore);
                standing.GoalsFavor = teamMatches.Sum(match => match.HomeTeam.Name == teamName ? match.HomeTeamScore ?? 0 : match.AwayTeamScore ?? 0);
                standing.GoalsAgainst = teamMatches.Sum(match => match.HomeTeam.Name == teamName ? match.AwayTeamScore ?? 0 : match.HomeTeamScore ?? 0);
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

            return standings;
        }


        private static string CalculateForm(string teamName, List<MatchSchedule> matches)
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
    }
}
