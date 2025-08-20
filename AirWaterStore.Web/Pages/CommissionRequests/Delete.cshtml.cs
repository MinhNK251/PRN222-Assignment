using AirWaterStore.Business.Interfaces;
using AirWaterStore.Data.Models;
using AirWaterStore.Web.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace AirWaterStore.Web.Pages.CommissionRequests
{
    public class DeleteModel : PageModel
    {
        private readonly ICommissionRequestService _commissionRequestService;

        public DeleteModel(ICommissionRequestService commissionRequestService)
        {
            _commissionRequestService = commissionRequestService;
        }

        [BindProperty]
        public CommissionRequest CommissionRequest { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (!this.IsAuthenticated())
                return RedirectToPage("/Login");

            CommissionRequest = await _commissionRequestService.GetByIdAsync(id);
            if (CommissionRequest == null)
                return NotFound();

            var currentUserId = this.GetCurrentUserId();
            if (CommissionRequest.UserId != currentUserId)
                return Forbid();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!this.IsAuthenticated())
                return RedirectToPage("/Login");

            var existingRequest = await _commissionRequestService.GetByIdAsync(id);
            if (existingRequest == null)
                return NotFound();

            var currentUserId = this.GetCurrentUserId();
            if (existingRequest.UserId != currentUserId)
                return Forbid();

            await _commissionRequestService.DeleteAsync(id);
            return RedirectToPage("./Index");
        }
    }
}
