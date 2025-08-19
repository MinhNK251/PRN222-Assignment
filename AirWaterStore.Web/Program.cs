using AirWaterStore.Business.Interfaces;
using AirWaterStore.Business.Library;
using AirWaterStore.Business.Services;
using AirWaterStore.Data;
using AirWaterStore.Data.Repositories;
using AirWaterStore.Web.Hubs;
using Microsoft.EntityFrameworkCore;

namespace AirWaterStore.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages().AddRazorPagesOptions(options =>
            {
                options.Conventions.AddPageRoute("/Games/Index", "");
            });
            builder.Services.AddSession();
            builder.Services.AddSignalR();
            builder.Services.AddHttpContextAccessor();

            //Configure DbContext
            builder.Services.AddDbContext<AirWaterStoreContext>(options =>
                //options.UseSqlServer(builder.Configuration.GetConnectionString("DockerConnection")));
             options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Register repositories	
            builder.Services.AddScoped<IGameRepository, GameRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IChatRoomRepository, ChatRoomRepository>();
            builder.Services.AddScoped<IMessageRepository, MessageRepository>();
            builder.Services.AddScoped<ICommissionRequestRepository, CommissionRequestRepository>();
            builder.Services.AddScoped<IRequestUpvoteRepository, RequestUpvoteRepository>();
            builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();

            // Register services	
            builder.Services.AddScoped<IGameService, GameService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IOrderDetailService, OrderDetailService>();
            builder.Services.AddScoped<IReviewService, ReviewService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IChatRoomService, ChatRoomService>();
            builder.Services.AddScoped<IMessageService, MessageService>();
            builder.Services.AddScoped<IVnPayService, VnPayService>();
            builder.Services.AddScoped<ICommissionRequestService, CommissionRequestService>();
            builder.Services.AddScoped<IRequestUpvoteService, RequestUpvoteService>();
            builder.Services.AddScoped<IWishlistService, WishlistService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IDashboardService, DashboardService>();
            builder.Services.AddScoped<CloudinaryService>();

            builder.Services.Configure<VnPayConfig>(builder.Configuration.GetSection("VnPay"));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
            app.UseAuthorization();

            app.MapRazorPages();
            app.MapHub<ChatHub>("/chathub");

            app.Run();
        }
    }
}