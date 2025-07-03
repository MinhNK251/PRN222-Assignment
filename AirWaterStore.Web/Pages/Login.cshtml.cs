using AirWaterStore.Business.Interfaces;
using AirWaterStore.Web.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AirWaterStore.Web.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IUserService _userService;

        public LoginModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public LoginInputModel LoginInput { get; set; } = default!;

        public string ErrorMessage { get; set; } = string.Empty;

        public class LoginInputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;
        }

        public void OnGet()
        {
            // Clear session on login page
            HttpContext.Session.Clear();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userService.LoginAsync(LoginInput.Email, LoginInput.Password);

            if (user != null)
            {
                if (user.IsBan == true)
                {
                    ErrorMessage = "Your account has been banned.";
                    return Page();
                }

                // Set session
                HttpContext.Session.SetInt32(SessionParams.UserId, user.UserId);
                HttpContext.Session.SetString(SessionParams.UserName, user.Username);
                HttpContext.Session.SetInt32(SessionParams.UserRole, user.Role);

                // Redirect based on role
                if (user.Role == 2) // Staff
                {
                    return RedirectToPage("/Admin/Dashboard");
                }
                else // Customer
                {
                    return RedirectToPage("/Games/Index");
                }
            }

            ErrorMessage = "Invalid email or password.";
            return Page();
        }
    }
}