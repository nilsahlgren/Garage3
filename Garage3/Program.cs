using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Garage3.Data;
using Garage3.Models;

namespace Garage3
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<Garage3Context>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Garage3Context") ?? throw new InvalidOperationException("Connection string 'Garage3Context' not found.")));

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            int parkingSpaces = Int32.Parse(builder.Configuration.GetSection("ParkingSpaces").Value);
            

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {

                SeedData.Initialize(services, parkingSpaces);
                }
                catch(Exception ex)
                {


                }
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}