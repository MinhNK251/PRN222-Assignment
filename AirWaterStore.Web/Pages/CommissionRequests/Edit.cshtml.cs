using AirWaterStore.Business.Interfaces;
using AirWaterStore.Data.Models;
using AirWaterStore.Web.Helper;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AirWaterStore.Web.Pages.CommissionRequests
{
    public class EditModel : PageModel
    {
        private readonly ICommissionRequestService _commissionRequestService;

        public EditModel(ICommissionRequestService commissionRequestService)
        {
            _commissionRequestService = commissionRequestService;
        }

        [BindProperty]
        public EditRequestInputModel EditRequest { get; set; } = new();

        public bool IsStaff { get; set; }

        public class EditRequestInputModel
        {
            public int CommissionRequestId { get; set; }

            [Required, StringLength(200)]
            public string GameTitle { get; set; } = string.Empty;

            [Required]
            public decimal ExpectedPrice { get; set; } = 1;

            [StringLength(1000)]
            public string? Description { get; set; }

            [Required]
            public string Status { get; set; } = "Open";
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var req = await _commissionRequestService.GetByIdAsync(id);
            if (req == null) return NotFound();

            var userRole = HttpContext.Session.GetInt32(SessionParams.UserRole);
            IsStaff = userRole == 2;

            // Only forbid if not staff AND not the owner
            if (!IsStaff && req.UserId != this.GetCurrentUserId())
            {
                return Forbid(); // or redirect
            }

            EditRequest = new EditRequestInputModel
            {
                CommissionRequestId = req.CommissionRequestId,
                GameTitle = req.GameTitle,
                ExpectedPrice = req.ExpectedPrice,
                Description = req.Description,
                Status = req.Status
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var req = await _commissionRequestService.GetByIdAsync(EditRequest.CommissionRequestId);
            if (req == null) return NotFound();

            var userRole = HttpContext.Session.GetInt32(SessionParams.UserRole);
            IsStaff = userRole == 2;

            // Only forbid if not staff AND not the owner
            if (!IsStaff && req.UserId != this.GetCurrentUserId())
            {
                return Forbid(); // or redirect
            }

            // Update editable fields
            req.GameTitle = EditRequest.GameTitle;
            req.ExpectedPrice = EditRequest.ExpectedPrice;
            req.Description = EditRequest.Description;

            // Only staff can change Status
            if (IsStaff)
                req.Status = EditRequest.Status;

            await _commissionRequestService.UpdateAsync(req);

            return RedirectToPage("./Details", new { id = req.CommissionRequestId });
        }
    }
}
