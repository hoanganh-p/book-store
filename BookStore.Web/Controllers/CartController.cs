using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;
using BookStore.Infrastructure.Identity;

public class CartController : Controller
{
    private readonly ICartService _cartService;
    private readonly IBookService _bookService;
    private readonly UserManager<ApplicationUser> _userManager;

    public CartController(ICartService cartService, IBookService bookService, UserManager<ApplicationUser> userManager)
    {
        _cartService = cartService;
        _bookService = bookService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }

        var cart = await _cartService.GetCartAsync(userId);
        cart = cart ?? new Cart { UserID = userId };
        return View(cart);
    }


    [HttpPost]
    public async Task<IActionResult> AddToCart(int bookId)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }

        var book = await _bookService.GetBookByIdAsync(bookId);
        if (book == null)
        {
            return RedirectToAction("Index", "Home");
        }

        var cartItem = new CartItem
        {
            BookID = book.BookID,
            Price = book.Price,
            Quantity = 1
        };

        await _cartService.AddItemToCartAsync(userId, cartItem);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> UpdateItemQuantity(Dictionary<int, int> quantities)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }

        foreach (var quantity in quantities)
        {
            var itemId = quantity.Key;  
            var newQuantity = quantity.Value;

            if (newQuantity <= 0)
            {
                return BadRequest("Quantity must be greater than zero.");
            }

            await _cartService.UpdateItemQuantityAsync(userId, itemId, newQuantity);
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> RemoveFromCart(int itemId)
    {
        if (itemId <= 0)
        {
            return BadRequest("Invalid item ID.");
        }

        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }

        await _cartService.RemoveItemFromCartAsync(userId, itemId);
        return RedirectToAction("Index");
    }
}
