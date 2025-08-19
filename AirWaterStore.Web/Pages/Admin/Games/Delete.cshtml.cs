using AirWaterStore.Business.Interfaces;
using AirWaterStore.Business.Services;
using AirWaterStore.Data.Models;
using AirWaterStore.Web.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AirWaterStore.Web.Pages.Admin.Games
{
    public class DeleteModel : PageModel
    {
        private readonly IGameService _gameService;
        private readonly CloudinaryService _cloudinaryService;

        public DeleteModel(IGameService gameService, CloudinaryService cloudinaryService)
        {
            _gameService = gameService;
            _cloudinaryService = cloudinaryService;
        }

        [BindProperty]
        public Game Game { get; set; } = default!; 
        [BindProperty]
        public string? ThumbnailUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Check if user is staff
            if (!this.IsStaff())
            {
                return RedirectToPage("/Login");
            }

            var game = await _gameService.GetByIdAsync(id);

            if (game == null)
            {
                return NotFound();
            }

            Game = game;
            ThumbnailUrl = game.ThumbnailUrl;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (HttpContext.Session.GetInt32(SessionParams.UserRole) != 2)
            {
                return RedirectToPage("/Login");
            }

            try
            {
                if (!string.IsNullOrEmpty(ThumbnailUrl) && ThumbnailUrl.Contains("res.cloudinary.com"))
                {
                    var uri = new Uri(ThumbnailUrl);
                    var segments = uri.Segments;
                    string publicId = Path.GetFileNameWithoutExtension(segments.Last());
                    await _cloudinaryService.DeleteImageAsync(publicId);
                }
                await _gameService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Game deleted successfully!";
                return RedirectToPage("/Games/Index");
            }
            catch
            {
                // If deletion fails (e.g., due to foreign key constraints)
                TempData["ErrorMessage"] = "Cannot delete this game because it has associated orders or reviews.";
                return RedirectToPage("/Games/Details", new { id = id });
            }
        }
    }
}