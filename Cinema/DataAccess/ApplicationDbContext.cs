
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CinemaECommerce.ViewModels;

namespace CinemaECommerce.DataAccess
{
    public class ApplicationDbContext :IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
          : base(options) { }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MovieSubImg> MovieSubImgs { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<ApplicationUserOTP> applicationUserOTPs  { get; set; }
        public DbSet<CinemaECommerce.ViewModels.ValidateOTPVM> ValidateOTPVM { get; set; } = default!;
        public DbSet<CinemaECommerce.ViewModels.ResetPasswordVM> ResetPasswordVM { get; set; } = default!;
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    base.OnConfiguring(optionsBuilder);
        //    optionsBuilder.UseSqlServer("Data Source=DESKTOP-UEGOAUQ;Initial Catalog=CinemaECommerceECommerce;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");
        //}
    }
}
