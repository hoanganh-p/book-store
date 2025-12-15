using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;

namespace BookStore.Application.Services
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            return await _unitOfWork.GetRepository<Book>().GetAllAsync();
        }

        public async Task<Book> GetBookByIdAsync(int id)
        {
            return await _unitOfWork.GetRepository<Book>().GetByIdAsync(id);
        }

        public async Task<IEnumerable<Book>> SearchBooksAsync(string keyword)
        {
            return await _unitOfWork
                .GetRepository<Book>()
                .FindAsync(book => book.Title.Contains(keyword) || book.Author.Contains(keyword) || book.ISBN.Contains(keyword));
        }
        public async Task<IEnumerable<Book>> GetBooksByCategoryAsync(int categoryId)
        {
            var books = await _unitOfWork.GetRepository<Book>()
                .FindAsync(b => b.CategoryID == categoryId);
            return books;
        }

        public async Task<IEnumerable<Book>> GetBooksByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            var books = await _unitOfWork.GetRepository<Book>()
                .FindAsync(b => b.Price >= minPrice && b.Price <= maxPrice);
            return books;
        }

        public async Task AddBookAsync(Book book)
        {
            await _unitOfWork.GetRepository<Book>().AddAsync(book);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateBookAsync(Book book)
        {
            await _unitOfWork.GetRepository<Book>().UpdateAsync(book);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteBookAsync(int id)
        {
            await _unitOfWork.GetRepository<Book>().DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }

}
