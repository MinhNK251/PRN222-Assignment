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
        private const int PageSize = 9;

        public IndexModel(IGameService gameService)
        {
            _gameService = gameService;
        }

        public List<Game> Games { get; set; } = new List<Game>();

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; } = string.Empty;
        public int TotalPages { get; set; }
        public string? SuccessMessage { get; set; }

        // public bool IsAuthenticated => HttpContext.Session.GetInt32(SessionParams.UserId).HasValue;
        // public bool IsCustomer => HttpContext.Session.GetInt32(SessionParams.UserRole) == 1;
        // public bool IsStaff => HttpContext.Session.GetInt32(SessionParams.UserRole) == 2;

        // public async Task<IActionResult> OnGetAsync(int currentPage = 1, string searchString = "")
        public async Task<IActionResult> OnGetAsync()
        {
            // CurrentPage = currentPage;
            // SearchString = searchString;

            // Get success message from TempData
            var successMessgae = TempData["SuccessMessage"];
            if (successMessgae != null)
            {
                SuccessMessage = successMessgae.ToString();
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

            return RedirectToPage(null, new
            { CurrentPage, SearchString });
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