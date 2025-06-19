using AirWaterStore.Business.Interfaces;
using AirWaterStore.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AirWaterStore.Web.Pages
{
    public class CheckoutModel : PageModel
    {
        private readonly IOrderService _orderService;
        private readonly IOrderDetailService _orderDetailService;
        private readonly IGameService _gameService;

        public CheckoutModel(IOrderService orderService, IOrderDetailService orderDetailService, IGameService gameService)
        {
            _orderService = orderService;
            _orderDetailService = orderDetailService;
            _gameService = gameService;
        }

        public List<Games.CartItem> CartItems { get; set; } = new List<Games.CartItem>();
        public decimal TotalPrice => CartItems.Sum(item => item.Price * item.Quantity);
        public string ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToPage("/Login");
            }

            CartItems = HttpContext.Session.GetObjectFromJson<List<Games.CartItem>>("Cart") ?? new List<Games.CartItem>();

            if (!CartItems.Any())
            {
                return RedirectToPage("/Cart");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToPage("/Login");
            }

            CartItems = HttpContext.Session.GetObjectFromJson<List<Games.CartItem>>("Cart") ?? new List<Games.CartItem>();

            if (!CartItems.Any())
            {
                return RedirectToPage("/Cart");
            }

            try
            {
                // Validate stock availability
                foreach (var item in CartItems)
                {
                    var game = await _gameService.GetByIdAsync(item.GameId);
                    if (game == null || game.Quantity < item.Quantity)
                    {
                        ErrorMessage = $"'{item.Title}' is out of stock or insufficient quantity available.";
                        return Page();
                    }
                }

                // Create order
                var order = new Order
                {
                    UserId = userId.Value,
                    OrderDate = DateTime.Now,
                    TotalPrice = TotalPrice,
                    Status = "Completed"
                };

                await _orderService.AddAsync(order);

                // Create order details and update stock
                foreach (var item in CartItems)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.OrderId,
                        GameId = item.GameId,
                        Quantity = item.Quantity,
                        Price = item.Price
                    };

                    await _orderDetailService.AddAsync(orderDetail);

                    // Update game stock
                    var game = await _gameService.GetByIdAsync(item.GameId);
                    game.Quantity -= item.Quantity;
                    await _gameService.UpdateAsync(game);
                }

                // Clear cart
                HttpContext.Session.Remove("Cart");

                // Redirect to order confirmation
                return RedirectToPage("/Orders/Details", new { id = order.OrderId });
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred while processing your order. Please try again.";
                return Page();
            }
        }
    }
}