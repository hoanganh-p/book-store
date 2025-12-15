using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        private readonly ICategoryService _categoryService;
        private readonly IImageService _imageService;

        public BookController(IBookService bookService, ICategoryService categoryService, IImageService imageService)
        {
            _bookService = bookService;
            _categoryService = categoryService;
            _imageService = imageService;
        }

        public async Task<IActionResult> Index()
        {
            var books = await _bookService.GetAllBooksAsync();
            
            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewData["Categories"] = categories;
            
            return View(books);
        }

        public async Task<IActionResult> Search(string keyword)
        {
            var books = await _bookService.SearchBooksAsync(keyword);

            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewData["Categories"] = categories;

            return View("Index", books);
        }

        public async Task<IActionResult> Details(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewData["Categories"] = categories;
            
            return View(book);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["Categories"] = await _categoryService.GetAllCategoriesAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book, IFormFile bookImage)
        {
            if (bookImage != null && bookImage.Length > 0)
            {
                var imagePath = await _imageService.UploadImageAsync(bookImage, "books");
                book.ImageURL = imagePath;
            }

            if (ModelState.IsValid)
            {
                await _bookService.AddBookAsync(book);
                return RedirectToAction(nameof(Index));
            }

            ViewData["Categories"] = await _categoryService.GetAllCategoriesAsync();
            return View(book);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            
            ViewData["Categories"] = await _categoryService.GetAllCategoriesAsync();

            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Book book, IFormFile bookImage)
        {
            if (id != book.BookID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (bookImage != null && bookImage.Length > 0)
                {
                    var imageUrl = await _imageService.UploadImageAsync(bookImage, "books");
                    book.ImageURL = imageUrl;
                }

                await _bookService.UpdateBookAsync(book);
                return RedirectToAction(nameof(Index));
            }

            ViewData["Categories"] = await _categoryService.GetAllCategoriesAsync();
            return View(book);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _bookService.DeleteBookAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }

}
