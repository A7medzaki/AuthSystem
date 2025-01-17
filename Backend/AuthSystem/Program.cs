using AuthSystem.Data.Context;
using AuthSystem.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Home/Error";
            })
            .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
            {
                IConfigurationSection googleAuthSection = builder.Configuration.GetSection("Authentication:Google");
                options.ClientId = googleAuthSection["ClientId"];
                options.ClientSecret = googleAuthSection["ClientSecret"];
            })
            .AddFacebook(options =>
            {
                IConfigurationSection facebookAuthSection = builder.Configuration.GetSection("Authentication:Facebook");
                options.AppId = facebookAuthSection["AppId"];
                options.AppSecret = facebookAuthSection["AppSecret"];
            });


            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
            })
            .AddEntityFrameworkStores<AuthSystemDbContext>()
            .AddDefaultTokenProviders();


            builder.Services.AddAuthorization();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddRazorPages();

            builder.Services.AddDbContext<AuthSystemDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            }
            );

            var app = builder.Build();

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

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=LogIn}/{id?}");

            app.Run();
        }
    }
}
