
using CinemaECommerce.Repositories.IRepositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NuGet.Protocol.Core.Types;
using System.Globalization;

namespace CinemaECommerce
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<IRepository<Category>, Repository<Category>>();
            builder.Services.AddScoped<IRepository<Cinema>, Repository<Cinema>>();
            builder.Services.AddScoped<IRepository<Actor>, Repository<Actor>>();
            builder.Services.AddScoped<IRepository<Movie>, Repository<Movie>>();
            builder.Services.AddScoped<IMovieSubImgRepository, SubImgRepository>();
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
            builder.Services.AddDbContext<ApplicationDbContext>(
                option =>
                {
                    option.UseSqlServer(builder.Configuration.GetConnectionString(
                        "DefualtConnection"));
                }
                );
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(Options =>
            {
                Options.User.RequireUniqueEmail = true;
                Options.Password.RequiredLength = 8;
                Options.Lockout.MaxFailedAccessAttempts = 6;
            }).AddEntityFrameworkStores<ApplicationDbContext>();
            const string defaultCulture = "en";
            var supportedCultures = new[]
            {
                    new CultureInfo(defaultCulture),
                    new CultureInfo("ar")
            };

            builder.Services.Configure<RequestLocalizationOptions>(options => {
                options.DefaultRequestCulture = new RequestCulture(defaultCulture);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });
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

            app.UseAuthorization();
            app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Admin}/{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
