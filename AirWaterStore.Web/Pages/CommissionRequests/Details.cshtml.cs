using AirWaterStore.Business.Interfaces;
using AirWaterStore.Data.Models;
using AirWaterStore.Data.Repositories;
using AirWaterStore.Web.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AirWaterStore.Web.Pages.CommissionRequests
{
    public class DetailsModel : PageModel
    {
        private readonly ICommissionRequestService _commissionRequestService;
        private readonly IUserService _userService;
        private readonly IRequestUpvoteService _upvoteService;

        public DetailsModel(
            ICommissionRequestService commissionRequestService,
            IUserService userService,
            IRequestUpvoteService upvoteService)
        {
            _commissionRequestService = commissionRequestService;
            _userService = userService;
            _upvoteService = upvoteService;
        }

        public CommissionRequest Request { get; set; } = default!;
        public string CreatorUsername { get; set; } = "Unknown User";
        public bool AlreadyUpvoted { get; set; }

        [BindProperty]
        public NewRequestInputModel NewRequest { get; set; } = new();

        public class NewRequestInputModel
        {
            [Required, StringLength(200)]
            public string GameTitle { get; set; } = string.Empty;

            [Required, Range(0, 1000)]
            public decimal ExpectedPrice { get; set; }

            [StringLength(1000)]
            public string? Description { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var req = await _commissionRequestService.GetByIdAsync(id);
            if (req == null) return NotFound();

            Request = req;

            var user = await _userService.GetByIdAsync(req.UserId);
            CreatorUsername = user?.Username ?? "Unknown User";

            if (this.IsAuthenticated())
            {
                AlreadyUpvoted = await _upvoteService.HasUserUpvotedAsync(id, this.GetCurrentUserId());
            }

            return Page();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (!this.IsAuthenticated() || !this.IsCustomer())
                return RedirectToPage("/Login");

            if (!ModelState.IsValid)
                return Page();

            var req = new CommissionRequest
            {
                UserId = this.GetCurrentUserId(),
                GameTitle = NewRequest.GameTitle,
                ExpectedPrice = NewRequest.ExpectedPrice,
                Description = NewRequest.Description,
                Status = "Open",
                Upvotes = 0,
                CreatedAt = DateTime.UtcNow
            };

            await _commissionRequestService.AddAsync(req);
            return RedirectToPage("/CommissionRequests/Index");
        }

        public async Task<IActionResult> OnPostUpvoteAsync(int requestId)
        {
            if (!this.IsAuthenticated() || !this.IsCustomer())
                return RedirectToPage("/Login");

            await _upvoteService.UpvoteRequestAsync(requestId, this.GetCurrentUserId());

            return RedirectToPage(new { id = requestId });
        }
    }
}
