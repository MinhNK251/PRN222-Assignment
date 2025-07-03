using AirWaterStore.Business.Interfaces;
using AirWaterStore.Data.Models;
using AirWaterStore.Web.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AirWaterStore.Web.Pages.Admin.Users
{
    public class IndexModel : PageModel
    {
        private readonly IUserService _userService;
        private const int PageSize = 10;

        public IndexModel(IUserService userService)
        {
            _userService = userService;
        }

        public List<User> Users { get; set; } = new List<User>();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public string? SuccessMessage { get; set; }
        // public int CurrentUserId => HttpContext.Session.GetInt32(SessionParams.UserId) ?? 0;

        public async Task<IActionResult> OnGetAsync(int currentPage = 1)
        {
            // Check if user is staff
            if (!this.IsStaff())
            {
                return RedirectToPage("/Login");
            }

            CurrentPage = currentPage;

            var successMessage = TempData["SuccessMessage"];
            if (successMessage != null)
            {
                SuccessMessage = successMessage.ToString();
            }

            Users = await _userService.GetAllAsync(this.GetCurrentUserId(), currentPage, PageSize);
            var totalCount = await _userService.GetTotalCountAsync();
            TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

            return Page();
        }

        public async Task<IActionResult> OnPostBanAsync(int userId)
        {
            if (HttpContext.Session.GetInt32(SessionParams.UserRole) != 2)
            {
                return Forbid();
            }

            var user = await _userService.GetByIdAsync(userId);
            if (user != null && user.UserId != this.GetCurrentUserId())
            {
                user.IsBan = true;
                await _userService.UpdateAsync(user);
                TempData["SuccessMessage"] = $"User {user.Username} has been banned.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUnbanAsync(int userId)
        {
            if (HttpContext.Session.GetInt32(SessionParams.UserRole) != 2)
            {
                return Forbid();
            }

            var user = await _userService.GetByIdAsync(userId);
            if (user != null)
            {
                user.IsBan = false;
                await _userService.UpdateAsync(user);
                TempData["SuccessMessage"] = $"User {user.Username} has been unbanned.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostMakeStaffAsync(int userId)
        {
            if (HttpContext.Session.GetInt32(SessionParams.UserRole) != 2)
            {
                return Forbid();
            }

            var user = await _userService.GetByIdAsync(userId);
            if (user != null && user.Role == 1)
            {
                user.Role = 2;
                await _userService.UpdateAsync(user);
                TempData["SuccessMessage"] = $"User {user.Username} has been promoted to Staff.";
            }

            return RedirectToPage();
        }
    }
}