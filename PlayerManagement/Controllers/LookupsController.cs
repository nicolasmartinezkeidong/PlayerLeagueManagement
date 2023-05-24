using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PlayerManagement.Data;

namespace PlayerManagement.Controllers
{
    public class LookupsController : Controller
    {
        private readonly PlayerManagementContext _context;

        public LookupsController(PlayerManagementContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewData["PlayerPositionId"] = new SelectList(_context.PlayerPositions.OrderBy(p => p.PlayerPos), "Id", "PlayerPos");
            return View();
        }
        //Player Positions partial
        public PartialViewResult PLayerPositions()
        {
            ViewData["PlayerPositionId"] = new
                SelectList(_context.PlayerPositions
                .OrderBy(p => p.PlayerPos), "Id", "PlayerPos");
            return PartialView("_PlayerPositions");
        }
    }
}
