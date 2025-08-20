using AirWaterStore.Business.Interfaces;
using AirWaterStore.Data.Models;
using AirWaterStore.Web.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace AirWaterStore.Web.Pages.UserProfile
{
    public class EditModel : PageModel
    {
        private readonly IUserService _userService;

        public EditModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public User UserProfile { get; set; }


        public async Task<IActionResult> OnGetAsync(int userid)
        {
            var user = await _userService.GetByIdAsync(userid);
            if (user == null)
            {
                return NotFound();
            }

            UserProfile = user;
            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userInDb = await _userService.GetByIdAsync(UserProfile.UserId);
            if (userInDb == null)
            {
                return NotFound();
            }

            userInDb.Username = UserProfile.Username;
            userInDb.Email = UserProfile.Email;
            userInDb.Password = UserProfile.Password;

            await _userService.UpdateAsync(userInDb);

            HttpContext.Session.SetInt32(SessionParams.UserId, userInDb.UserId);
            HttpContext.Session.SetString(SessionParams.UserName, userInDb.Username);
            HttpContext.Session.SetInt32(SessionParams.UserRole, userInDb.Role);

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToPage("/UserProfile/Details", new { userid = UserProfile.UserId });
        }

    }
}
