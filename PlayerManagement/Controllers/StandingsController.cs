using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlayerManagement.Data;
using PlayerManagement.Models;
using PlayerManagement.ViewModels;

namespace PlayerManagement.Controllers
{
    public class StandingsController : Controller
    {
        private readonly PlayerManagementContext _context;
        private readonly StandingsVM _standingsVM;

        public StandingsController(PlayerManagementContext context, StandingsVM standingsVM)
        {
            _context = context;
           _standingsVM = standingsVM;
        }

        // GET: Standings
        public async Task<IActionResult> Index()
        {
            return null;
            //var standings = await (from s in _context.Standings
            //                   .OrderBy(s => s.Position)
            //                       select new StandingsVM
            //                       {
            //                           Id = s.Id,
            //                           Position = s.Position,
            //                           TeamName = s.TeamName,
            //                           Played = s.Played,
            //                           Won = s.Won,
            //                           Drawn = s.Drawn,
            //                           Lost = s.Lost,
            //                           GoalsFavor = s.GoalsFavor,
            //                           GoalsAgainst = s.GoalsAgainst,
            //                           GoalsDifference = s.GoalsDifference,
            //                           Points = s.Points,
            //                           Form = s.Form
            //                       }).ToListAsync();

            //return View(standings);
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
