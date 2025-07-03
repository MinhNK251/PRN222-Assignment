using AirWaterStore.Business.Interfaces;
using AirWaterStore.Data.Models;
using AirWaterStore.Web.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AirWaterStore.Web.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly IUserService _userService;

        public RegisterModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public RegisterInputModel RegisterInput { get; set; } = default!;

        public string ErrorMessage { get; set; } = string.Empty;

        public class RegisterInputModel
        {
            [Required]
            [StringLength(50, MinimumLength = 3)]
            public string Username { get; set; } = string.Empty;

            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            [StringLength(100, MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var newUser = new User
                {
                    Username = RegisterInput.Username,
                    Email = RegisterInput.Email,
                    Password = RegisterInput.Password,
                    Role = 1, // Customer by default
                    IsBan = false
                };

                await _userService.AddAsync(newUser);

                // Auto-login after registration
                HttpContext.Session.SetInt32(SessionParams.UserId, newUser.UserId);
                HttpContext.Session.SetString(SessionParams.UserName, newUser.Username);
                HttpContext.Session.SetInt32(SessionParams.UserRole, newUser.Role);

                return RedirectToPage("/Games/Index");
            }
            catch (Exception ex)
            {
                // Handle duplicate username/email
                if (ex.InnerException?.Message.Contains("UNIQUE") == true)
                {
                    if (ex.InnerException.Message.Contains("Email"))
                        ErrorMessage = "This email is already registered.";
                    else if (ex.InnerException.Message.Contains(SessionParams.UserName))
                        ErrorMessage = "This username is already taken.";
                    else
                        ErrorMessage = "Registration failed. Please try again.";
                }
                else
                {
                    ErrorMessage = "An error occurred during registration.";
                }

                return Page();
            }
        }
    }
}