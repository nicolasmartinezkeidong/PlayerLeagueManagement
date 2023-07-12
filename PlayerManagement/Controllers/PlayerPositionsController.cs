using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OfficeOpenXml;
using PlayerManagement.CustomControllers;
using PlayerManagement.Data;
using PlayerManagement.Models;

namespace PlayerManagement.Controllers
{
    public class PlayerPositionsController : CognizantController
    {
        private readonly PlayerManagementContext _context;

        public PlayerPositionsController(PlayerManagementContext context)
        {
            _context = context;
        }

        // GET: PlayerPositions
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Lookups", new { Tab = ControllerName() + "-Tab" });
        }

        // GET: PlayerPositions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.PlayerPositions == null)
            {
                return NotFound();
            }

            var playerPosition = await _context.PlayerPositions
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (playerPosition == null)
            {
                return NotFound();
            }

            return View(playerPosition);
        }

        // GET: PlayerPositions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PlayerPositions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PlayerPos")] PlayerPosition playerPosition)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(playerPosition);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Lookups", new { Tab = ControllerName() + "-Tab" });
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: PlayerPositions.PlayerPos"))
                {
                    ModelState.AddModelError("PlayerPos", "Unable to save changes. You cannot have duplicate Player Positions.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }

            return View(playerPosition);
        }

        // GET: PlayerPositions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.PlayerPositions == null)
            {
                return NotFound();
            }

            var playerPosition = await _context.PlayerPositions.FindAsync(id);
            if (playerPosition == null)
            {
                return NotFound();
            }
            return View(playerPosition);
        }

        // POST: PlayerPositions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PlayerPos")] PlayerPosition playerPosition)
        {
            //Go get the record to update
            var playerPositionToUpdate = await _context.PlayerPositions.SingleOrDefaultAsync(p => p.Id == id);

            if (playerPositionToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<PlayerPosition>(playerPositionToUpdate, "",
                p => p.PlayerPos))

            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Lookups", new { Tab = ControllerName() + "-Tab" });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlayerPositionExists(playerPositionToUpdate.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
                return View(playerPositionToUpdate);
            }

        // GET: PlayerPositions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.PlayerPositions == null)
            {
                return NotFound();
            }

            var playerPosition = await _context.PlayerPositions
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (playerPosition == null)
            {
                return NotFound();
            }

            return View(playerPosition);
        }

        // POST: PlayerPositions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.PlayerPositions == null)
            {
                return Problem("Entity set 'PlayerManagementContext.PlayerPositions'  is null.");
            }
            var playerPosition = await _context.PlayerPositions.FindAsync(id);
            try
            {
                if (playerPosition != null)
                {
                    _context.PlayerPositions.Remove(playerPosition);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Lookups", new { Tab = ControllerName() + "-Tab" });
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
                {
                    ModelState.AddModelError("", "Unable to Delete Player Position. Remember, you cannot delete a Player Position that has been used.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            return View(playerPosition);
        }

        [HttpPost]
        public async Task<IActionResult> InsertFromExcel(IFormFile theExcel)
        {
            // Check if file is null
            if (theExcel == null)
            {
                TempData["MessageFail"] = "No file was uploaded. Please select a file and try again.";
                return RedirectToAction("Index", "Lookups", new { Tab = "PlayerPositions-Tab" });
            }

            // Check if the file extension is valid
            var fileExtension = Path.GetExtension(theExcel.FileName);
            if (fileExtension != ".xlsx" && fileExtension != ".xls")
            {
                TempData["MessageFail"] = "Invalid file format. Please upload an Excel file (.xlsx or .xls).";
                return RedirectToAction("Index", "Lookups", new { Tab = "PlayerPositions-Tab" });
            }

            // The file is not null and has a valid extension, continue with processing

            ExcelPackage excel;
            using (var memoryStream = new MemoryStream())
            {
                await theExcel.CopyToAsync(memoryStream);
                excel = new ExcelPackage(memoryStream);
            }

            var workSheet = excel.Workbook.Worksheets[0];
            var start = workSheet.Dimension.Start;
            var end = workSheet.Dimension.End;

            // Start a new list to hold imported objects
            List<PlayerPosition> playerPositions = new List<PlayerPosition>();

            List<string> modelErrors = new List<string>();

            for (int row = start.Row; row <= end.Row; row++)
            {
                // Row by row...
                string playerPos = workSheet.Cells[row, 1].Text;

                // Check if the value exists in the database
                if (_context.PlayerPositions.Any(p => p.PlayerPos.ToLower() == playerPos.ToLower()))
                {
                    modelErrors.Add("The file was not updated because there is a duplicate value: " + playerPos);
                }
                else
                {
                    // Create a new PlayerPosition object
                    PlayerPosition p = new PlayerPosition
                    {
                        PlayerPos = playerPos
                    };

                    playerPositions.Add(p);
                }
            }

            if (modelErrors.Count > 0)
            {
                if (playerPositions.Count == 0)
                {
                    // No records were added
                    TempData["MessageFail"] = "The file was not updated. " + modelErrors.Count + " duplicate value(s) found.";
                }
                else
                {
                    // Some records were added
                    try
                    {
                        _context.PlayerPositions.AddRange(playerPositions);
                        await _context.SaveChangesAsync();

                        TempData["MessageSucc"] = "The file was successfully uploaded. Number of records added: " + playerPositions.Count;
                    }
                    catch (Exception ex)
                    {
                        
                        TempData["MessageFail"] = "An error occurred while saving the records to the database: " + ex.Message;
                        return RedirectToAction("Index", "Lookups", new { Tab = "PlayerPositions-Tab" });
                    }

                    ViewData["ModelErrors"] = modelErrors;
                }
            }
            else
            {
                // All records were added
                try
                {
                    _context.PlayerPositions.AddRange(playerPositions);
                    await _context.SaveChangesAsync();

                    TempData["MessageSucc"] = "The file was successfully uploaded. Number of records added: " + playerPositions.Count;
                }
                catch (Exception ex)
                {
                    
                    TempData["MessageFail"] = "An error occurred while saving the records to the database: " + ex.Message;
                    return RedirectToAction("Index", "Lookups", new { Tab = "PlayerPositions-Tab" });
                }
            }
            return RedirectToAction("Index", "Lookups", new { Tab = "PlayerPositions-Tab" });
        }

        private bool PlayerPositionExists(int id)
        {
          return _context.PlayerPositions.Any(e => e.Id == id);
        }
    }
}
