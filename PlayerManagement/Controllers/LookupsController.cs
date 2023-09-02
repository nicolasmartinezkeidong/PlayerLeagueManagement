using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PlayerManagement.Data;

namespace PlayerManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LookupsController : Controller
    {
        private readonly PlayerManagementContext _context;

        public LookupsController(PlayerManagementContext context)
        {
            _context = context;
        }

        public IActionResult Index(string Tab = "PLayerPositions-Tab")
        {
            //Note: select the tab you want to load by passing in
            //the ID of the tab such as PlayerPositions-Tab, OtherLookUp-Tab, etc
            ViewData["Tab"] = Tab;
            return View();
        }
        //Player Positions partial
        public PartialViewResult PLayerPositions()
        {
            ViewData["PlayerPositionsId"] = new
                SelectList(_context.PlayerPositions
                .OrderBy(p => p.PlayerPos), "Id", "PlayerPos");
            return PartialView("_PlayerPositions");
        }
        //League partial
        public PartialViewResult Leagues()
        {
            ViewData["LeaguesId"] = new
                SelectList(_context.Leagues
                .OrderBy(l => l.Name), "Id", "Name");
            return PartialView("_Leagues");
        }

    }
}
