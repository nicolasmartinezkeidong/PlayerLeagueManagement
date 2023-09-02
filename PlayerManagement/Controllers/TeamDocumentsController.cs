using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PlayerManagement.Data;
using PlayerManagement.Models;
using PlayerManagement.Utilities;

namespace PlayerManagement.Controllers
{
    [Authorize(Roles = "Admin, Captain")]
    public class TeamDocumentsController : CustomControllers.ElephantController
    {
        private readonly PlayerManagementContext _context;

        public TeamDocumentsController(PlayerManagementContext context)
        {
            _context = context;
        }

        // GET: TeamDocuments
        public async Task<IActionResult> Index(int? page, int? pageSizeID,
            int? TeamId, string SearchString)
        {
            //Clear the sort/filter/paging URL Cookie for Controller
            CookieHelper.CookieSet(HttpContext, ControllerName() + "URL", "", -1);

            PopulateDropDownLists();

            //Toggle the Open/Closed state of the collapse depending on if we are filtering
            ViewData["Filtering"] = "btn-outline-dark"; //Asume not filtering
            //Then in each "test" for filtering, add ViewData["Filtering"] = "btn-danger" if true;

            var teamDocuments = from t in _context.TeamDocuments
                .Include(t => t.Team)
                                  select t;

            if (TeamId.HasValue)
            {
                teamDocuments = teamDocuments.Where(p => p.TeamId == TeamId);
                ViewData["Filtering"] = "btn-danger";
            }
            if (!String.IsNullOrEmpty(SearchString))
            {
                teamDocuments = teamDocuments.Where(p => p.FileName.ToUpper().Contains(SearchString.ToUpper()));
                ViewData["Filtering"] = "btn-danger";
            }
            // Always sort by File Name
            teamDocuments = teamDocuments.OrderBy(m => m.FileName);

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, ControllerName());
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<TeamDocument>.CreateAsync(teamDocuments.AsNoTracking(), page ?? 1, pageSize);


            return View(pagedData);
        }

        // GET: TeamDocuments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TeamDocuments == null)
            {
                return NotFound();
            }

            var teamDocument = await _context.TeamDocuments
                .Include(t => t.Team)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);
            
            if (teamDocument == null)
            {
                return NotFound();
            }
            PopulateDropDownLists(teamDocument);
            return View(teamDocument);
        }

        // POST: TeamDocuments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            var teamDocumentToUpdate = await _context.TeamDocuments
                .Include(m => m.Team)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (teamDocumentToUpdate == null)
            {
                return NotFound();
            }
            if (await TryUpdateModelAsync<TeamDocument>(teamDocumentToUpdate, "",
                d => d.FileName))

            {
                try
                {
                    await _context.SaveChangesAsync();
                    return Redirect(ViewData["returnURL"].ToString());
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeamDocumentExists(teamDocumentToUpdate.Id))
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
            PopulateDropDownLists(teamDocumentToUpdate);
            return View(teamDocumentToUpdate);
        }

        // GET: TeamDocuments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TeamDocuments == null)
            {
                return NotFound();
            }

            var teamDocument = await _context.TeamDocuments
                .Include(t => t.Team)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (teamDocument == null)
            {
                return NotFound();
            }

            return View(teamDocument);
        }

        // POST: TeamDocuments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TeamDocuments == null)
            {
                return Problem("Entity set 'PlayerManagementContext.TeamDocuments'  is null.");
            }
            var teamDocument = await _context.TeamDocuments.FindAsync(id);
            try
            {
                if (teamDocument != null)
                {
                    _context.TeamDocuments.Remove(teamDocument);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save the update. Try again, and if the problem persists see your system administrator.");
            }
            return View(teamDocument);
        }

        public async Task<FileContentResult> Download(int id)
        {
            var theFile = await _context.UploadedFiles
                .Include(d => d.FileContent)
                .Where(f => f.Id == id)
                .FirstOrDefaultAsync();
            return File(theFile.FileContent.Content, theFile.MimeType, theFile.FileName);
        }

        private SelectList TeamSelectList(int? id)
        {
            var dQuery = from t in _context.Teams
                         orderby t.Name
                         select t;
            return new SelectList(dQuery, "Id", "Name", id);
        }
        private void PopulateDropDownLists(TeamDocument teamDocument = null)
        {
            ViewData["TeamID"] = TeamSelectList(teamDocument?.TeamId);
        }

        private bool TeamDocumentExists(int id)
        {
          return _context.TeamDocuments.Any(e => e.Id == id);
        }
    }
}
