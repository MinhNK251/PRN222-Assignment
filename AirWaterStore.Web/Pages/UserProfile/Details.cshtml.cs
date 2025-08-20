using AirWaterStore.Business.Interfaces;
using AirWaterStore.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AirWaterStore.Web.Pages.UserProfile
{
    public class DetailsModel : PageModel
    {
        private readonly IUserService _userService;

        public DetailsModel(IUserService userService)
        {
            _userService = userService;
        }
        [BindProperty]
        public User UserProfile { get; set; }
        public async Task<IActionResult> OnGetAsync(int? userid)
        {
            if (userid == null)
            {
                return NotFound();
            }

            var userProfile = await _userService.GetByIdAsync(userid.Value);

            if (userProfile == null)
            {
                return NotFound();
            }

            UserProfile = userProfile;

            return Page();
        }
    }
}
