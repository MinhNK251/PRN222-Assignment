using AirWaterStore.Business.Interfaces;
using AirWaterStore.Data.Models;
using AirWaterStore.Data.Repositories;
using AirWaterStore.Web.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AirWaterStore.Web.Pages.CommissionRequests
{
    public class IndexModel : PageModel
    {
        private readonly ICommissionRequestService _commissionRequestService;
        private readonly IUserService _userService;
        private readonly IRequestUpvoteRepository _upvoteRepository;
        private const int PageSize = 10;

        public IndexModel(
            ICommissionRequestService commissionRequestService,
            IUserService userService,
            IRequestUpvoteRepository upvoteRepository)
        {
            _commissionRequestService = commissionRequestService;
            _userService = userService;
            _upvoteRepository = upvoteRepository;
        }

        public List<CommissionRequest> Requests { get; set; } = new();
        public Dictionary<int, string> Usernames { get; set; } = new();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Search { get; set; } = string.Empty;
        public Dictionary<int, int> UpvoteCounts { get; set; } = new();
        public HashSet<int> UserUpvotedRequests { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int currentPage = 1)
        {
            CurrentPage = currentPage;

            var allRequests = await _commissionRequestService.GetAllAsync();

            // search filter
            if (!string.IsNullOrWhiteSpace(Search))
            {
                allRequests = allRequests.Where(r =>
                    r.GameTitle.Contains(Search, StringComparison.OrdinalIgnoreCase) ||
                    (r.Description?.Contains(Search, StringComparison.OrdinalIgnoreCase) ?? false))
                    .ToList();
            }

            TotalPages = (int)Math.Ceiling(allRequests.Count / (double)PageSize);

            Requests = allRequests
                .OrderByDescending(r => r.CreatedAt)
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            // Load usernames for creators
            foreach (var req in Requests)
            {
                if (!Usernames.ContainsKey(req.UserId))
                {
                    var user = await _userService.GetByIdAsync(req.UserId);
                    Usernames[req.UserId] = user?.Username ?? "Unknown User";
                }

                // Fetch upvote count for each request
                UpvoteCounts[req.CommissionRequestId] =
                    await _upvoteRepository.GetUpvoteCountAsync(req.CommissionRequestId);
            }

            // Get current user ID from simple login
            var currentUserId = this.GetCurrentUserId(); // implement based on your session/cookie
            if (currentUserId != 0)
            {
                foreach (var req in Requests)
                {
                    if (await _upvoteRepository.HasUserUpvotedAsync(req.CommissionRequestId, currentUserId))
                    {
                        UserUpvotedRequests.Add(req.CommissionRequestId);
                    }
                }
            }

            return Page();
        }


        public async Task<IActionResult> OnPostUpvoteAsync(int requestId)
        {
            if (!this.IsAuthenticated())
                return RedirectToPage("/Login");

            var userId = this.GetCurrentUserId();
            await _upvoteRepository.UpvoteRequestAsync(requestId, userId);

            return RedirectToPage(new { currentPage = CurrentPage, search = Search });
        }

        public string GetUsername(int userId)
        {
            return Usernames.TryGetValue(userId, out var name) ? name : "Unknown User";
        }
    }
}
