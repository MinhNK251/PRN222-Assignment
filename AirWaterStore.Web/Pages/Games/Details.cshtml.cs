using AirWaterStore.Business.Interfaces;
using AirWaterStore.Data.Models;
using AirWaterStore.Web.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AirWaterStore.Web.Pages.Games
{
    public class DetailsModel : PageModel
    {
        private readonly IGameService _gameService;
        private readonly IReviewService _reviewService;
        private readonly IUserService _userService;

        public DetailsModel(IGameService gameService, IReviewService reviewService, IUserService userService)
        {
            _gameService = gameService;
            _reviewService = reviewService;
            _userService = userService;
        }

        public Game Game { get; set; } = default!;
        public List<Review> Reviews { get; set; } = default!;
        public Dictionary<int, string> UserNames { get; set; } = new Dictionary<int, string>();

        [BindProperty]
        public ReviewInputModel NewReview { get; set; } = default!;

        // public int? CurrentUserId => HttpContext.Session.GetInt32(SessionParams.UserId);
        // public bool IsAuthenticated => CurrentUserId.HasValue;
        // public bool IsCustomer => HttpContext.Session.GetInt32(SessionParams.UserRole) == 1;
        // public bool IsStaff => HttpContext.Session.GetInt32(SessionParams.UserRole) == 2;
        public bool CanReview { get; set; }

        public class ReviewInputModel
        {
            public int GameId { get; set; }

            [Required]
            [Range(1, 5)]
            public int Rating { get; set; }

            [Required]
            [StringLength(1000)]
            public string Comment { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var game = await _gameService.GetByIdAsync(id);
            if (game == null)
            {
                return NotFound();
            }

            Game = game;

            Reviews = await _reviewService.GetAllByGameIdAsync(id);

            // Load usernames for reviews
            foreach (var review in Reviews)
            {
                if (!UserNames.ContainsKey(review.UserId))
                {
                    var user = await _userService.GetByIdAsync(review.UserId);
                    UserNames[review.UserId] = user?.Username ?? "Unknown User";
                }
            }

            // Check if current user can review (hasn't reviewed this game yet)
            if (this.IsCustomer() && this.IsAuthenticated())
            {
                CanReview = !Reviews.Any(r => r.UserId == this.GetCurrentUserId());
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAddToCartAsync(int gameId, int quantity = 1)
        {
            if (!this.IsAuthenticated() || !this.IsCustomer())
            {
                return RedirectToPage("/Login");
            }

            var game = await _gameService.GetByIdAsync(gameId);
            if (game == null || game.Quantity < quantity)
            {
                return RedirectToPage();
            }

            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            var existingItem = cart.FirstOrDefault(c => c.GameId == gameId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem
                {
                    GameId = gameId,
                    Title = game.Title,
                    Price = game.Price,
                    Quantity = quantity
                });
            }

            HttpContext.Session.SetObjectAsJson("Cart", cart);
            TempData["SuccessMessage"] = "Game added to cart!";

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAddReviewAsync()
        {
            if (!this.IsAuthenticated() || !this.IsCustomer())
            {
                return RedirectToPage("/Login");
            }

            if (!ModelState.IsValid || !this.IsAuthenticated())
            {
                return await OnGetAsync(NewReview.GameId);
            }

            var review = new Review
            {
                UserId = this.GetCurrentUserId(),
                GameId = NewReview.GameId,
                Rating = NewReview.Rating,
                Comment = NewReview.Comment,
                ReviewDate = DateTime.Now
            };

            await _reviewService.AddAsync(review);

            return RedirectToPage(new { id = NewReview.GameId });
        }

        public async Task<IActionResult> OnPostUpdateReviewAsync(int reviewId, int gameId, int rating, string comment)
        {
            // if (!this.IsAuthenticated() || !this.IsCustomer() || this.GetCurrentUserId() == null)
            if (!this.IsAuthenticated() || !this.IsCustomer())
            {
                return RedirectToPage("/Login");
            }

            var review = await _reviewService.GetByIdAsync(reviewId);
            if (review != null && review.UserId == this.GetCurrentUserId())
            {
                review.Rating = rating;
                review.Comment = comment;
                await _reviewService.UpdateAsync(review);
            }

            return RedirectToPage(new { id = gameId });
        }

        public async Task<IActionResult> OnPostDeleteReviewAsync(int reviewId)
        {
            // if (!this.IsAuthenticated() || !this.IsCustomer() || this.GetCurrentUserId() == null)
            if (!this.IsAuthenticated() || !this.IsCustomer())
            {
                return RedirectToPage("/Login");
            }

            var review = await _reviewService.GetByIdAsync(reviewId);
            if (review != null && review.UserId == this.GetCurrentUserId())
            {
                await _reviewService.DeleteAsync(reviewId);
            }

            return RedirectToPage(new { id = Game?.GameId ?? review?.GameId });
        }

        public string GetUsername(int userId)
        {
            return UserNames.TryGetValue(userId, out var username) ? username : "Unknown User";
        }
    }
}