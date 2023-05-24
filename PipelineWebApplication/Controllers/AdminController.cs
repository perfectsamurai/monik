using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using monitoring;
using monitoring.Data;
using monitoring.Models;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;


namespace monitoring.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly MonitoringSystemContext _context;

        public AdminController(MonitoringSystemContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var ngdus = await _context.Ngdus.ToListAsync();
            return View(ngdus);
        }

        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.Ngdus == null)

            {
                return NotFound();
            }

            var ngdus = await _context.Ngdus.FindAsync(id);
            if (ngdus == null)
            {
                return NotFound();
            }

            return View(ngdus);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("NgduId, Name")] Ngdu ngdus)
        {
            if (id != ngdus.NgduId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ngdus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NgdusExists(ngdus.NgduId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }


            return View(ngdus);
        }

        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ngdus = await _context.Ngdus
                .FirstOrDefaultAsync(d => d.NgduId == id);

            if (ngdus == null)
            {
                return NotFound();
            }

            return View(ngdus);
        }

        // POST: Dynamogram/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var ngdus = await _context.Ngdus.FindAsync(id);
           
            var workshop = _context.Workshops.FirstOrDefault(x => x.NgduId == id);

           

            if (workshop != null)
            {
                ModelState.AddModelError("", "К НГДУ привязаны цеха");
                
            }
            else
            {
                _context.Ngdus.Remove(ngdus);
            }
   
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: PipelineDatums/Create
        [Authorize]
        public IActionResult Create()
        {
           


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NgduId,Name")] Ngdu ngdus)
        {
            if (ModelState.IsValid)
            {
                   
                _context.Add(ngdus);
                await _context.SaveChangesAsync();
              
                return RedirectToAction(nameof(Index));
            }

            return View(ngdus);
        }

        private bool NgdusExists(long id)
        {
            return _context.Ngdus.Any(d => d.NgduId == id);
        }
    }
}
