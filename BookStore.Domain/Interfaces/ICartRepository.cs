using BookStore.Domain.Entities;

namespace BookStore.Domain.Interfaces
{
    public interface ICartRepository 
    {
        Task<Cart> GetCartAsync(string userId);
        Task AddItemToCartAsync(string userId, CartItem item);
        Task RemoveItemFromCartAsync(string userId, int itemId);
        Task UpdateItemQuantityAsync(string userId, int itemId, int quantity);
        Task SaveAsync();
    }
}
