using AirWaterStore.Business.Interfaces;
using AirWaterStore.Business.Services;
using AirWaterStore.Data.Models;
using AirWaterStore.Web.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AirWaterStore.Web.Pages.Admin.Games
{
    public class EditModel : PageModel
    {
        private readonly IGameService _gameService;
        private readonly CloudinaryService _cloudinaryService;

        public EditModel(IGameService gameService, CloudinaryService cloudinaryService)
        {
            _gameService = gameService;
            _cloudinaryService = cloudinaryService;
        }

        [BindProperty]
        public Game Game { get; set; } = default!;
        [BindProperty]
        public IFormFile? ThumbnailFile { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Check if user is staff
            if (HttpContext.Session.GetInt32(SessionParams.UserRole) != 2)
            {
                return RedirectToPage("/Login");
            }

            var game = await _gameService.GetByIdAsync(id);

            if (game == null)
            {
                return NotFound();
            }
            Game = game;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!this.IsStaff())
            {
                return RedirectToPage("/Login");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                if (ThumbnailFile != null)
                {
                    if (!string.IsNullOrEmpty(Game.ThumbnailUrl))
                    {
                        var uri = new Uri(Game.ThumbnailUrl);
                        var segments = uri.Segments;
                        string publicId = Path.GetFileNameWithoutExtension(segments.Last());
                        await _cloudinaryService.DeleteImageAsync(publicId);
                    }
                    using (var stream = ThumbnailFile.OpenReadStream())
                    {
                        var imageUrl = await _cloudinaryService.UploadImageAsync(stream, ThumbnailFile.FileName);
                        Game.ThumbnailUrl = imageUrl;
                    }
                }
                await _gameService.UpdateAsync(Game);
                TempData["SuccessMessage"] = "Game updated successfully!";
                return RedirectToPage("/Games/Details", new { id = Game.GameId });
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "An error occurred while updating the game.");
                return Page();
            }
        }
    }
}