using BookStore.Domain.Entities;

namespace BookStore.Application.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetAllBooksAsync();
        
        Task<Book> GetBookByIdAsync(int id);
        
        Task<IEnumerable<Book>> SearchBooksAsync(string keyword);

        Task<IEnumerable<Book>> GetBooksByCategoryAsync(int categoryId);

        Task<IEnumerable<Book>> GetBooksByPriceRangeAsync(decimal minPrice, decimal maxPrice);

        Task AddBookAsync(Book book);

        Task UpdateBookAsync(Book book);

        Task DeleteBookAsync(int id);
    }
}
