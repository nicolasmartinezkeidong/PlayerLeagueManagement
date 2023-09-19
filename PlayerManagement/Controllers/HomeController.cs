using Microsoft.AspNetCore.Mvc;
using PlayerManagement.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using PlayerManagement.Data;
using Microsoft.AspNetCore.Identity;

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

        public IActionResult Index()
        {
            //MatchSchedules Items query
            #region
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
            var standingData = from s in _context.StandingsVM
                               .AsNoTracking()
                               select s;

            ViewBag.Standings = standingData.ToList();
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