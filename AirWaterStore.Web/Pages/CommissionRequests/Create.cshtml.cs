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
        private readonly ICommissionRequestRepository _repository;

        public CreateModel(ICommissionRequestRepository repository)
        {
            _repository = repository;
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

            [Range(0, 9999)]
            public decimal ExpectedPrice { get; set; }
        }

        public IActionResult OnGet()
        {
            // Require login
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
                ExpectedPrice = Input.ExpectedPrice,
                UserId = userId.Value,
                Status = "Open",
                Upvotes = 0,
                CreatedAt = DateTime.UtcNow
                
            };

            await _repository.AddAsync(request);

            return RedirectToPage("./Index");
        }
    }
}
