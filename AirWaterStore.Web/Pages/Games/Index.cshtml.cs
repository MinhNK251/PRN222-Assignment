using AirWaterStore.Business.Interfaces;
using AirWaterStore.Data.Models;
using AirWaterStore.Web.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AirWaterStore.Web.Pages.Games
{
    public class IndexModel : PageModel
    {
        private readonly IGameService _gameService;
        private readonly IWishlistService _wishlistService;
        private const int PageSize = 9;

        public IndexModel(IGameService gameService, IWishlistService wishlistService)
        {
            _gameService = gameService;
            _wishlistService = wishlistService;
        }

        public List<Game> Games { get; set; } = new List<Game>();
        public Dictionary<int, bool> GameWishlistStatus { get; set; } = new Dictionary<int, bool>();

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; } = string.Empty;
        public int TotalPages { get; set; }
        public string? SuccessMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Get success message from TempData
            var successMessage = TempData["SuccessMessage"];
            if (successMessage != null)
            {
                SuccessMessage = successMessage.ToString();
            }

            var allGames = await _gameService.GetAllAsync(1, 1000); // Get all for filtering

            // Filter by search string
            if (!string.IsNullOrEmpty(SearchString))
            {
                allGames = allGames.Where(g =>
                    g.Title.Contains(SearchString, StringComparison.OrdinalIgnoreCase) ||
                    (g.Genre?.Contains(SearchString, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (g.Developer?.Contains(SearchString, StringComparison.OrdinalIgnoreCase) ?? false)
                ).ToList();
            }

            // Calculate pagination
            var totalCount = allGames.Count;
            TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

            // Get paginated results
            Games = allGames
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            // Check wishlist status for each game
            if (this.IsAuthenticated() && this.IsCustomer())
            {
                var userId = this.GetCurrentUserId();
                GameWishlistStatus.Clear();
                
                foreach (var game in Games)
                {
                    try
                    {
                        var isInWishlist = await _wishlistService.HasUserWishlistedAsync(game.GameId, userId);
                        GameWishlistStatus[game.GameId] = isInWishlist;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error checking wishlist status for game {game.GameId}: {ex.Message}");
                        GameWishlistStatus[game.GameId] = false;
                    }
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAddToCartAsync(int gameId)
        {
            if (!this.IsAuthenticated() || !this.IsCustomer())
            {
                return RedirectToPage("/Login");
            }

            // Get or create cart in session
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            var existingItem = cart.FirstOrDefault(c => c.GameId == gameId);
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                var game = await _gameService.GetByIdAsync(gameId);
                if (game != null && game.Quantity > 0)
                {
                    cart.Add(new CartItem
                    {
                        GameId = gameId,
                        Title = game.Title,
                        Price = game.Price,
                        Quantity = 1
                    });
                }
            }

            HttpContext.Session.SetObjectAsJson("Cart", cart);
            TempData["SuccessMessage"] = "Game added to cart!";

            return RedirectToPage(null, new { CurrentPage, SearchString });
        }

        public async Task<IActionResult> OnPostToggleWishlistAsync(int gameId)
        {
            if (!this.IsAuthenticated() || !this.IsCustomer())
            {
                return RedirectToPage("/Login");
            }

            var userId = this.GetCurrentUserId();
            
            try
            {
                // Check if game is currently in wishlist
                var isInWishlist = await _wishlistService.HasUserWishlistedAsync(gameId, userId);
                
                if (isInWishlist)
                {
                    // Remove from wishlist using the correct method
                    await _wishlistService.DeleteByUserAndGameAsync(userId, gameId);
                    
                    // Get game title for better message
                    var game = await _gameService.GetByIdAsync(gameId);
                    var gameTitle = game?.Title ?? "Game";
                    
                    TempData["SuccessMessage"] = $"{gameTitle} removed from wishlist!";
                }
                else
                {
                    // Add to wishlist
                    var wishlist = new Data.Models.Wishlist
                    {
                        UserId = userId,
                        GameId = gameId,
                        CreatedAt = DateTime.Now
                    };
                    await _wishlistService.AddAsync(wishlist);
                    
                    // Get game title for better message
                    var game = await _gameService.GetByIdAsync(gameId);
                    var gameTitle = game?.Title ?? "Game";
                    
                    TempData["SuccessMessage"] = $"{gameTitle} added to wishlist!";
                }
            }
            catch (Exception ex)
            {
                // Log error for debugging
                Console.WriteLine($"Error toggling wishlist for gameId {gameId}, userId {userId}: {ex.Message}");
                TempData["ErrorMessage"] = "Unable to update wishlist. Please try again.";
            }

            return RedirectToPage(new { CurrentPage, SearchString });
        }

        public bool IsGameInWishlist(int gameId)
        {
            return GameWishlistStatus.TryGetValue(gameId, out var isInWishlist) && isInWishlist;
        }
    }

    public class CartItem
    {
        public int GameId { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}