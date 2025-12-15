using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;

namespace BookStore.Application.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IUnitOfWork _unitOfWork;


        public CartService(ICartRepository cartRepository, IUnitOfWork unitOfWork)
        {
            _cartRepository = cartRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Cart> GetCartAsync(string userId)
        {
            return await _cartRepository.GetCartAsync(userId);
        }

        public async Task AddItemToCartAsync(string userId, CartItem item)
        {
            await _cartRepository.AddItemToCartAsync(userId, item);
        }

        public async Task RemoveItemFromCartAsync(string userId, int itemId)
        {
            await _cartRepository.RemoveItemFromCartAsync(userId, itemId);
        }

        public async Task UpdateItemQuantityAsync(string userId, int itemId, int quantity)
        {
            await _cartRepository.UpdateItemQuantityAsync(userId, itemId, quantity);
        }
    }
}
