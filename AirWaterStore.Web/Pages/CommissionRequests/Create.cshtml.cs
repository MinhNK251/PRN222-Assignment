using AirWaterStore.Business.Interfaces;
using AirWaterStore.Data.Models;
using AirWaterStore.Data.Repositories;
using AirWaterStore.Web.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AirWaterStore.Web.Pages.CommissionRequests
{
    public class CreateModel : PageModel
    {
        private readonly ICommissionRequestService _service;

        public CreateModel(ICommissionRequestService service)
        {
            _service = service;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required]
            [StringLength(200)]
            public string GameTitle { get; set; }

            [Required]
            [StringLength(1000)]
            public string Description { get; set; }
        }

        public IActionResult OnGet()
        {
            var userId = HttpContext.Session.GetInt32(SessionParams.UserId);
            if (userId == null)
            {
                return RedirectToPage("/Login");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = HttpContext.Session.GetInt32(SessionParams.UserId);
            if (userId == null)
            {
                return RedirectToPage("/Login");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var request = new CommissionRequest
            {
                GameTitle = Input.GameTitle,
                Description = Input.Description,
                ExpectedPrice = 1,
                UserId = userId.Value,
                Status = "Open",
                Upvotes = 0,
                CreatedAt = DateTime.UtcNow
            };

            await _service.AddAsync(request);

            return RedirectToPage("./Index");
        }

        // New: Endpoint for autocomplete suggestions
        public async Task<JsonResult> OnGetSimilarGamesAsync(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return new JsonResult(new string[0]);

            var matches = await _service.GetAllAsync(); // ideally, add a dedicated GetGameTitlesContainingAsync
            var results = matches
                .Where(c => c.GameTitle.Contains(term, StringComparison.OrdinalIgnoreCase))
                .Select(c => c.GameTitle)
                .Distinct()
                .Take(5)
                .ToList();

            return new JsonResult(results);
        }
    }
}
