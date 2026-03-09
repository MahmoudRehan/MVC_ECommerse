using Microsoft.EntityFrameworkCore;
using PP.Models.Context;
using PP.Repos.Implementation;
using PP.Repos.Inerfaces;

namespace PP
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Configure Session support for shopping cart
            // Session allows us to store data (like cart items) per user across multiple requests
            builder.Services.AddDistributedMemoryCache(); // Required for session storage
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromDays(30); // Session expires after 30 Days of inactivity
                options.Cookie.HttpOnly = true; // Security: Cookie cannot be accessed via JavaScript
                options.Cookie.IsEssential = true; // Required for GDPR compliance - session is essential for cart functionality
            });

            // Configure Entity Framework Core with SQL Server
            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IProductRepo, ProductRepo>();
            builder.Services.AddScoped<ICategoryRepo, CategoryRepo>();
            builder.Services.AddScoped<IOrderRepo, OrderRepo>();
            builder.Services.AddScoped<IAddressRepo, AddressRepo>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            // Enable Session middleware - MUST be called before UseAuthorization
            // This activates session support for the application
            app.UseSession();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Catalog}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
