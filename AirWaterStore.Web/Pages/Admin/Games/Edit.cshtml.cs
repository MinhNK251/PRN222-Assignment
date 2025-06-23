using AirWaterStore.Business.Interfaces;
using AirWaterStore.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AirWaterStore.Web.Pages.Admin.Games
{
    public class EditModel : PageModel
    {
        private readonly IGameService _gameService;

        public EditModel(IGameService gameService)
        {
            _gameService = gameService;
        }

        [BindProperty]
        public Game Game { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Check if user is staff
            if (HttpContext.Session.GetInt32("UserRole") != 2)
            {
                return RedirectToPage("/Login");
            }

            Game = await _gameService.GetByIdAsync(id);

            if (Game == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (HttpContext.Session.GetInt32("UserRole") != 2)
            {
                return RedirectToPage("/Login");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _gameService.UpdateAsync(Game);
                TempData["SuccessMessage"] = "Game updated successfully!";
                return RedirectToPage("/Games/Details", new { id = Game.GameId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while updating the game.");
                return Page();
            }
        }
    }
}