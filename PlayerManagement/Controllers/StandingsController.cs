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
            var standings = await (from s in _context.StandingsVM
                               .OrderBy(s => s.Position)
                                   select new StandingsVM
                                   {
                                       Id = s.Id,
                                       Position = s.Position,
                                       TeamName = s.TeamName,
                                       Played = s.Played,
                                       Won = s.Won,
                                       Drawn = s.Drawn,
                                       Lost = s.Lost,
                                       GoalsFavor = s.GoalsFavor,
                                       GoalsAgainst = s.GoalsAgainst,
                                       GoalsDifference = s.GoalsDifference,
                                       Points = s.Points,
                                       Form = s.Form
                                   }).ToListAsync();

            //// Debugging output
            //foreach (var item in standings)
            //{
            //    Console.WriteLine($"Team: {item.TeamName}, Points: {item.Points}");
            //}

            return View(standings);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            return null;
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string Id, string[] selectedRoles)
        {
            return null;
        }


    }
}
