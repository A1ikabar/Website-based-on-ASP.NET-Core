using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryWebApp.Data;
using LibraryWebApp.Models;

namespace LibraryWebApp.Controllers
{
    public class BorrowingsController : Controller
    {
        private readonly LibraryContext _context;

        public BorrowingsController(LibraryContext context)
        {
            _context = context;
        }

        // GET: Borrowings
        public async Task<IActionResult> Index()
        {
            var borrowings = await _context.Borrowings
                .Include(b => b.Reader)
                .Include(b => b.Book)
                .AsNoTracking()
                .ToListAsync();
            return View(borrowings);
        }

        // GET: Borrowings/Details/5
        public async Task<IActionResult> Details(int? readerId, int? bookId)
        {
            if (readerId == null || bookId == null) return NotFound();

            var borrowing = await _context.Borrowings
                .Include(b => b.Reader)
                .Include(b => b.Book)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.ReaderId == readerId && b.BookId == bookId);

            if (borrowing == null) return NotFound();
            return View(borrowing);
        }

        // GET: Borrowings/Create
        public async Task<IActionResult> Create()
        {
            await FillReadersDropDown();
            await FillAvailableBooksDropDown();
            return View();
        }

        // POST: Borrowings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReaderId,BookId,BorrowedAt,ReturnedAt")] Borrowing borrowing)
        {
            if (borrowing.BorrowedAt == default)
                borrowing.BorrowedAt = DateTime.Now;

            // Проверка на уже существующую активную выдачу
            var exists = await _context.Borrowings
                .AnyAsync(b => b.BookId == borrowing.BookId && b.ReturnedAt == null);

            if (exists)
            {
                ModelState.AddModelError("BookId", "This book is already borrowed");
            }

            if (ModelState.IsValid)
            {
                _context.Add(borrowing);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await FillReadersDropDown(borrowing.ReaderId);
            await FillAvailableBooksDropDown(borrowing.BookId);
            return View(borrowing);
        }

        // GET: Borrowings/Edit/5
        public async Task<IActionResult> Edit(int? readerId, int? bookId)
        {
            if (readerId == null || bookId == null) return NotFound();

            var borrowing = await _context.Borrowings
                .FindAsync(readerId, bookId);

            if (borrowing == null) return NotFound();

            await FillReadersDropDown(borrowing.ReaderId);
            await FillBooksDropDown(borrowing.BookId);
            return View(borrowing);
        }

        // POST: Borrowings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int readerId, int bookId, [Bind("ReaderId,BookId,BorrowedAt,ReturnedAt")] Borrowing borrowing)
        {
            if (readerId != borrowing.ReaderId || bookId != borrowing.BookId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(borrowing);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Borrowings.AnyAsync(b => b.ReaderId == borrowing.ReaderId && b.BookId == borrowing.BookId))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            await FillReadersDropDown(borrowing.ReaderId);
            await FillBooksDropDown(borrowing.BookId);
            return View(borrowing);
        }

        // GET: Borrowings/Delete/5
        public async Task<IActionResult> Delete(int? readerId, int? bookId)
        {
            if (readerId == null || bookId == null) return NotFound();

            var borrowing = await _context.Borrowings
                .Include(b => b.Reader)
                .Include(b => b.Book)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.ReaderId == readerId && b.BookId == bookId);

            if (borrowing == null) return NotFound();
            return View(borrowing);
        }

        // POST: Borrowings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int readerId, int bookId)
        {
            var borrowing = await _context.Borrowings
                .FindAsync(readerId, bookId);

            if (borrowing != null)
            {
                _context.Borrowings.Remove(borrowing);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Borrowings/Return/5
        public async Task<IActionResult> Return(int? readerId, int? bookId)
        {
            if (readerId == null || bookId == null) return NotFound();

            var borrowing = await _context.Borrowings
                .Include(b => b.Reader)
                .Include(b => b.Book)
                .FirstOrDefaultAsync(b => b.ReaderId == readerId && b.BookId == bookId && b.ReturnedAt == null);

            if (borrowing == null) return NotFound();
            return View(borrowing);
        }

        // POST: Borrowings/Return/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReturnConfirmed(int readerId, int bookId)
        {
            var borrowing = await _context.Borrowings
                .FindAsync(readerId, bookId);

            if (borrowing != null)
            {
                borrowing.ReturnedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task FillReadersDropDown(int? selectedReaderId = null)
        {
            var readers = await _context.Readers
                .OrderBy(r => r.FullName)
                .AsNoTracking()
                .ToListAsync();
            ViewBag.ReaderId = new SelectList(readers, "Id", "FullName", selectedReaderId);
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

        private async Task FillAvailableBooksDropDown(int? selectedBookId = null)
        {
            var borrowedBookIds = await _context.Borrowings
                .Where(b => b.ReturnedAt == null)
                .Select(b => b.BookId)
                .ToListAsync();

            var availableBooks = await _context.Books
                .Include(b => b.Author)
                .Where(b => !borrowedBookIds.Contains(b.Id))
                .OrderBy(b => b.Title)
                .AsNoTracking()
                .ToListAsync();

            var bookList = availableBooks.Select(b => new
            {
                b.Id,
                Display = $"{b.Title} ({b.Author?.FullName})"
            });

            ViewBag.BookId = new SelectList(bookList, "Id", "Display", selectedBookId);
        }
    }
}