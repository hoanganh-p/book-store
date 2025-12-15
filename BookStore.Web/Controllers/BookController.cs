using BookStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Web.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        private readonly ICategoryService _categoryService;

        public BookController(IBookService bookService, ICategoryService categoryService)
        {
            _bookService = bookService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index(string keyword, int? categoryId, decimal? minPrice, decimal? maxPrice)
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewData["Categories"] = categories;

            var books = await _bookService.GetAllBooksAsync();

            if (!string.IsNullOrEmpty(keyword))
            {
                books = books.Where(b => b.Title.Contains(keyword) || b.Author.Contains(keyword) || b.ISBN.Contains(keyword)).ToList();
            }

            if (categoryId.HasValue)
            {
                books = books.Where(b => b.CategoryID == categoryId.Value).ToList();
            }

            if (minPrice.HasValue)
            {
                books = books.Where(b => b.Price >= minPrice.Value).ToList();
            }
            if (maxPrice.HasValue)
            {
                books = books.Where(b => b.Price <= maxPrice.Value).ToList();
            }

            return View(books);
        }

        public async Task<IActionResult> Search(string keyword)
        {
            var books = await _bookService.SearchBooksAsync(keyword);

            return View("Index", books);
        }

        public async Task<IActionResult> Details(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

    }
}
