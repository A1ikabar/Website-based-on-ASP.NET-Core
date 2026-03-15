using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryWebApp.Data;
using LibraryWebApp.Models;

namespace LibraryWebApp.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly LibraryContext _context;

        public ReviewsController(LibraryContext context)
        {
            _context = context;
        }

        // GET: Reviews
        public async Task<IActionResult> Index()
        {
            var reviews = await _context.Reviews
                .Include(r => r.Book)
                .Include(r => r.Reader)
                .AsNoTracking()
                .ToListAsync();
            return View(reviews);
        }

        // GET: Reviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var review = await _context.Reviews
                .Include(r => r.Book)
                .Include(r => r.Reader)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (review == null) return NotFound();
            return View(review);
        }

        // GET: Reviews/Create
        public async Task<IActionResult> Create()
        {
            await FillBooksDropDown();
            await FillReadersDropDown();
            return View();
        }

        // POST: Reviews/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookId,ReaderId,Rating,Comment")] Review review)
        {
            review.ReviewDate = DateTime.Now;

            // Простая проверка на повторный отзыв
            var exists = await _context.Reviews
                .AnyAsync(r => r.BookId == review.BookId && r.ReaderId == review.ReaderId);

            if (exists)
            {
                ModelState.AddModelError("", "This reader has already reviewed this book");
            }

            if (ModelState.IsValid)
            {
                _context.Add(review);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await FillBooksDropDown(review.BookId);
            await FillReadersDropDown(review.ReaderId);
            return View(review);
        }

        // GET: Reviews/Edit/5 - УПРОЩЕННАЯ ВЕРСИЯ!
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return NotFound();

            await FillBooksDropDown(review.BookId);
            await FillReadersDropDown(review.ReaderId);
            return View(review);
        }

        // POST: Reviews/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BookId,ReaderId,Rating,Comment,ReviewDate")] Review review)
        {
            if (id != review.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(review);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await FillBooksDropDown(review.BookId);
            await FillReadersDropDown(review.ReaderId);
            return View(review);
        }

        // GET: Reviews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var review = await _context.Reviews
                .Include(r => r.Book)
                .Include(r => r.Reader)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (review == null) return NotFound();
            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task FillBooksDropDown(int? selectedBookId = null)
        {
            var books = await _context.Books
                .Include(b => b.Author)
                .OrderBy(b => b.Title)
                .AsNoTracking()
                .ToListAsync();

            var bookList = books.Select(b => new
            {
                b.Id,
                Display = $"{b.Title} ({b.Author?.FullName})"
            });

            ViewBag.BookId = new SelectList(bookList, "Id", "Display", selectedBookId);
        }

        private async Task FillReadersDropDown(int? selectedReaderId = null)
        {
            var readers = await _context.Readers
                .OrderBy(r => r.FullName)
                .AsNoTracking()
                .ToListAsync();
            ViewBag.ReaderId = new SelectList(readers, "Id", "FullName", selectedReaderId);
        }
    }
}