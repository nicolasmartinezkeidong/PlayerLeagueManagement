using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PlayerManagement.Data;
using PlayerManagement.Models;
using PlayerManagement.Utilities;

namespace PlayerManagement.Controllers
{
    public class PlayerDocumentsController : CustomControllers.ElephantController
    {
        private readonly PlayerManagementContext _context;

        public PlayerDocumentsController(PlayerManagementContext context)
        {
            _context = context;
        }

        // GET: PlayerDocuments
        public async Task<IActionResult> Index(int? page, int? pageSizeID,
            int? PlayerId, string SearchString)
        {
            //Clear the sort/filter/paging URL Cookie for Controller
            CookieHelper.CookieSet(HttpContext, ControllerName() + "URL", "", -1);

            PopulateDropDownLists();

            //Toggle the Open/Closed state of the collapse depending on if we are filtering
            ViewData["Filtering"] = "btn-outline-dark"; //Asume not filtering
            //Then in each "test" for filtering, add ViewData["Filtering"] = "btn-danger" if true;

            var playerDocuments = from p in _context.PlayerDocuments
                .Include(p => p.Player)
                                  select p;
            
            if (PlayerId.HasValue)
            {
                playerDocuments = playerDocuments.Where(p => p.PlayerId == PlayerId);
                ViewData["Filtering"] = "btn-danger";
            }
            if (!String.IsNullOrEmpty(SearchString))
            {
                playerDocuments = playerDocuments.Where(p => p.FileName.ToUpper().Contains(SearchString.ToUpper()));
                ViewData["Filtering"] = "btn-danger";
            }
            // Always sort by File Name
            playerDocuments = playerDocuments.OrderBy(m => m.FileName);

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<PlayerDocument>.CreateAsync(playerDocuments.AsNoTracking(), page ?? 1, pageSize);

            return View(pagedData);
        }

        // GET: PlayerDocuments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.PlayerDocuments == null)
            {
                return NotFound();
            }

            var playerDocument = await _context.PlayerDocuments.FindAsync(id);
            if (playerDocument == null)
            {
                return NotFound();
            }
            ViewData["PlayerId"] = new SelectList(_context.Players, "Id", "Email", playerDocument.PlayerId);
            return View(playerDocument);
        }

        // POST: PlayerDocuments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            var playerDocumentToUpdate = await _context.PlayerDocuments
               .Include(p => p.Player)
               .FirstOrDefaultAsync(p => p.Id == id);

            if (playerDocumentToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<PlayerDocument>(playerDocumentToUpdate, "",
                d => d.FileName))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return Redirect(ViewData["returnURL"].ToString());
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlayerDocumentExists(playerDocumentToUpdate.Id))
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
                    ModelState.AddModelError("", "Unable to save the update. Try again, and if the problem persists see your system administrator.");
                }
            }
            PopulateDropDownLists(playerDocumentToUpdate);
            return View(playerDocumentToUpdate);
        }

        // GET: PlayerDocuments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.PlayerDocuments == null)
            {
                return NotFound();
            }

            var playerDocument = await _context.PlayerDocuments
                .Include(p => p.Player)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (playerDocument == null)
            {
                return NotFound();
            }

            return View(playerDocument);
        }

        // POST: PlayerDocuments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.PlayerDocuments == null)
            {
                return Problem("Entity set 'PlayerManagementContext.PlayerDocuments'  is null.");
            }
            var playerDocument = await _context.PlayerDocuments.FindAsync(id);

            try
            {
                if (playerDocument != null)
                {
                    _context.PlayerDocuments.Remove(playerDocument);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save the update. Try again, and if the problem persists see your system administrator.");
            }
            return View(playerDocument);
        }

        public async Task<FileContentResult> Download(int id)
        {
            var theFile = await _context.UploadedFiles
                .Include(d => d.FileContent)
                .Where(f => f.Id == id)
                .FirstOrDefaultAsync();
            return File(theFile.FileContent.Content, theFile.MimeType, theFile.FileName);
        }

        private SelectList PlayerSelectList(int? id)
        {
            var dQuery = from d in _context.Players
                         orderby d.LastName, d.FirstName
                         select d;
            return new SelectList(dQuery, "Id", "FulllName", id);
        }
        private void PopulateDropDownLists(PlayerDocument playerDocument = null)
        {
            ViewData["PLayerID"] = PlayerSelectList(playerDocument?.PlayerId);
        }

        private bool PlayerDocumentExists(int id)
        {
          return _context.PlayerDocuments.Any(e => e.Id == id);
        }
    }
}
