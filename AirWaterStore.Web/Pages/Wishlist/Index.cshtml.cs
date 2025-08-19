using AirWaterStore.Business.Interfaces;
using AirWaterStore.Data.Models;
using AirWaterStore.Web.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AirWaterStore.Web.Pages.Wishlist
{
    public class IndexModel : PageModel
    {
        private readonly IWishlistService _wishlistService;
        private readonly IGameService _gameService;
        private const int PageSize = 9;

        public IndexModel(IWishlistService wishlistService, IGameService gameService)
        {
            _wishlistService = wishlistService;
            _gameService = gameService;
        }

        public List<Game> WishlistGames { get; set; } = new List<Game>();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public decimal TotalValue => WishlistGames.Sum(g => g.Price);
        public string? SuccessMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int currentPage = 1)
        {
            if (!this.IsAuthenticated() || !this.IsCustomer())
            {
                return RedirectToPage("/Login");
            }

            CurrentPage = currentPage;

            // Get success message from TempData
            var successMessage = TempData["SuccessMessage"];
            if (successMessage != null)
            {
                SuccessMessage = successMessage.ToString();
            }

            var userId = this.GetCurrentUserId();
            WishlistGames = await _wishlistService.GetWishlistGamesForUserAsync(userId, currentPage, PageSize);
            
            var totalCount = await _wishlistService.GetTotalCountByUserIdAsync(userId);
            TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

            return Page();
        }

        public async Task<IActionResult> OnPostAddToCartAsync(int gameId, int currentPage = 1)
        {
            if (!this.IsAuthenticated() || !this.IsCustomer())
            {
                return RedirectToPage("/Login");
            }

            var game = await _gameService.GetByIdAsync(gameId);
            if (game != null && game.Quantity > 0)
            {
                var cart = HttpContext.Session.GetObjectFromJson<List<Games.CartItem>>("Cart") ?? new List<Games.CartItem>();

                var existingItem = cart.FirstOrDefault(c => c.GameId == gameId);
                if (existingItem != null)
                {
                    existingItem.Quantity++;
                }
                else
                {
                    cart.Add(new Games.CartItem
                    {
                        GameId = gameId,
                        Title = game.Title,
                        Price = game.Price,
                        Quantity = 1
                    });
                }

                HttpContext.Session.SetObjectAsJson("Cart", cart);
                TempData["SuccessMessage"] = $"{game.Title} added to cart!";
            }

            return RedirectToPage(new { currentPage });
        }

        public async Task<IActionResult> OnPostAddAllToCartAsync()
        {
            if (!this.IsAuthenticated() || !this.IsCustomer())
            {
                return RedirectToPage("/Login");
            }

            var userId = this.GetCurrentUserId();
            var wishlistGames = await _wishlistService.GetWishlistGamesForUserAsync(userId, 1, 1000); // Get all
            
            var cart = HttpContext.Session.GetObjectFromJson<List<Games.CartItem>>("Cart") ?? new List<Games.CartItem>();
            var addedCount = 0;

            foreach (var game in wishlistGames.Where(g => g.Quantity > 0))
            {
                var existingItem = cart.FirstOrDefault(c => c.GameId == game.GameId);
                if (existingItem != null)
                {
                    existingItem.Quantity++;
                }
                else
                {
                    cart.Add(new Games.CartItem
                    {
                        GameId = game.GameId,
                        Title = game.Title,
                        Price = game.Price,
                        Quantity = 1
                    });
                }
                addedCount++;
            }

            HttpContext.Session.SetObjectAsJson("Cart", cart);
            TempData["SuccessMessage"] = $"{addedCount} games added to cart!";

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemoveFromWishlistAsync(int gameId, int currentPage = 1)
        {
            if (!this.IsAuthenticated() || !this.IsCustomer())
            {
                return RedirectToPage("/Login");
            }

            var userId = this.GetCurrentUserId();
    
            try
            {
                // Use the correct method to delete by user and game
                await _wishlistService.DeleteByUserAndGameAsync(userId, gameId);
        
                // Get the game title for the success message
                var game = await _gameService.GetByIdAsync(gameId);
                var gameTitle = game?.Title ?? "Game";
        
                TempData["SuccessMessage"] = $"{gameTitle} removed from wishlist!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unable to remove game from wishlist. Please try again.";
                // Log the error if you have logging
                Console.WriteLine($"Error removing from wishlist: {ex.Message}");
            }

            return RedirectToPage(new { currentPage });
        }

        public async Task<IActionResult> OnPostClearWishlistAsync()
        {
            if (!this.IsAuthenticated() || !this.IsCustomer())
            {
                return RedirectToPage("/Login");
            }

            var userId = this.GetCurrentUserId();
    
            try
            {
                // Use the new method to clear all user's wishlist items
                await _wishlistService.ClearUserWishlistAsync(userId);
                TempData["SuccessMessage"] = "Wishlist cleared successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unable to clear wishlist. Please try again.";
                Console.WriteLine($"Error clearing wishlist: {ex.Message}");
            }
    
            return RedirectToPage();
        }
    }
}