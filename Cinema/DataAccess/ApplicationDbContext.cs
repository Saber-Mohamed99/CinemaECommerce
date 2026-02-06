
using Microsoft.EntityFrameworkCore;

namespace CinemaECommerce.DataAccess
{
    public class ApplicationDbContext :DbContext
    {
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MovieSubImg> MovieSubImgs { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Actor> Actors { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Data Source=DESKTOP-UEGOAUQ;Initial Catalog=CinemaECommerceECommerce;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");
        }
    }
}
