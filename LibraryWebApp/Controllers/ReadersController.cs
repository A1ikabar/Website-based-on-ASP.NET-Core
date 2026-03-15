using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryWebApp.Data;
using LibraryWebApp.Models;

namespace LibraryWebApp.Controllers
{
    public class ReadersController : Controller
    {
        private readonly LibraryContext _context;

        public ReadersController(LibraryContext context)
        {
            _context = context;
        }

        // GET: Readers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Readers.AsNoTracking().ToListAsync());
        }

        // GET: Readers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var reader = await _context.Readers
                .Include(r => r.Borrowings)
                    .ThenInclude(b => b.Book)
                .Include(r => r.Reviews)
                    .ThenInclude(rv => rv.Book)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (reader == null) return NotFound();
            return View(reader);
        }

        // GET: Readers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Readers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FullName,LibraryCardNumber,PhoneNumber")] Reader reader)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reader);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(reader);
        }

        // GET: Readers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var reader = await _context.Readers.FindAsync(id);
            if (reader == null) return NotFound();
            return View(reader);
        }

        // POST: Readers/Edit/5 - УПРОЩЕННАЯ ВЕРСИЯ!
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,LibraryCardNumber,PhoneNumber")] Reader reader)
        {
            if (id != reader.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(reader);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(reader);
        }

        // GET: Readers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var reader = await _context.Readers
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (reader == null) return NotFound();
            return View(reader);
        }

        // POST: Readers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reader = await _context.Readers.FindAsync(id);
            if (reader != null)
            {
                _context.Readers.Remove(reader);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}