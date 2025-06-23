using AirWaterStore.Business.Interfaces;
using AirWaterStore.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AirWaterStore.Web.Pages.Admin.Games
{
    public class DeleteModel : PageModel
    {
        private readonly IGameService _gameService;

        public DeleteModel(IGameService gameService)
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

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (HttpContext.Session.GetInt32("UserRole") != 2)
            {
                return RedirectToPage("/Login");
            }

            try
            {
                await _gameService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Game deleted successfully!";
                return RedirectToPage("/Games/Index");
            }
            catch (Exception ex)
            {
                // If deletion fails (e.g., due to foreign key constraints)
                TempData["ErrorMessage"] = "Cannot delete this game because it has associated orders or reviews.";
                return RedirectToPage("/Games/Details", new { id = id });
            }
        }
    }
}