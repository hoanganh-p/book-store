using BookStore.Domain.Entities;

namespace BookStore.Domain.Interfaces
{
    public interface IBookRepository : IGenericRepository<Book> 
    {
        Task<IEnumerable<Book>> SearchBooksAsync(string keyword);

        Task<IEnumerable<Book>> GetBooksByCategoryAsync(int categoryId);

        Task<IEnumerable<Book>> GetBooksByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    }
}
