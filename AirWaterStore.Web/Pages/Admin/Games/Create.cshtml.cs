using AirWaterStore.Business.Interfaces;
using AirWaterStore.Data.Models;
using AirWaterStore.Web.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AirWaterStore.Web.Pages.Admin.Games
{
    public class CreateModel : PageModel
    {
        private readonly IGameService _gameService;

        public CreateModel(IGameService gameService)
        {
            _gameService = gameService;
        }

        [BindProperty]
        public Game Game { get; set; } = default!;

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