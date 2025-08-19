using AirWaterStore.Business.Interfaces;
using AirWaterStore.Business.Services;
using AirWaterStore.Data.Models;
using AirWaterStore.Web.Helper;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AirWaterStore.Web.Pages.Admin.Games
{
    public class CreateModel : PageModel
    {
        private readonly IGameService _gameService;
        private readonly CloudinaryService _cloudinaryService;

        public CreateModel(IGameService gameService, CloudinaryService cloudinaryService)
        {
            _gameService = gameService;
            _cloudinaryService = cloudinaryService;
        }

        [BindProperty]
        public Game Game { get; set; } = default!;

        [BindProperty]
        public IFormFile ThumbnailFile { get; set; }

        public IActionResult OnGet()
        {
            // Check if user is staff
            if (!this.IsStaff())
            {
                return RedirectToPage("/Login");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (HttpContext.Session.GetInt32(SessionParams.UserRole) != 2)
            {
                return RedirectToPage("/Login");
            }

            using (var stream = ThumbnailFile.OpenReadStream())
            {
                var imageUrl = await _cloudinaryService.UploadImageAsync(stream, ThumbnailFile.FileName);
                Game.ThumbnailUrl = imageUrl;
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _gameService.AddAsync(Game);

            TempData["SuccessMessage"] = "Game created successfully!";
            return RedirectToPage("/Games/Index");
        }
    }
}