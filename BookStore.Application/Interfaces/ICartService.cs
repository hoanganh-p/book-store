using BookStore.Domain.Entities;

namespace BookStore.Application.Interfaces
{
    public interface ICartService
    {
        Task<Cart> GetCartAsync(string userId);
        Task AddItemToCartAsync(string userId, CartItem item);
        Task RemoveItemFromCartAsync(string userId, int itemId);
        Task UpdateItemQuantityAsync(string userId, int itemId, int quantity);
    }
}
