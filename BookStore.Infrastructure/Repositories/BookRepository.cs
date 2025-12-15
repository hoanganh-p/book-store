using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using BookStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Repositories
{
    public class BookRepository : GenericRepository<Book>, IBookRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<Book> _dbSet;

        public BookRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
            _dbSet = _context.Books;
        }

        public async Task<IEnumerable<Book>> SearchBooksAsync(string keyword)
        {
            return await _dbSet
                .Where(book => book.Title.Contains(keyword) || book.Author.Contains(keyword) || book.ISBN.Contains(keyword))
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetBooksByCategoryAsync(int categoryId)
        {
            return await _dbSet
                .Where(b => b.CategoryID == categoryId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetBooksByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _dbSet
                .Where(b => b.Price >= minPrice && b.Price <= maxPrice)
                .ToListAsync();
        }
    }
}
